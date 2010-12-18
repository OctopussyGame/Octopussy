#region Using Statements

using Microsoft.Xna.Framework;
using Octopussy.Game.Screens;
using Octopussy.Managers.PreferenceManager;
using Octopussy.Managers.ScreenManager;
using Octopussy.Utils;

#endregion

namespace Octopussy
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Microsoft.Xna.Framework.Game
    {
        #region Fields

        private readonly GraphicsDeviceManager graphics;
        private readonly ScreenManager screenManager;

        #endregion

        #region Initialization

        private static readonly string[] preloadAssets =
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
            //graphics.IsFullScreen = true;

            graphics.ApplyChanges();

            // Create the screen manager component.
            screenManager = new ScreenManager(this);

            PreferenceManager = new PreferenceManager();

            Components.Add(screenManager);

            // Activate the first screens.
            screenManager.AddScreen(new MainMenuScreen(), null);
        }

        public PreferenceManager PreferenceManager { get; set; }

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
            foreach (string asset in preloadAssets)
            {
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