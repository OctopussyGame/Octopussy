#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

using System;
using Microsoft.Xna.Framework;
<<<<<<< HEAD:Octopussy/Octopussy/Screens/MainMenuScreen.cs
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

=======
using Microsoft.Xna.Framework.Audio;
>>>>>>> 8790126693b4096f969055446c21a380ce038165:Octopussy/Octopussy/Game/Screens/MainMenuScreen.cs
#endregion

namespace Octopussy
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Fields

        private SoundEffectInstance backgroundSound;

        #endregion

        #region Initialization

        ContentManager content;
        Texture2D backgroundTexture;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("Main Menu")
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            
            // Create our menu entries.
           /* var campaignMenuEntry = new MenuEntry("Kampan");
            var singlePlayerMenuEntry = new MenuEntry("Hra jednoho hrace");
            var multiPlayerMenuEntry = new MenuEntry("Hra vice hracu");
            var optionsMenuEntry = new MenuEntry("Nastaveni");
            var exitMenuEntry = new MenuEntry("Konec");

            // Hook up menu event handlers.
            campaignMenuEntry.Selected += (sender, e) => LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                                                                            new GameplayScreen(GameMode.SinglePlayer));
            singlePlayerMenuEntry.Selected += (sender, e) => LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                                                                            new GameplayScreen(GameMode.SinglePlayer));
            multiPlayerMenuEntry.Selected += (sender, e) => ScreenManager.AddScreen(
                                                                            new MultiPlayerMenuScreen(), e.PlayerIndex);
            optionsMenuEntry.Selected += (sender, e) => ScreenManager.AddScreen(
                                                                            new OptionsMenuScreen(), e.PlayerIndex);
            exitMenuEntry.Selected += OnCancel;

            

            // Add entries to the menu.
            MenuEntries.Add(campaignMenuEntry);
            MenuEntries.Add(singlePlayerMenuEntry);
            MenuEntries.Add(multiPlayerMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);*/
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            backgroundTexture = content.Load<Texture2D>("images/menu/main-quit");

            var newGame = new ImageMenuEntry(new Rectangle(296, 17, 220, 68),
                                             new Rectangle(542, 17, 307, 68),
                                             content.Load<Texture2D>("images/menu/polozky"),
                                             content.Load<Texture2D>("images/menu/polozky"));

            newGame.PositionOriginal = new Vector2(387, 292);
            newGame.PositionSelected = new Vector2(343, 292);

            var options = new ImageMenuEntry(new Rectangle(293, 93, 220, 68),
                                             new Rectangle(542, 93, 307, 68),
                                             content.Load<Texture2D>("images/menu/polozky"),
                                             content.Load<Texture2D>("images/menu/polozky"));

            options.PositionOriginal = new Vector2(402, 360);
            options.PositionSelected = new Vector2(359, 360);

            var quit = new ImageMenuEntry(new Rectangle(293, 180, 220, 68),
                                             new Rectangle(542, 180, 307, 68),
                                             content.Load<Texture2D>("images/menu/polozky"),
                                             content.Load<Texture2D>("images/menu/polozky"));

            quit.PositionOriginal = new Vector2(434, 433);
            quit.PositionSelected = new Vector2(390, 433);

            //newGame.Selected += (sender, e) => LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
            //                                                                new GameplayScreen(GameMode.SinglePlayer));

            newGame.Selected += (sender, e) => ScreenManager.AddScreen(new NewGameMenuScreen(), e.PlayerIndex);

            options.Selected += (sender, e) => ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
            quit.Selected += (sender, e) => ScreenManager.AddScreen(new QuitMenuScreen(), e.PlayerIndex);

            MenuEntries.Add(newGame);
            MenuEntries.Add(options);
            MenuEntries.Add(quit);

            base.LoadContent();
        }


        #endregion

        #region Handle Input
        
        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
            base.UnloadContent();

            content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, fullscreen,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public override void LoadContent()
        {
            backgroundSound = ScreenManager.AudioManager.Play3DSound("sound/menu_background", true, new StaticAudioEmitter());
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        public override void UnloadContent()
        {
            if (!backgroundSound.IsDisposed) backgroundSound.Stop(true);
        }
    }
}
