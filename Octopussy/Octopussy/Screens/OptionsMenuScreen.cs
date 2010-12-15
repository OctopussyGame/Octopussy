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
    class OptionsMenuScreen : MenuScreen
    {
        ContentManager content;
        Texture2D backgroundTexture;
        string textInput;
        PreferenceManager pm;
        PreferenceManager previousPm;
        bool waiting = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Settings")
        {
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            backgroundTexture = content.Load<Texture2D>("images/menu/settings");

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

            SelectedEntry = -10;

            pm = ((MainGame)ScreenManager.Game).PreferenceManager;
            previousPm = pm;
            pm = (PreferenceManager) pm.Clone();

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

            

            spriteBatch.DrawString(font, pm.PlayerOne.Forward.ToString(), new Vector2(453, 302), SelectedEntry == -10 ? Color.Teal : Color.White, 0,
                                   origin, 1f, SpriteEffects.None, 0);

            spriteBatch.DrawString(font, pm.PlayerTwo.Forward.ToString(), new Vector2(574, 302), SelectedEntry == -9 ? Color.Teal : Color.White, 0,
                                   origin, 1f, SpriteEffects.None, 0);

            spriteBatch.DrawString(font, pm.PlayerOne.Backward.ToString(), new Vector2(453, 358), SelectedEntry == -8 ? Color.Teal : Color.White, 0,
                                   origin, 1f, SpriteEffects.None, 0);

            spriteBatch.DrawString(font, pm.PlayerTwo.Backward.ToString(), new Vector2(574, 358), SelectedEntry == -7 ? Color.Teal : Color.White, 0,
                                   origin, 1f, SpriteEffects.None, 0);

            spriteBatch.DrawString(font, pm.PlayerOne.Left.ToString(), new Vector2(453, 417), SelectedEntry == -6 ? Color.Teal : Color.White, 0,
                                   origin, 1f, SpriteEffects.None, 0);

            spriteBatch.DrawString(font, pm.PlayerTwo.Left.ToString(), new Vector2(574, 417), SelectedEntry == -5 ? Color.Teal : Color.White, 0,
                                   origin, 1f, SpriteEffects.None, 0);

            spriteBatch.DrawString(font, pm.PlayerOne.Right.ToString(), new Vector2(453, 480), SelectedEntry == -4 ? Color.Teal : Color.White, 0,
                                   origin, 1f, SpriteEffects.None, 0);

            spriteBatch.DrawString(font, pm.PlayerTwo.Right.ToString(), new Vector2(574, 480), SelectedEntry == -3 ? Color.Teal : Color.White, 0,
                                   origin, 1f, SpriteEffects.None, 0);

            spriteBatch.DrawString(font, pm.PlayerOne.Shoot.ToString(), new Vector2(453, 555), SelectedEntry == -2 ? Color.Teal : Color.White, 0,
                                   origin, 1f, SpriteEffects.None, 0);

            spriteBatch.DrawString(font, pm.PlayerTwo.Shoot.ToString(), new Vector2(574, 555), SelectedEntry == -1 ? Color.Teal : Color.White, 0,
                                   origin, 1f, SpriteEffects.None, 0);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public override void HandleInput(InputState input)
        {
            PlayerIndex playerIndex = PlayerIndex.One;
            if (true)
            {
                if (waiting)
                {
                    foreach (var key in input.CurrentKeyboardStates[0].GetPressedKeys())
                    {
                        if (input.LastKeyboardStates[0].IsKeyUp(key))
                        {
                            switch (SelectedEntry) {
                                case -10:
                                    pm.PlayerOne.Forward = key;
                                    break;
                                case -9:
                                    pm.PlayerTwo.Forward = key;
                                    break;
                                case -8:
                                    pm.PlayerOne.Backward = key;
                                    break;
                                case -7:
                                    pm.PlayerTwo.Backward = key;
                                    break;
                                case -6:
                                    pm.PlayerOne.Left = key;
                                    break;
                                case -5:
                                    pm.PlayerTwo.Left = key;
                                    break;

                                case -4:
                                    pm.PlayerOne.Right = key;
                                    break;

                                case -3:
                                    pm.PlayerTwo.Right = key;
                                    break;

                                case -2:
                                    pm.PlayerOne.Shoot = key;
                                    break;

                                case -1:
                                    pm.PlayerTwo.Shoot = key;
                                    break;

                                default:
                                    break;
                            }

                            waiting = false;
                            return;
                        }
                    }
                }

                if (SelectedEntry == 0 && input.IsNewKeyPress(Keys.Enter, ControllingPlayer, out playerIndex))
                {
                    ExitScreen();
                    ((MainGame)ScreenManager.Game).PreferenceManager = previousPm;
                    return;
                }

                if (SelectedEntry == 1 && input.IsNewKeyPress(Keys.Enter, ControllingPlayer, out playerIndex)) {
                    ExitScreen();
                    ((MainGame)ScreenManager.Game).PreferenceManager = pm;
                    return;
                }

                if (SelectedEntry >= -10 && SelectedEntry < 0 && input.IsNewKeyPress(Keys.Enter, ControllingPlayer, out playerIndex))
                {
                    switch (SelectedEntry)
                    {
                        case -10:
                            if (!waiting)
                            {
                                pm.PlayerOne.Forward = Keys.None;
                                waiting = true;
                            }
                            break;
                        case -9:
                            if (!waiting) {
                                pm.PlayerTwo.Forward = Keys.None;
                                waiting = true;
                            }
                            break;
                        case -8:
                            if (!waiting) {
                                pm.PlayerOne.Backward = Keys.None;
                                waiting = true;
                            }
                            break;
                        case -7:
                            if (!waiting) {
                                pm.PlayerTwo.Backward = Keys.None;
                                waiting = true;
                            }
                            break;
                        case -6:
                            if (!waiting) {
                                pm.PlayerOne.Left = Keys.None;
                                waiting = true;
                            }
                            break;
                        case -5:
                            if (!waiting) {
                                pm.PlayerTwo.Left = Keys.None;
                                waiting = true;
                            }
                            break;

                        case -4:
                            if (!waiting) {
                                pm.PlayerOne.Right = Keys.None;
                                waiting = true;
                            }
                            break;

                        case -3:
                            if (!waiting) {
                                pm.PlayerTwo.Right = Keys.None;
                                waiting = true;
                            }
                            break;

                        case -2:
                            if (!waiting) {
                                pm.PlayerOne.Shoot = Keys.None;
                                waiting = true;
                            }
                            break;

                        case -1:
                            if (!waiting) {
                                pm.PlayerTwo.Shoot = Keys.None;
                                waiting = true;
                            }
                            break;

                        default:
                            break;
                    }
                    return;
                }

                // Move to the previous menu entry?
                if (input.IsMenuUp(ControllingPlayer))
                {
                    SelectedEntry--;

                    if (SelectedEntry < -10)
                        SelectedEntry = 1;

                    if (SelectedEntry == -2)
                    {
                        //    textInput = playerOneName;
                    }

                    if (SelectedEntry == -1)
                    {
                        //  textInput = playerTwoName;
                    }
                }

                // Move to the next menu entry?
                if (input.IsMenuDown(ControllingPlayer))
                {
                    SelectedEntry++;

                    if (SelectedEntry > 1)
                        SelectedEntry = -10;

                    if (SelectedEntry == -2)
                    {
                        // textInput = playerOneName;
                    }

                    if (SelectedEntry == -1)
                    {
                        //  textInput = playerTwoName;
                    }
                }
            }

            if (SelectedEntry == -2)
            {
                //  playerOneName = textInput;
            }

            if (SelectedEntry == -1)
            {
                //   playerTwoName = textInput;
            }
        }

        #endregion
    }
}
