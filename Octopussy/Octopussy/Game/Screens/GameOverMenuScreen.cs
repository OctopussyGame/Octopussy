#region File Description

//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion

#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Octopussy.Managers.ScreenManager;

#endregion

namespace Octopussy.Game.Screens
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    internal class GameOverMenuScreen : MenuScreen
    {
        private Texture2D backgroundTexture;
        private ContentManager content;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameOverMenuScreen()
            : base("GameOver")
        {
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            backgroundTexture = content.Load<Texture2D>("images/menu/main-quit");

            var ok = new ImageMenuEntry(new Rectangle(29, 95, 79, 68),
                                        new Rectangle(174, 95, 79, 68),
                                        content.Load<Texture2D>("images/menu/polozky"),
                                        content.Load<Texture2D>("images/menu/polozky"));

            ok.PositionOriginal = new Vector2(460, 350);
            ok.PositionSelected = new Vector2(460, 350);

            ok.Selected += (sender, e) => LoadingScreen.Load(ScreenManager, false, null, new MainMenuScreen());
       
            MenuEntries.Add(ok);

            base.LoadContent();
        }

        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
            base.UnloadContent();

            content.Unload();
        }

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
            var fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, fullscreen,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));


            SpriteFont font = ScreenManager.Font;
            var origin = new Vector2(0, font.LineSpacing / 2.0f);


            spriteBatch.DrawString(font, "Game is over", new Vector2(420, 304), Color.White, 0,
                                   origin, 1, SpriteEffects.None, 0);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }
}