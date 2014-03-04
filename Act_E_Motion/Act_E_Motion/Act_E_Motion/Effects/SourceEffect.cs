using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Act_E_Motion.Effects
{
    class SourceEffect
    {

        public String source { get; set; }
        public int id { get; set; }


        public SourceEffect( int id,String source)    
        {
            this.source = source;
            this.id = id;
        }

        public static int getCount(int a){
            //0 Foto, 1 Video, 2 Sounds
            switch (a)
            {
                case 0:
                    return 3;
                case 1:
                    return 1;
                case 2:
                    return 0;
                default:
                    return 0;
            }
        }

        public static string getImageFromID(int id)
        {
            switch (id)
            {   
                case 0:
                    return "Image\\deep-sky";
                case 1:
                    return "Image\\ISS";
                case 2:
                    return "Image\\Beethoven";

                default:
                    return null;
            }
        }
        public static string getVideoFromID(int id)
        {
            switch (id)
            {
                case 0:
                    return "Video\\Wildlife";
                default:
                    return null;
            }
        }
        public static string getSongFromId(int i)
        {
            switch (i)
            {
                case 0:
                    return "Music\\Debussy";
                case 1:
                    return "Music\\Mozart";
                case 2:
                    return "Music\\Beethoven";
                default:
                    return null;
            }
        }
        public static string getSoundFromId(int i)
        {
            switch (i)
            {
                default:
                    return null;
            }
        }
    }
}
