using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;

namespace Act_E_Motion
{
    public static class Utils
    {
        public static int PICTURE_ID = 0;
        public static int VIDEO_ID = 1;
        public static int SOUND_FX_ID = 2;

        public static int CRAZY_COLORS_ID = 3;
        public static int COLORED_SHADOW_ID = 4;
        public static int BORDER_ID = 5;
        public static int LINES_ID = 6;
        public static int ROOM_ID = 7;
        public static int SILHOUETTE_ID = 8;
        public static int PAINT_ID = 9;
        
        public static int NUM_SHADOWS = 5;

        public static int TIME_CHANGE = 0;
        public static int POSE_CHANGE = 1;

        public static int POSE_TIME_OFFSET = 3;

        
        //Blink-Effekt
        public static int[] getPlayerBorderBlink(int[] ar, Color color = new Color())
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
                    if (counter % 50 == 0)
                    {
                        result[i] = ar[i];
                    }
                }
            }
            return result;
        }

        public static Color ToColor(this string hexString)
        {
            if (hexString.StartsWith("#"))
                hexString = hexString.Substring(1);
            uint hex = uint.Parse(hexString, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            Color color = Color.White;
            if (hexString.Length == 8)
            {
                color.A = (byte)(hex >> 24);
                color.R = (byte)(hex >> 16);
                color.G = (byte)(hex >> 8);
                color.B = (byte)(hex);
            }
            else if (hexString.Length == 6)
            {
                color.R = (byte)(hex >> 16);
                color.G = (byte)(hex >> 8);
                color.B = (byte)(hex);
            }
            else
            {
                throw new InvalidOperationException("Invald hex representation of an ARGB or RGB color value.");
            }
            return color;
        }
        

        public static JointID getJointbyID(int i)
        {
            switch (i)
            {
                case 0:
                    return JointID.HipCenter;
                case 1:
                    return JointID.Spine;
                case 2:
                    return JointID.ShoulderCenter;
                case 3:
                    return JointID.Head;
                case 4:
                    return JointID.ShoulderLeft;
                case 5:
                    return JointID.ElbowLeft;
                case 6:
                    return JointID.WristLeft;
                case 7:
                    return JointID.HandLeft;
                case 8:
                    return JointID.ShoulderRight;
                case 9:
                    return JointID.ElbowRight;
                case 10:
                    return JointID.WristRight;
                case 11:
                    return JointID.HandRight;
                case 12:
                    return JointID.HipLeft;
                case 13:
                    return JointID.KneeLeft;
                case 14:
                    return JointID.AnkleLeft;
                case 15:
                    return JointID.FootLeft;
                case 16:
                    return JointID.HipRight;
                case 17:
                    return JointID.KneeRight;
                case 18:
                    return JointID.AnkleRight;
                case 19:
                    return JointID.FootRight;
            }
            return JointID.Head;
        }

       
    }
}
