﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using Engine;

namespace CFNGamejam2
{
    public class Game1 : Game
    {
        GraphicsDeviceManager Graphics;
        SpriteBatch spriteBatch;
        GameLogic TheGame;

        Timer FPSTimer;
        float FPSFrames = 0;

        KeyboardState OldKeyState;
        bool PauseGame = false;

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Graphics.IsFullScreen = false;
            Graphics.SynchronizeWithVerticalRetrace = true; //When true, 60FSP refresh rate locked.
            Graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Graphics.PreferredBackBufferWidth = 1200;
            Graphics.PreferredBackBufferHeight = 900;
            Graphics.PreferMultiSampling = true; //Error in MonoGame 3.6 for DirectX, fixed in version 3.7.
            Graphics.PreparingDeviceSettings += SetMultiSampling;
            Graphics.ApplyChanges();
            Graphics.GraphicsDevice.RasterizerState = new RasterizerState(); //Must be after Apply Changes.
            Core.GameRef = this;
            IsFixedTimeStep = false;
            Content.RootDirectory = "Content";
            // Positive Y is Up. Positive X is Right.
            Core.Initialize(Graphics, this, new Vector3(0, 200, 600), false, 1, 10000);

            TheGame = new GameLogic(this);
            FPSTimer = new Timer(this, 1);
        }

        private void SetMultiSampling(object sender, PreparingDeviceSettingsEventArgs eventArgs)
        {
            PresentationParameters PresentParm = eventArgs.GraphicsDeviceInformation.PresentationParameters;
            PresentParm.MultiSampleCount = 8;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Setup lighting.
            Core.DefuseLight = new Vector3(0.6f, 0.5f, 0.7f);
            Core.LightDirection = new Vector3(-0.75f, -0.75f, -0.5f);
            Core.SpecularColor = new Vector3(0.1f, 0, 0.5f);
            Core.AmbientLightColor = new Vector3(0.25f, 0.25f, 0.25f); // Add some overall ambient light.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void BeginRun()
        {
            base.BeginRun();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState KBS = Keyboard.GetState();

            if (TheGame.CurrentMode == GameState.InPlay)
            {
                if (!OldKeyState.IsKeyDown(Keys.P) && KBS.IsKeyDown(Keys.P))
                    PauseGame = !PauseGame;
            }

            OldKeyState = Keyboard.GetState();

            if (!PauseGame)
                base.Update(gameTime);

            FPSFrames++;

            if (FPSTimer.Elapsed)
            {
                FPSTimer.Reset();
                System.Diagnostics.Debug.WriteLine("FPS " + FPSFrames.ToString());
                FPSFrames = 0;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(0.05f, 0, 0.2f, 1));

            base.Draw(gameTime);
        }
    }
}
