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
    class NewGameMenuScreen : MenuScreen
    {
        ContentManager content;
        Texture2D backgroundTexture;
        private bool shiftKey;
        private string playerOneName = "Player 1";
        private string playerTwoName = "Player 2";
        private string textInput;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public NewGameMenuScreen()
            : base("New Game")
        {
            textInput = playerOneName;
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            backgroundTexture = content.Load<Texture2D>("images/menu/newgame");

            var back = new ImageMenuEntry(new Rectangle(23, 25, 78, 54),
                                             new Rectangle(170, 25, 78, 54),
                                             content.Load<Texture2D>("images/menu/polozky"),
                                             content.Load<Texture2D>("images/menu/polozky"));

            back.PositionOriginal = new Vector2(240, 662);
            back.PositionSelected = new Vector2(240, 662);
            
            back.Selected += OnCancel;
            
            var ok = new ImageMenuEntry(new Rectangle(29, 95, 79, 68),
                                             new Rectangle(174, 95, 79, 68),
                                             content.Load<Texture2D>("images/menu/polozky"),
                                             content.Load<Texture2D>("images/menu/polozky"));

            ok.PositionOriginal = new Vector2(685, 656);
            ok.PositionSelected = new Vector2(685, 656);

            ok.Selected += OnCancel;

            MenuEntries.Add(back);
            MenuEntries.Add(ok);

            SelectedEntry = -2;

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

            
            spriteBatch.DrawString(font, playerOneName, new Vector2(474, 303), SelectedEntry == -2 ? Color.Teal : Color.White, 0,
                                   origin, 1f, SpriteEffects.None, 0);

            spriteBatch.DrawString(font, playerTwoName, new Vector2(474, 367), SelectedEntry == -1 ? Color.Teal : Color.White, 0,
                                   origin, 1f, SpriteEffects.None, 0);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public override void HandleInput(InputState input)
        {
            PlayerIndex playerIndex = PlayerIndex.One;

            if (true) {
                if (SelectedEntry == 0 && input.IsNewKeyPress(Keys.Enter, ControllingPlayer, out playerIndex))
                {
                    ExitScreen();
                    return;
                }

                if (SelectedEntry == 1 && input.IsNewKeyPress(Keys.Enter, ControllingPlayer, out playerIndex)) {
                    LoadingScreen.Load(ScreenManager, true, playerIndex, new GameplayScreen(GameMode.SinglePlayer, playerOneName, playerTwoName));
                    return;
                }

                // Move to the previous menu entry?
                if (input.IsMenuUp(ControllingPlayer)) {
                    SelectedEntry--;

                    if (SelectedEntry < -2)
                        SelectedEntry = 1;

                    if (SelectedEntry == -2) {
                        textInput = playerOneName;
                    }

                    if (SelectedEntry == -1) {
                        textInput = playerTwoName;
                    }
                }

                // Move to the next menu entry?
                if (input.IsMenuDown(ControllingPlayer)) {
                    SelectedEntry++;

                    if (SelectedEntry > 1)
                        SelectedEntry = -2;

                    if (SelectedEntry == -2) {
                        textInput = playerOneName;
                    }

                    if (SelectedEntry == -1) {
                        textInput = playerTwoName;
                    }
                }

                /*if (textInput.Length > 9) {
                    return;
                }*/
                
                var i = (int)playerIndex;
                var selected = false;
                var pressed = string.Empty;
                foreach (var key in input.CurrentKeyboardStates[i].GetPressedKeys()) {
                    if (input.LastKeyboardStates[i].IsKeyUp(key) && !selected) {
                        var keyNum = (int)key;
                        if (key == Keys.Back && textInput.Length > 0) {
                            textInput = textInput.Substring(0, textInput.Length - 1);
                            return;
                        }
                        if (keyNum >= (int)Keys.A && keyNum <= (int)Keys.Z) {
                            selected = true;
                            pressed = key.ToString();
                        } else if (keyNum >= (int)Keys.D0 && keyNum <= (int)Keys.D9) {
                            selected = true;
                            pressed = key.ToString().Substring(1, 1);
                        }
                    }
                    if ((key == Keys.RightShift || key == Keys.LeftShift)) {
                        shiftKey = true;
                    } else {
                        shiftKey = false;
                    }
                }

                if (!string.IsNullOrEmpty(pressed)) {
                    if (!shiftKey) {
                        textInput += pressed.ToLower();
                    } else {
                        textInput += pressed.ToUpper();
                    }
                }

                if (SelectedEntry == -2) {
                    playerOneName = textInput;
                }

                if (SelectedEntry == -1) {
                    playerTwoName = textInput;
                }

            } else if (input.IsMenuCancel(ControllingPlayer, out playerIndex)) {
                return;
            } else {
                base.HandleInput(input);
            }
        }

        #endregion
    }
}
