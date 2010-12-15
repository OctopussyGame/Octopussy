#region Using Statements
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
#endregion

namespace Octopussy
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Microsoft.Xna.Framework.Game
    {
        #region Fields

        GraphicsDeviceManager graphics;
        ScreenManager screenManager;
        private PreferenceManager preferenceManager;

        #endregion

        #region Initialization

        static readonly string[] preloadAssets =
        {
            "images/gradient",
        };

        public MainGame()
        {
            // Basic initialization
            Window.Title = "Octopussy Game";
            Content.RootDirectory = "Content";
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            // graphics.IsFullScreen = true;

            graphics.ApplyChanges();

            // Create the screen manager component.
            screenManager = new ScreenManager(this);

            PreferenceManager = new PreferenceManager();

            Components.Add(screenManager);

            // Activate the first screens.
            screenManager.AddScreen(new MainMenuScreen(), null);
        }

        public PreferenceManager PreferenceManager
        {
            get { return preferenceManager; }
            set { preferenceManager = value; }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Initialize our renderer
            DebugShapeRenderer.Initialize(GraphicsDevice);
            
            base.Initialize();
        }

        /// <summary>
        /// Loads graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            foreach (string asset in preloadAssets) {
                Content.Load<object>(asset);
            }
        }
        
        #endregion

        #region Draw


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }


        #endregion

        
    }
}
