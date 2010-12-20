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
        private const float ActionInterval = 0.8f;
        private const float ShootInterval = 0.03f;
        private const float AutoShootInterval = 0.5f;

        private BoundingSphere _shootRadius;
        private readonly string _name;
        private readonly int _player;
        private readonly GameplayScreen _screen;
        private int _hp = 10;
        private GameTime _gameTime;
        private Texture2D _hud;
        private PreferenceManager _pm;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
        private bool _isOver;
        private TimeSpan actionDelay = TimeSpan.Zero;
        private TimeSpan shootDelay = TimeSpan.Zero;
        private TimeSpan autoShootDelay = TimeSpan.Zero;

        public Player(GameplayScreen screen, string modelName, string name, int player, Boolean isUsingBumpMap = false)
            : base(screen, modelName, isUsingBumpMap, false, true)
        {
            this._screen = screen;
            this._player = player;
            this._name = name;
            this._isOver = false;
            this.HeightOffset = 5;
            _shootRadius = new BoundingSphere(Position, 1000);
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

        public BoundingSphere ShootRadius
        {
            get { return _shootRadius; }
        }

        public void OnShot()
        {
            if (_player == 1) _screen.setBloomPreset("Blurry");
            _screen.ScreenManager.AudioManager.Play3DSound("sound/nesahej_na_me", false, this);
            HP--;
        }

        public void OnSeaFlower()
        {
            if (HP == 10) return;

            if (_player == 1) _screen.setBloomPreset("Subtle");
            _screen.ScreenManager.AudioManager.Play3DSound("sound/to_je_moje_chapadlo", false, this);
            HP++;
        }

        public void OnUrchin()
        {
            if (_player == 1) _screen.setBloomPreset("Blurry");
            _screen.ScreenManager.AudioManager.Play3DSound("sound/no_fuj", false, this);
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
            _shootRadius.Center = Position;

            autoShootDelay -= gameTime.ElapsedGameTime;
            if (_player == 2 && autoShootDelay < TimeSpan.Zero && _shootRadius.Intersects(_screen.PlayerOne.BoundingSphere))
            {
                Shoot(gameTime);
                autoShootDelay += TimeSpan.FromSeconds(AutoShootInterval);
            }
            

            if (HP == 0 && !_isOver)
            {
                _isOver = true;
                _screen.ScreenManager.AddScreen(new GameOverMenuScreen(), PlayerIndex.One);
            }

            this._gameTime = gameTime;
        }

        protected override void OnCollision(Entity entity, GameTime gameTime, HeightMapInfo heightMapInfo)
        {
            if (_isOver) return;

            if (entity.ModelName.Contains("egg") ||
                entity.ModelName.Contains("stone") ||
                entity.ModelName.Contains("seaGrassBig") ||
                entity.GetType() == typeof(Player))
            {
                while (InCollisionWith(entity))
                {
                    this.MoveBack(gameTime, heightMapInfo);
                }
            }

            actionDelay -= gameTime.ElapsedGameTime;
            if (actionDelay >= TimeSpan.Zero)
            {
                return;
            }

            if(entity.ModelName.Contains("urchin"))
            {
                this.OnUrchin();
            }
            else if (entity.ModelName.Contains("seaGrassCylinder"))
            {
                this.OnSeaFlower();
            }
            else if (entity.GetType() == typeof(Rocket) && ((Rocket)entity).Owner != this)
            {
                this.OnShot();
                ((Rocket)entity).Die();
            }

            actionDelay += TimeSpan.FromSeconds(ActionInterval);
        }

// ReSharper disable UnusedParameter.Local
        public void DrawHUD(GameTime time)
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

        // ReSharper disable MemberCanBeProtected.Global
        public void Shoot(GameTime gameTime)
        // ReSharper restore MemberCanBeProtected.Global
        {
            shootDelay -= gameTime.ElapsedGameTime;
            if (shootDelay >= TimeSpan.Zero)
            {
                return;
            }
            
            //float vx = ((float)Math.Cos(_rotation) * (ProjectileSpeed));
            //float vz = ((float)Math.Sin(_rotation) * (ProjectileSpeed));

            _screen.ScreenManager.AudioManager.Play3DSound("sound/tu_mas", false, this);
            _screen.AddRocket(this);

            /*_screen.AddProjectile(new Projectile(new Vector3(_position.X, _position.Y + 50, _position.Z), new Vector3(-vz, 0, -vx),
                                                _screen.explosionParticles,
                                                _screen.explosionSmokeParticles,
                                                _screen.projectileTrailParticles));*/

            shootDelay += TimeSpan.FromSeconds(ShootInterval);
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
                    Shoot(gameTime);
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
                    Shoot(gameTime);
                }
            }
        }
    }
}