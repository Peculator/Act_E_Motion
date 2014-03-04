using System;

namespace Act_E_Motion
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        static void Main(string[] args)
        {
            using (MyGame game = new MyGame())
            {
                if (args.Length > 0)
                {
                    if (args[0] == "debug")
                    {
                        MyGame.DEBUG = true;
                    }
                }
                game.Run();
            }
        }
    }
#endif
}

