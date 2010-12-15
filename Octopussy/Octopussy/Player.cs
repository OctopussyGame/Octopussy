using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Octopussy
{
    public class Player : Entity
    {
        private int hp = 10;
        private Texture2D hud;
        private GameplayScreen screen;
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;
        private int player;
        private GameTime gameTime;
        private PreferenceManager pm;
        private string name;

        public int HP
        {
            get
            {
                return hp;
            }

            set
            {
                hp = value;

                if (hp > 10)
                    hp = 10;
                else if (hp < 0)
                    hp = 0;
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

        public Player(GameplayScreen screen, string modelName, string name, int player, Boolean isUsingBumpMap = false) 
            : base(screen, modelName, isUsingBumpMap)
        {
            this.screen = screen;
            this.player = player;
            this.name = name;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            
            hud = screen.ScreenManager.Game.Content.Load<Texture2D>("images/HUD-novy");
            spriteBatch = new SpriteBatch(screen.ScreenManager.Game.GraphicsDevice);
            spriteFont = screen.ScreenManager.Game.Content.Load<SpriteFont>("fonts/hudFont");

            pm = ((MainGame) screen.ScreenManager.Game).PreferenceManager;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (HP == 0)
            {
                // gameover
            }

            this.gameTime = gameTime;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            DrawHUD(gameTime);
        }
        
        private void DrawHUD(GameTime time)
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

            var offset = 0;

            if (player == 2)
                offset = 600;

            spriteBatch.Begin();
            var locationHP = new Vector2(20 + offset, 50);
            //var locationBullets = new Vector2(630 + offset, 18);
            //var locationNumBullets = new Vector2(670 + offset, 29);
            var locationName = new Vector2(30 + offset, 20);
            var locationWeapon = new Vector2(20 + offset, 110);

            var origin = new Vector2(0, 0);
            
            spriteBatch.Draw(hud, locationHP, huds[HP], Color.White, 0, origin, 1, SpriteEffects.None, 0); // HP bar

            //spriteBatch.Draw(hud, locationBullets, bullets, Color.White, 0, origin, 1, SpriteEffects.None, 0); // Bullets

            spriteBatch.DrawString(spriteFont, name, locationName, Color.Black, 0, origin, 1, SpriteEffects.None, 0);

            spriteBatch.Draw(hud, locationWeapon, weapon, Color.White, 0, origin, 1, SpriteEffects.None, 0); // Weapon

            spriteBatch.End();
        }

        public override void HandleInput(KeyboardState lastKeyboardState, GamePadState lastGamePadState, KeyboardState currentKeyboardState, GamePadState currentGamePadState)
        {
            var gameTime = this.gameTime; // this needs rewrite

            if (player == 1)
            {
                var playerPreference = pm.PlayerOne;
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

            if (player == 2) {
                var playerPreference = pm.PlayerTwo;
                // Check for input to rotate left and right.
                if (currentKeyboardState.IsKeyDown(playerPreference.Left)) {
                    TurnLeft(gameTime);
                }
                if (currentKeyboardState.IsKeyDown(playerPreference.Right)) {
                    TurnRight(gameTime);
                }

                // Check for input to adjust speed.
                if (currentKeyboardState.IsKeyDown(playerPreference.Forward)) {
                    Accellerate(gameTime);
                }

                if (currentKeyboardState.IsKeyDown(playerPreference.Backward)) {
                    Decellerate(gameTime);
                }

                if ((currentKeyboardState.IsKeyDown(playerPreference.Shoot) &&
                     lastKeyboardState.IsKeyUp(playerPreference.Shoot))) {
                    Shoot();
                }
            }
        }
    }
}
