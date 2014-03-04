using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Research.Kinect.Nui;
using System.IO;
using Act_E_Motion.Effects;
using Microsoft.Xna.Framework.Audio;

namespace Act_E_Motion
{
    /// <summary>
    /// Dies ist die Hauptklasse von act_e_motion
    /// </summary>
    public class MyGame : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private const int WIDTH = 640;
        private const int HEIGHT = 480;
        private SpriteBatch spriteBatch;

        //Buttons
        private bool FLock = false;
        private bool NLock = false;

        //myImages
        private Texture2D[] mImages;

        //myVideos
        private Video[] mVideos;

        //mySoundEffects
        private SoundEffect[] mSoundFX;

        //my bg Song
        private Song mSong;
        private int totalTime = 0;

        Texture2D myTex;

        //Kinect-Initialisierungen
        private Runtime kinectRuntime;
        private SpriteFont myFont;

        public static bool DEBUG = false;

        private int presentationStart = 0;

        VideoPlayer player;

        private bool SKELETON = true;
        private bool poseLock = false;

        private int lastPoseTime = 0;

        private int totalFrames;
        private DateTime lastTime;
        private int lastFrames;
        private string frameRate;

        private Show mShow;
        private ShowLoader mShowLoader;
        private GameTime currentGameTime;

        public MyGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content = new ContentManager(Services);

            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;
            graphics.IsFullScreen = true;
            //graphics.PreferMultiSampling = false;
            //Window.AllowUserResizing = true;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Ermöglicht dem Spiel die Durchführung einer Initialisierung, die es benötigt, bevor es ausgeführt werden kann.
        /// Dort kann es erforderliche Dienste abfragen und nicht mit der Grafik
        /// verbundenen Content laden.  Bei Aufruf von base.Initialize werden alle Komponenten aufgezählt
        /// sowie initialisiert.
        /// </summary>
        protected override void Initialize()
        {
            try
            {
                kinectRuntime = Runtime.Kinects[0];
                kinectRuntime.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Runtime initialization failed. Please make sure Kinect device is plugged in.\n" + ex.Message);
                return;
            }

            kinectRuntime.DepthFrameReady += DepthFrameReady;
            kinectRuntime.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.DepthAndPlayerIndex);

            if (SKELETON)
            {
                kinectRuntime.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);

