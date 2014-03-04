using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using System.Globalization;

namespace Act_E_Motion.Effects
{
    class ColorValueEffect
    {

        public Color mainColor { get; set; }
        public String value { get; set; }
        public Color value_Color { get; set; }
        public int value_Int { get; set; }
        public int[][] oldPlayerCoded { get; set; }
        public int shadowCounter { get; set; }

        public int type_id { get; set; }

        public ColorValueEffect(int id, String mcolor, String val)
        {
            this.mainColor = Utils.ToColor(mcolor);
            this.value = val;
            this.type_id = id;

            if (this.type_id == Utils.SILHOUETTE_ID || this.type_id == Utils.CRAZY_COLORS_ID)
            {
                this.value_Color = Utils.ToColor(value);
            }
            else if (this.type_id == Utils.ROOM_ID || this.type_id == Utils.LINES_ID || this.type_id == Utils.COLORED_SHADOW_ID || this.type_id == Utils.PAINT_ID)
            {
                this.value_Int = Convert.ToInt16(value);
            }

            if (this.type_id == Utils.CRAZY_COLORS_ID || this.type_id == Utils.COLORED_SHADOW_ID || this.type_id == Utils.PAINT_ID )
            {
                oldPlayerCoded = new int[Utils.NUM_SHADOWS][];
                shadowCounter = 0;
            }
        }

        public int[] doMagic(int width, int height, byte[] bits)
        {
            int[] depth = new int[width * height];
            int[] player = new int[width * height];
            int[] playercoded = new int[width * height];


            for (int i = 0; i < depth.Length; i++)
            {
                //The first three Bits of every second Byte are for the player index
                player[i] = bits[i * 2] & 0x07;
                //The next 13 Bits are for the depth-Image
                depth[i] = (bits[i * 2 + 1] << 5) | (bits[i * 2] >> 3);

                if (this.type_id == Utils.ROOM_ID)
                {
                    playercoded[i] = (this.mainColor.A << 24) | (this.mainColor.B << 16) | (this.mainColor.G << 8) | (this.mainColor).R;
                        byte value = (byte)(((depth[i] - 800)) / (value_Int+1));
                        playercoded[i] -= value;
                }
                else if (this.type_id == Utils.CRAZY_COLORS_ID)
                {
                    if (((player[i] & 0x01) == 0x01 || (player[i] & 0x02) == 0x02 || (player[i] & 0x04) == 0x04))
                    {
                        playercoded[i] = (this.mainColor.A << 24) | (this.mainColor.B << 16) | (this.mainColor.G << 8) | (this.mainColor).R;
                    }
                    else if (shadowCounter > 0 && Utils.NUM_SHADOWS > 0)
                    {
                        playercoded[i] = oldPlayerCoded[(shadowCounter - 1) % Utils.NUM_SHADOWS][i];
                        playercoded[i] -= (int)value_Color.PackedValue;
                    }
                }
                else if (this.type_id == Utils.COLORED_SHADOW_ID)
                {
                    if (((player[i] & 0x01) == 0x01 || (player[i] & 0x02) == 0x02 || (player[i] & 0x04) == 0x04))
                    {
                        playercoded[i] = (this.mainColor.A << 24) | (this.mainColor.B << 16) | (this.mainColor.G << 8) | (this.mainColor).R;
                    }
                    else if (shadowCounter > 0 && Utils.NUM_SHADOWS > 0)
                    {
                        playercoded[i] = oldPlayerCoded[(shadowCounter - 1) % Utils.NUM_SHADOWS][i];
                        playercoded[i] /= (value_Int+1);
                    }
                }
                else if (this.type_id == Utils.PAINT_ID)
                {
                    if (((player[i] & 0x01) == 0x01 || (player[i] & 0x02) == 0x02 || (player[i] & 0x04) == 0x04) && depth[i] < this.value_Int)
                    {
                        playercoded[i] = (this.mainColor.A << 24) | (this.mainColor.B << 16) | (this.mainColor.G << 8) | (this.mainColor).R;
                    }
                    else if (shadowCounter > 0 && Utils.NUM_SHADOWS > 0)
                    {
                        playercoded[i] = oldPlayerCoded[(shadowCounter - 1) % Utils.NUM_SHADOWS][i];
                    }
                }
                else if (this.type_id == Utils.SILHOUETTE_ID)
                {
                    if (((player[i] & 0x01) == 0x01 || (player[i] & 0x02) == 0x02 || (player[i] & 0x04) == 0x04))
                    {
                        playercoded[i] = (this.mainColor.A << 24) | (this.mainColor.B << 16) | (this.mainColor.G << 8) | (this.mainColor).R;

                    }
                    else
                    {
                        playercoded[i] = (this.value_Color.A << 24) | (this.value_Color.B << 16) | (this.value_Color.G << 8) | (this.value_Color).R;
                    }
                }
            }

            if (this.type_id == Utils.BORDER_ID)
            {
                playercoded = getPlayerBorder(player, this.mainColor);
            }
            else if (this.type_id == Utils.LINES_ID)
            {
                playercoded = getPlayerLines(player, 20, this.value_Int, this.mainColor);
            }

            if (this.type_id == Utils.CRAZY_COLORS_ID || this.type_id == Utils.COLORED_SHADOW_ID || this.type_id == Utils.PAINT_ID)
            {
                oldPlayerCoded[shadowCounter % Utils.NUM_SHADOWS] = playercoded;
                shadowCounter++;
            }

            return playercoded;
        }

