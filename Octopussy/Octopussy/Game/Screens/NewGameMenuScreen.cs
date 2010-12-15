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
using Octopussy.Managers.ScreenManager;
using Octopussy.Utils;

#endregion

namespace Octopussy.Game.Screens
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    internal class NewGameMenuScreen : MenuScreen
    {
        private Texture2D _backgroundTexture;
        private ContentManager _content;
        private string _playerOneName = "Player 1";
        private string _playerTwoName = "Player 2";
        private bool _shiftKey;
        private string _textInput;

        /// <summary>
        /// Constructor.
        /// </summary>
        public NewGameMenuScreen()
            : base("New Game")
        {
            _textInput = _playerOneName;
        }

        public override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _backgroundTexture = _content.Load<Texture2D>("images/menu/newgame");

            var back = new ImageMenuEntry(new Rectangle(23, 25, 78, 54),
                                          new Rectangle(170, 25, 78, 54),
                                          _content.Load<Texture2D>("images/menu/polozky"),
                                          _content.Load<Texture2D>("images/menu/polozky"));

            back.PositionOriginal = new Vector2(240, 662);
            back.PositionSelected = new Vector2(240, 662);

            back.Selected += OnCancel;

            var ok = new ImageMenuEntry(new Rectangle(29, 95, 79, 68),
                                        new Rectangle(174, 95, 79, 68),
                                        _content.Load<Texture2D>("images/menu/polozky"),
                                        _content.Load<Texture2D>("images/menu/polozky"));

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

            _content.Unload();
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

            spriteBatch.Draw(_backgroundTexture, fullscreen,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));


            SpriteFont font = ScreenManager.Font;
            var origin = new Vector2(0, font.LineSpacing/2.0f);


            spriteBatch.DrawString(font, _playerOneName, new Vector2(474, 303),
                                   SelectedEntry == -2 ? Color.Teal : Color.White, 0,
                                   origin, 1f, SpriteEffects.None, 0);

            spriteBatch.DrawString(font, _playerTwoName, new Vector2(474, 367),
                                   SelectedEntry == -1 ? Color.Teal : Color.White, 0,
                                   origin, 1f, SpriteEffects.None, 0);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public override void HandleInput(InputState input)
        {
            PlayerIndex playerIndex = PlayerIndex.One;

            if (true)
            {
                if (SelectedEntry == 0 && input.IsNewKeyPress(Keys.Enter, ControllingPlayer, out playerIndex))
                {
                    ExitScreen();
                    return;
                }

                if (SelectedEntry == 1 && input.IsNewKeyPress(Keys.Enter, ControllingPlayer, out playerIndex))
                {
                    LoadingScreen.Load(ScreenManager, true, playerIndex,
                                       new GameplayScreen(GameMode.SinglePlayer, _playerOneName, _playerTwoName));
                    return;
                }

                // Move to the previous menu entry?
                if (input.IsMenuUp(ControllingPlayer))
                {
                    SelectedEntry--;

                    if (SelectedEntry < -2)
                        SelectedEntry = 1;

                    if (SelectedEntry == -2)
                    {
                        _textInput = _playerOneName;
                    }

                    if (SelectedEntry == -1)
                    {
                        _textInput = _playerTwoName;
                    }
                }

                // Move to the next menu entry?
                if (input.IsMenuDown(ControllingPlayer))
                {
                    SelectedEntry++;

                    if (SelectedEntry > 1)
                        SelectedEntry = -2;

                    if (SelectedEntry == -2)
                    {
                        _textInput = _playerOneName;
                    }

                    if (SelectedEntry == -1)
                    {
                        _textInput = _playerTwoName;
                    }
                }

                /*if (textInput.Length > 9) {
                    return;
                }*/

                var i = (int) playerIndex;
                bool selected = false;
                string pressed = string.Empty;
                foreach (Keys key in input.CurrentKeyboardStates[i].GetPressedKeys())
                {
                    if (input.LastKeyboardStates[i].IsKeyUp(key) && !selected)
                    {
                        var keyNum = (int) key;
                        if (key == Keys.Back && _textInput.Length > 0)
                        {
                            _textInput = _textInput.Substring(0, _textInput.Length - 1);
                            return;
                        }
                        if (keyNum >= (int) Keys.A && keyNum <= (int) Keys.Z)
                        {
                            selected = true;
                            pressed = key.ToString();
                        }
                        else if (keyNum >= (int) Keys.D0 && keyNum <= (int) Keys.D9)
                        {
                            selected = true;
                            pressed = key.ToString().Substring(1, 1);
                        }
                    }
                    if ((key == Keys.RightShift || key == Keys.LeftShift))
                    {
                        _shiftKey = true;
                    }
                    else
                    {
                        _shiftKey = false;
                    }
                }

                if (!string.IsNullOrEmpty(pressed))
                {
                    if (!_shiftKey)
                    {
                        _textInput += pressed.ToLower();
                    }
                    else
                    {
                        _textInput += pressed.ToUpper();
                    }
                }

                if (SelectedEntry == -2)
                {
                    _playerOneName = _textInput;
                }

                if (SelectedEntry == -1)
                {
                    _playerTwoName = _textInput;
                }
            }
            /*else if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
            {
                return;
            }
            else
            {
                base.HandleInput(input);
            }*/
        }

        #endregion
    }
}