                var parameters = new TransformSmoothParameters
                {
                    Smoothing = 0.75f,
                    Correction = 0.0f,
                    Prediction = 0.0f,
                    JitterRadius = 0.05f,
                    MaxDeviationRadius = 0.04f
                };
                kinectRuntime.SkeletonEngine.SmoothParameters = parameters;
            }



            mShowLoader = new ShowLoader();
            mShow = mShowLoader.mShow;


            ////Test ohne json-file
            //int time = 5;
            //List<Scene> mtimeLine = new List<Scene>();
            //Scene mSceneFoto = new Scene(new SourceEffect(Utils.PICTURE_ID, "0"), time, "pic");
            //Scene mSceneVideo = new Scene(new SourceEffect(Utils.VIDEO_ID, "0"), time, "video");
            //Scene mSceneSound = new Scene(new SourceEffect(Utils.SOUND_FX_ID, "0"), time, "sound");

            //Scene mScene = new Scene(new ColorValueEffect(Utils.BORDER_ID, "#fff222", "3"), time, "border");
            //Scene mScene2 = new Scene(new ColorValueEffect(Utils.LINES_ID, "#fff222", "0"), time, "lines0");
            //Scene mScene3 = new Scene(new ColorValueEffect(Utils.LINES_ID, "#fff222", "1"), time, "lines1");
            //Scene mScene4 = new Scene(new ColorValueEffect(Utils.LINES_ID, "#fff222", "2"), time, "lines2");
            //Scene mScene5 = new Scene(new ColorValueEffect(Utils.LINES_ID, "#fff222", "3"), time, "lines3");
            //Scene mScene6 = new Scene(new ColorValueEffect(Utils.LINES_ID, "#fff222", "4"), time, "lines4");
            //Scene mScene7 = new Scene(new ColorValueEffect(Utils.LINES_ID, "#fff222", "5"), time, "lines5");
            //Scene mScene8 = new Scene(new ColorValueEffect(Utils.ROOM_ID, "#777555", "10"), time, "room");
            //Scene mScene9 = new Scene(new ColorValueEffect(Utils.CRAZY_COLORS_ID, "#fff222", "#222000"), time, "cc");
            //Scene mScene10 = new Scene(new ColorValueEffect(Utils.COLORED_SHADOW_ID, "#000000", "10"), time, "Colored Shadow");
            //Scene mScene11 = new Scene(new ColorValueEffect(Utils.SILHOUETTE_ID, "#ffffff", "#000000"), time, "silhouette");
            //Scene mScene12 = new Scene(new ColorValueEffect(Utils.PAINT_ID, "#0000FF", "2000"), time, "Paint");

            //List<Flash> mflashes = new List<Flash>();

            //Flash fl = new Flash(new ColorValueEffect(Utils.ROOM_ID, "#777555", "10"), time, 0);
            //mflashes.Add(fl);


            //mtimeLine.Add(mScene);
            //mtimeLine.Add(mScene2);
            //mtimeLine.Add(mScene3);
            //mtimeLine.Add(mScene4);
            //mtimeLine.Add(mScene5);
            //mtimeLine.Add(mScene6);
            //mtimeLine.Add(mScene7);
            //mtimeLine.Add(mScene8);
            //mtimeLine.Add(mScene9);
            //mtimeLine.Add(mScene10);
            //mtimeLine.Add(mScene11);
            //mtimeLine.Add(mScene12);

            //mtimeLine.Add(mSceneFoto);
            //mtimeLine.Add(mSceneVideo);
            ////mtimeLine.Add(mSceneSound);

            ////Loading the Show;
            //mShow = new Show("myFirstShow", "this is my first show", Utils.TIME_CHANGE, "#444666", 0, mtimeLine, mflashes);

            mShow.currentScene = mShow.timeline.ElementAt(0);

            //Init Videoplayer
            player = new VideoPlayer();
            player.IsLooped = false;

            base.Initialize();
        }


        //--------------------------SKELETON------------------------------------

        void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            // Change on Pose
            if (mShow.type == Utils.POSE_CHANGE)
            {
                SkeletonFrame skeletonFrame = e.SkeletonFrame;

                if (skeletonFrame == null) return;

                foreach (SkeletonData skeleton in skeletonFrame.Skeletons)
                {
                    // Unschöne Methode - Sollte man irgendwie anders lösen
                    Pose Y = new Pose(0, "Y", Pose.getArrayFromID(0, skeleton));

                    if (SkeletonTrackingState.Tracked == skeleton.TrackingState)
                    {
                        if (skeleton.Joints.Count > 0)
                        {
                            if (Y.fits() && poseLock == false)
                            {
                                lastPoseTime = DateTime.Now.Minute * 60 + DateTime.Now.Second;
                                nextEffect();
                                poseLock = true;
                            }
                            else if (poseLock && lastPoseTime + Utils.POSE_TIME_OFFSET < DateTime.Now.Minute * 60 + DateTime.Now.Second)
                            {
                                poseLock = false;
                            }
                        }
                    }
                }
            }
            // Flashes
            else if (mShow.flashes != null && mShow.type == Utils.TIME_CHANGE)
            {
                SkeletonFrame skeletonFrame = e.SkeletonFrame;

                if (skeletonFrame == null) return;

                foreach (SkeletonData skeleton in skeletonFrame.Skeletons)
                {
                    if (SkeletonTrackingState.Tracked == skeleton.TrackingState)
                    {
                        if (skeleton.Joints.Count > 0)
                        {
                            for (int i = 0; i < mShow.flashes.Count; i++)
                            {
                                int Z = mShow.flashes.ElementAt(i).pose_id;
                                Pose Y = new Pose(Z, i + "" + Z, Pose.getArrayFromID(Z, skeleton));


                                if (Y.fits())
                                {
                                    mShow.flashes.ElementAt(i).active = true;
                                }
                                else
                                {
                                    mShow.flashes.ElementAt(i).active = false;
                                }
                            }
                        }
                    }
                }
            }
            //Screenshot on pose if no flashes are selected
            else if (mShow.flashes == null && mShow.type == Utils.TIME_CHANGE)
            {
                SkeletonFrame skeletonFrame = e.SkeletonFrame;

                if (skeletonFrame == null) return;

                foreach (SkeletonData skeleton in skeletonFrame.Skeletons)
                {
                    Pose Y = new Pose(0, "Y", Pose.getArrayFromID(0, skeleton));
                    if (SkeletonTrackingState.Tracked == skeleton.TrackingState)
                    {
                        if (skeleton.Joints.Count > 0)
                        {
                            if (Y.fits() && poseLock == false)
                            {
                                lastPoseTime = DateTime.Now.Minute * 60 + DateTime.Now.Second;
                                makeScreenshot(currentGameTime);
                                poseLock = true;
                            }
                            else if (poseLock && lastPoseTime + Utils.POSE_TIME_OFFSET < DateTime.Now.Minute * 60 + DateTime.Now.Second)
                            {
                                poseLock = false;
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// LoadContent wird einmal pro Spiel aufgerufen und ist der Platz, wo
        /// Ihr gesamter Content geladen wird.
        /// </summary>
        protected override void LoadContent()
        {
            // Erstellen Sie einen neuen SpriteBatch, der zum Zeichnen von Texturen verwendet werden kann.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            kinectRuntime.NuiCamera.ElevationAngle = 10;

            //Lade alle Bilder
            mImages = new Texture2D[SourceEffect.getCount(0)];
            for (int i = 0; i < mImages.Length; i++)
            {
                mImages[i] = this.Content.Load<Texture2D>(SourceEffect.getImageFromID(i));
            }

            //Lade alle Videos
            mVideos = new Video[SourceEffect.getCount(1)];

            for (int k = 0; k < mVideos.Length; k++)
            {
                mVideos[k] = this.Content.Load<Video>(SourceEffect.getVideoFromID(k));
            }

            //Lade alle SoundEffecte
            mSoundFX = new SoundEffect[SourceEffect.getCount(2)];

            for (int k = 0; k < mSoundFX.Length; k++)
            {
                mSoundFX[k] = this.Content.Load<SoundEffect>(SourceEffect.getSoundFromId(k));
            }

            if (SourceEffect.getSongFromId(mShow.bgMusic) != null)
            {

                mSong = this.Content.Load<Song>(SourceEffect.getSongFromId(mShow.bgMusic));
            }

            myFont = Content.Load<SpriteFont>(@"SpriteFont1");

            //Start Video if its first
            if (mShow.currentScene.mSourceEffect != null && mShow.currentScene.mSourceEffect.id == Utils.VIDEO_ID)
            {
                player.Play(mVideos[Convert.ToInt16(mShow.currentScene.mSourceEffect.source)]);
            }
            //Start Soundeffect if its first
            if (mShow.currentScene.mSourceEffect != null && mShow.currentScene.mSourceEffect.id == Utils.SOUND_FX_ID)
            {
                mSoundFX[Convert.ToInt16(mShow.currentScene.mSourceEffect.source)].Play();
            }
            //Start BG Song, if it exists
            if (mSong != null)
            {
                MediaPlayer.Play(mSong);
            }

        }

        /// <summary>
        /// UnloadContent wird einmal pro Spiel aufgerufen und ist der Ort, wo
        /// Ihr gesamter Content entladen wird.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Entladen Sie jeglichen Nicht-ContentManager-Content hier
            MediaPlayer.Stop();
        }

        /// <summary>
        /// OnExiting wird aufgerufen, wenn die Anwendung geschlossen wird. Dann muss auch die Kinect-Umgebung beendet werden
        /// </summary>
        protected override void OnExiting(object sender, EventArgs args)
        {
            kinectRuntime.Uninitialize();
            base.OnExiting(sender, args);
        }

        private void DepthFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            CalculateFps();

            if (mShow.currentScene.mColorEffect != null)
            {
                int[] playercoded = mShow.currentScene.mColorEffect.doMagic(e.ImageFrame.Image.Width, e.ImageFrame.Image.Height, (byte[])(e.ImageFrame.Image.Bits));

                myTex = new Texture2D(GraphicsDevice, e.ImageFrame.Image.Width, e.ImageFrame.Image.Height);
                myTex.SetData(playercoded);
            }
            if (mShow.flashes != null)
            {
                for (int i = 0; i < mShow.flashes.Count; i++)
                {
                    if (mShow.flashes.ElementAt(i).active && mShow.flashes.ElementAt(i).colorEffect != null)
                    {
                        int[] playercoded = mShow.flashes.ElementAt(i).colorEffect.doMagic(e.ImageFrame.Image.Width, e.ImageFrame.Image.Height, (byte[])(e.ImageFrame.Image.Bits));

                        myTex = new Texture2D(GraphicsDevice, e.ImageFrame.Image.Width, e.ImageFrame.Image.Height);
                        myTex.SetData(playercoded);
                    }
                }
            }

        }

        void CalculateFps()
        {
            ++totalFrames;

            var cur = DateTime.Now;
            if (cur.Subtract(lastTime) > TimeSpan.FromSeconds(1))
            {
                int frameDiff = totalFrames - lastFrames;
                lastFrames = totalFrames;
                lastTime = cur;
                frameRate = frameDiff.ToString() + " fps";
            }
        }

        /// <summary>
        /// Ermöglicht dem Spiel die Ausführung der Logik, wie zum Beispiel Aktualisierung der Welt,
        /// Überprüfung auf Kollisionen, Erfassung von Eingaben und Abspielen von Ton.
        /// </summary>
        /// <param name="gameTime">Bietet einen Schnappschuss der Timing-Werte.</param>
        protected override void Update(GameTime gameTime)
        {
            this.currentGameTime = gameTime;
            //Handling Video
            if (mShow.currentScene.mSourceEffect != null && mShow.currentScene.mSourceEffect.id == Utils.VIDEO_ID)
            {
                if (player.State == MediaState.Stopped)
                {
                    player.Play(mVideos[Convert.ToInt16(mShow.currentScene.mSourceEffect.source)]);
                }
            }
            else if (player.State == MediaState.Playing)
            {
                player.Stop();
            }

            //Handling bg Song
            if (mSong != null)
            {
                if (MediaPlayer.State == MediaState.Stopped)
                {
                    MediaPlayer.Play(mSong);
                }
            }

            KeyboardState myState = Keyboard.GetState(PlayerIndex.One);
            // Ermöglicht ein Beenden des Spiels
            if (myState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }
            if (myState.IsKeyDown(Keys.N) && NLock == false)
            {
                nextEffect();
                NLock = true;
            }

            else if ((myState.IsKeyDown(Keys.F) && FLock == false))
            {
                makeScreenshot(gameTime);

                FLock = true;
            }
            //------------Release Buttons
            if (Keyboard.GetState().IsKeyUp(Keys.F))
            {
                FLock = false;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.N))
            {
                NLock = false;
            }

            base.Update(gameTime);
        }

        private void makeScreenshot(GameTime gameTime)
        {
            try
            {
                //force a frame to be drawn (otherwise back buffer is empty) 
                this.totalTime += gameTime.TotalGameTime.Seconds;
                Draw(new GameTime());

                //int w = GraphicsDevice.DisplayMode.Width;
                //int h = GraphicsDevice.DisplayMode.Height;
                //pull the picture from the buffer 
                int[] backBuffer = new int[WIDTH * HEIGHT];
                GraphicsDevice.GetBackBufferData(backBuffer);

                //copy into a texture 
                Texture2D texture = new Texture2D(GraphicsDevice, WIDTH, HEIGHT, false, GraphicsDevice.PresentationParameters.BackBufferFormat);
                texture.SetData(backBuffer);

                //save to disk 
                Stream stream = File.OpenWrite(mShow.currentScene.name + this.totalTime + ".jpg");

                texture.SaveAsJpeg(stream, WIDTH, HEIGHT);
                stream.Dispose();

                texture.Dispose();
                Console.WriteLine("FOTO");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        private void nextEffect()
        {

            if (mShow.timeline.IndexOf(mShow.currentScene) != mShow.timeline.Count - 1)
            {
                mShow.currentScene = mShow.timeline.ElementAt(mShow.timeline.IndexOf(mShow.currentScene) + 1);
            }
            else
            {
                mShow.currentScene = mShow.timeline.ElementAt(0);
            }


            //Start Video if its time
            if (mShow.currentScene.mSourceEffect != null && mShow.currentScene.mSourceEffect.id == Utils.VIDEO_ID)
            {
                player.Play(mVideos[Convert.ToInt16(mShow.currentScene.mSourceEffect.source)]);
            }

            //Start SoundEffect if its time
            if (mShow.currentScene.mSourceEffect != null && mShow.currentScene.mSourceEffect.id == Utils.SOUND_FX_ID)
            {
                mSoundFX[Convert.ToInt16(mShow.currentScene.mSourceEffect.source)].Play();
            }

        }
        /// <summary>
        /// Dies wird aufgerufen, wenn das Spiel selbst zeichnen soll.
        /// </summary>
        /// <param name="gameTime">Bietet einen Schnappschuss der Timing-Werte.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Utils.ToColor(mShow.bgColor));

            spriteBatch.Begin();

            if (mShow.currentScene.mSourceEffect != null)
            {

                if (mShow.currentScene.mSourceEffect.id == Utils.VIDEO_ID)
                {
                    if (player.State != MediaState.Stopped)
                    {
                        myTex = player.GetTexture();
                    }
                }
                else if (mShow.currentScene.mSourceEffect.id == Utils.PICTURE_ID)
                {
                    myTex = mImages[Convert.ToInt16(mShow.currentScene.mSourceEffect.source)];
                }

                else if (mShow.currentScene.mSourceEffect.id == Utils.SOUND_FX_ID)
                {
                    myTex = new Texture2D(GraphicsDevice, WIDTH, HEIGHT);
                }
            }

            if (myTex != null)
            {
                if (DEBUG)
                {
                    String debug_txt = mShow.currentScene.name + "- t: " + gameTime.TotalGameTime.Seconds + " - r: " + frameRate;
                    spriteBatch.DrawString(myFont, debug_txt, new Vector2(0, HEIGHT - 25), Color.White);
                    spriteBatch.Draw(myTex, new Rectangle(0, 0, WIDTH, HEIGHT - 25), null, Color.White, 0, new Vector2(), SpriteEffects.None, 1);
                }
                else
                {
                    spriteBatch.Draw(myTex, new Rectangle(0, 0, WIDTH, HEIGHT), null, Color.White, 0, new Vector2(), SpriteEffects.None, 1);
                }
            }

            if (mShow.type == Utils.TIME_CHANGE)
            {
                if (mShow.currentScene.duration + presentationStart < gameTime.TotalGameTime.Hours * 3600 + gameTime.TotalGameTime.Minutes * 60 + gameTime.TotalGameTime.Seconds)
                {
                    presentationStart = gameTime.TotalGameTime.Hours * 3600 + gameTime.TotalGameTime.Minutes * 60 + gameTime.TotalGameTime.Seconds;
                    nextEffect();
                }
            }


            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