        //Border-Effect
        public static int[] getPlayerBorder(int[] ar, Color color = new Color())
        {
            int cr = (color.B << 16) | (color.G << 8) | color.R;
            int[] result = new int[ar.Length];
            for (int i = 1; i < ar.Length - 1; i++)
            {

                if (ar[i] != 0)
                {
                    if (ar[i - 1] == 0 || ar[i + 1] == 0)
                        result[i] = cr;
                }
                else
                {
                    result[i] = 0;
                }
            }
            return result;
        }

        //Lines-Effect
        public static int[] getPlayerLines(int[] ar, int value = 20, int mode = 0, Color color = new Color())
        {
            int cr = (color.A << 24) | (color.B << 16) | (color.G << 8) | color.R;
            int[] result = new int[ar.Length];
            //get the connection points
            int counter = 0;
            for (int i = 0; i < ar.Length; i++)
            {
                if (ar[i] != 0)
                {
                    counter++;
                    //verticle lines
                    if (mode == 0)
                    {
                        if (i % (320 / value) == 0 && ar[i] != 0)
                        {
                            result[i] = cr;
                        }
                    }
                    //horizonale lines
                    else if (mode == 1)
                    {
                        if (i / 320 % (240 / value) == 0 && ar[i] != 0)
                        {
                            result[i] = cr;
                        }
                    }
                    //diagonale lines

                    else if (mode == 2)
                    {
                        if (i % ((321 / 6)) == 0 && (ar[i] != 0))
                        {
                            result[i] = cr;
                        }
                    }
                    //dotted
                    else if (mode == 3)
                    {
                        if (i % ((311 / value)) == 0 && (ar[i] != 0))
                        {
                            result[i] = cr;
                        }
                    }
                    //Combination of 1 and 2
                    else if (mode == 4)
                    {
                        if ((i % (320 / value) == 0 && ar[i] != 0) || ((i / 320 % (240 / value) == 0 && ar[i] != 0)))
                        {
                            result[i] = cr;
                        }
                    }
                    //Rectangles
                    else if (mode == 5)
                    {
                        value = (value < 2) ? 2 : value;
                        if ((i / 320) % (value / 2) != 0 && (i % 320) % (value / 2) != 0 && (ar[i] != 0))
                        {
                            result[i] = cr;
                        }
                    }
                }
            }
            return result;
        }
    }
}
