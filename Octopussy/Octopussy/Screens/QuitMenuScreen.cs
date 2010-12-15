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
using Microsoft.Xna.Framework.Input;

#endregion

namespace Octopussy
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class QuitMenuScreen : MenuScreen
    {
        ContentManager content;
        Texture2D backgroundTexture;
        private bool shiftKey;
        private string playerOneName = "Janicka";
        private string playerTwoName = "Tkanicka";
        private string textInput;
        private bool inGame;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public QuitMenuScreen(bool inGame = false)
            : base("Quit")
        {
            this.inGame = inGame;
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            backgroundTexture = content.Load<Texture2D>("images/menu/main-quit");

            var yes = new ImageMenuEntry(new Rectangle(28, 255, 94, 68),
                                             new Rectangle(178, 255, 94, 68),
                                             content.Load<Texture2D>("images/menu/polozky"),
                                             content.Load<Texture2D>("images/menu/polozky"));

            yes.PositionOriginal = new Vector2(507, 367);
            yes.PositionSelected = new Vector2(507, 367);

            yes.Selected += (e, sender) =>
                                {
                                    if (!inGame)
                                        ScreenManager.Game.Exit();
                                    else
                                        LoadingScreen.Load(ScreenManager, false, null, new MainMenuScreen());
                                };

            var no = new ImageMenuEntry(new Rectangle(27, 178, 79, 68),
                                             new Rectangle(177, 178, 79, 68),
                                             content.Load<Texture2D>("images/menu/polozky"),
                                             content.Load<Texture2D>("images/menu/polozky"));

            no.PositionOriginal = new Vector2(408, 367);
            no.PositionSelected = new Vector2(408, 367);

            no.Selected += OnCancel;


            var back = new ImageMenuEntry(new Rectangle(23, 25, 78, 54),
                                             new Rectangle(170, 25, 78, 54),
                                             content.Load<Texture2D>("images/menu/polozky"),
                                             content.Load<Texture2D>("images/menu/polozky"));

            back.PositionOriginal = new Vector2(240, 662);
            back.PositionSelected = new Vector2(240, 662);
            
            back.Selected += OnCancel;

            MenuEntries.Add(no);
            MenuEntries.Add(yes);
            MenuEntries.Add(back);

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
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, fullscreen,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            

            SpriteFont font = ScreenManager.Font;
            var origin = new Vector2(0, font.LineSpacing / 2.0f);

            
            spriteBatch.DrawString(font, "Do you want to quit?", new Vector2(361, 304), Color.White, 0,
                                   origin, 1f, SpriteEffects.None, 0);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }
}
