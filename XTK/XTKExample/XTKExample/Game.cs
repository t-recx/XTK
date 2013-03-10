using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using GameResolution;

namespace XTKExample
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameResolution.VirtualScreen virtualScreen;
        XTK.WindowManager windowManager = new XTK.WindowManager(640, 480);

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            virtualScreen = new VirtualScreen(640, 480, GraphicsDevice);
            Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
            Window.AllowUserResizing = true;
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            virtualScreen.PhysicalResolutionChanged();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        /// 
        XTK.Label lbl;

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            XTK.Theme Theme = XTK.ThemeIO.Load("./XTK.THX", GraphicsDevice);
            Theme.Font = Content.Load<SpriteFont>("Default");
            Theme.FontBold = Content.Load<SpriteFont>("DefaultBold");

            windowManager.Theme = Theme;

            frmNotes testForm1 = new frmNotes(windowManager) { X = 120, Y = 40, Width = 320, Height = 240, BackColor = new Color(170, 170, 170), Text = "Notes" };
            frmTest testForm2 = new frmTest(windowManager) { X = 15, Y = 10, MinimumWidth = 230, Width = 230, MinimumHeight = 240, Height = 240, 
                BackColor = new Color(170, 170, 170), Text = "Window", MaximumWidth = 320, MaximumHeight = 400 };
            frmMixer testForm3 = new frmMixer(windowManager) { X = 260, Y = 30, Width = 185, Height = 160, BackColor = new Color(170, 170, 170), Text = "Mixer", Resizable = false };
            frmListBoxes testForm4 = new frmListBoxes(windowManager)
            {
                X = 105,
                Y = 260,
                MinimumWidth = 230,
                Width = 340,
                MinimumHeight = 130,
                Height = 130,
                BackColor = new Color(170, 170, 170),
                Text = "ListBoxes"
            };

            windowManager.Forms.Add(testForm1);
            windowManager.Forms.Add(testForm2);
            windowManager.Forms.Add(testForm3);
            windowManager.Forms.Add(testForm4);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            virtualScreen.Update();

            windowManager.Update(gameTime, virtualScreen);
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            virtualScreen.BeginCapture();
            spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, null, null); 

            GraphicsDevice.Clear(Color.Black);
            // drawing goes here
            windowManager.Draw(GraphicsDevice, spriteBatch);

            spriteBatch.End();
            virtualScreen.EndCapture();

            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null);
            virtualScreen.Draw(spriteBatch, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
