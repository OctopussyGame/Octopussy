using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Octopussy.Game.Screens;
using Octopussy.Managers.PreferenceManager;
using Octopussy.Managers.ScreenManager;
using Octopussy.Utils;

namespace Octopussy.Game.Elements
{
    public class Player : Entity
    {
        private readonly string _name;
        private readonly int _player;
        private readonly GameplayScreen _screen;
        private GameTime _gameTime;
        private int _hp = 10;
        private Texture2D _hud;
        private PreferenceManager _pm;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
        private bool isOver;

        public Player(GameplayScreen screen, string modelName, string name, int player, Boolean isUsingBumpMap = false)
            : base(screen, modelName, isUsingBumpMap)
        {
            this._screen = screen;
            this._player = player;
            this._name = name;
        }

        public int HP
        {
            get { return _hp; }

            set
            {
                _hp = value;

                if (_hp > 10)
                    _hp = 10;
                else if (_hp < 0)
                    _hp = 0;
            }
        }

        public void OnShot()
        {
            HP--;
        }

        public void OnSeaFlower()
        {
            HP++;
        }

        public void OnUrchin()
        {
            HP--;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            _hud = _screen.ScreenManager.Game.Content.Load<Texture2D>("images/HUD-novy");
            _spriteBatch = new SpriteBatch(_screen.ScreenManager.Game.GraphicsDevice);
            _spriteFont = _screen.ScreenManager.Game.Content.Load<SpriteFont>("fonts/hudFont");

            _pm = ((MainGame) _screen.ScreenManager.Game).PreferenceManager;
        }

        public override void Update(GameTime gameTime, HeightMapInfo heightMapInfo)
        {
            base.Update(gameTime, heightMapInfo);

            if (HP == 0 && !isOver)
            {
                isOver = true;
                _screen.ScreenManager.AddScreen(new GameOverMenuScreen(), PlayerIndex.One);
            }

            this._gameTime = gameTime;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            DrawHUD(gameTime);
        }

// ReSharper disable UnusedParameter.Local
        private void DrawHUD(GameTime time)
// ReSharper restore UnusedParameter.Local
        {
            var huds = new[]
                           {
                               new Rectangle(17, 558, 372, 40),
                               new Rectangle(17, 504, 372, 40),
                               new Rectangle(17, 450, 372, 40),
                               new Rectangle(17, 396, 372, 40),
                               new Rectangle(17, 341, 372, 40),
                               new Rectangle(17, 287, 372, 40),
                               new Rectangle(17, 233, 372, 40),
                               new Rectangle(17, 179, 372, 40),
                               new Rectangle(17, 125, 372, 40),
                               new Rectangle(17, 71, 372, 40),
                               new Rectangle(17, 17, 372, 40)
                           };

            var weapon = new Rectangle(201, 632, 238, 59);
            //var bullets = new Rectangle(708, 51, 29, 70);

            int offset = 0;

            if (_player == 2)
                offset = 600;

            _spriteBatch.Begin();
            var locationHP = new Vector2(20 + offset, 50);
            //var locationBullets = new Vector2(630 + offset, 18);
            //var locationNumBullets = new Vector2(670 + offset, 29);
            var locationName = new Vector2(30 + offset, 20);
            var locationWeapon = new Vector2(20 + offset, 110);

            var origin = new Vector2(0, 0);

            _spriteBatch.Draw(_hud, locationHP, huds[HP], Color.White, 0, origin, 1, SpriteEffects.None, 0); // HP bar

            //spriteBatch.Draw(hud, locationBullets, bullets, Color.White, 0, origin, 1, SpriteEffects.None, 0); // Bullets

            _spriteBatch.DrawString(_spriteFont, _name, locationName, Color.Black, 0, origin, 1, SpriteEffects.None, 0);

            _spriteBatch.Draw(_hud, locationWeapon, weapon, Color.White, 0, origin, 1, SpriteEffects.None, 0); // Weapon

            _spriteBatch.End();
        }

        public override void HandleInput(KeyboardState lastKeyboardState, GamePadState lastGamePadState,
                                         KeyboardState currentKeyboardState, GamePadState currentGamePadState)
        {
            GameTime gameTime = this._gameTime; // this needs rewrite

            if (_player == 1)
            {
                PlayerPreference playerPreference = _pm.PlayerOne;
                // Check for input to rotate left and right.
                if (currentKeyboardState.IsKeyDown(playerPreference.Left))
                {
                    TurnLeft(gameTime);
                }
                if (currentKeyboardState.IsKeyDown(playerPreference.Right))
                {
                    TurnRight(gameTime);
                }

                // Check for input to adjust speed.
                if (currentKeyboardState.IsKeyDown(playerPreference.Forward))
                {
                    Accellerate(gameTime);
                }

                if (currentKeyboardState.IsKeyDown(playerPreference.Backward))
                {
                    Decellerate(gameTime);
                }

                if ((currentKeyboardState.IsKeyDown(playerPreference.Shoot) &&
                     lastKeyboardState.IsKeyUp(playerPreference.Shoot)))
                {
                    Shoot();
                }
            }

            if (_player == 2)
            {
                PlayerPreference playerPreference = _pm.PlayerTwo;
                // Check for input to rotate left and right.
                if (currentKeyboardState.IsKeyDown(playerPreference.Left))
                {
                    TurnLeft(gameTime);
                }
                if (currentKeyboardState.IsKeyDown(playerPreference.Right))
                {
                    TurnRight(gameTime);
                }

                // Check for input to adjust speed.
                if (currentKeyboardState.IsKeyDown(playerPreference.Forward))
                {
                    Accellerate(gameTime);
                }

                if (currentKeyboardState.IsKeyDown(playerPreference.Backward))
                {
                    Decellerate(gameTime);
                }

                if ((currentKeyboardState.IsKeyDown(playerPreference.Shoot) &&
                     lastKeyboardState.IsKeyUp(playerPreference.Shoot)))
                {
                    Shoot();
                }
            }
        }
    }
}