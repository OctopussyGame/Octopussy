using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Octopussy
{
    public class Player : Entity
    {
        private int hp = 10;
        private Texture2D hud;
        private GameplayScreen screen;
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;


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

        public Player(GameplayScreen screen, string modelName, string name, Boolean isUsingBumpMap = false) 
            : base(screen, modelName, isUsingBumpMap)
        {
            this.screen = screen;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            
            hud = screen.ScreenManager.Game.Content.Load<Texture2D>("images/HUD-novy");
            spriteBatch = new SpriteBatch(screen.ScreenManager.Game.GraphicsDevice);
            spriteFont = screen.ScreenManager.Game.Content.Load<SpriteFont>("fonts/hudFont");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (HP == 0)
            {
                // gameover
            }
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
                               new Rectangle(53, 60, 549, 60),
                               new Rectangle(53, 140, 549, 60),
                               new Rectangle(53, 220, 549, 60),
                               new Rectangle(53, 300, 549, 60),
                               new Rectangle(53, 380, 549, 60),
                               new Rectangle(53, 460, 549, 60),
                               new Rectangle(53, 540, 549, 60),
                               new Rectangle(53, 620, 549, 60),
                               new Rectangle(53, 700, 549, 60),
                               new Rectangle(53, 780, 549, 60),
                               new Rectangle(53, 860, 549, 60)
                           };

            var weapon = new Rectangle(844, 52, 110, 70);
            var bullets = new Rectangle(708, 51, 29, 70);

            spriteBatch.Begin();
            var locationHP = new Vector2(20, 20);
            var locationBullets = new Vector2(630, 18);
            var locationNumBullets = new Vector2(670, 29);
            var locationWeapon = new Vector2(770, 22);
            var origin = new Vector2(0, 0);

            spriteBatch.Draw(hud, locationHP, huds[HP], Color.White, 0, origin, 1, SpriteEffects.None, 0); // HP bar

            //spriteBatch.Draw(hud, locationBullets, bullets, Color.White, 0, origin, 1, SpriteEffects.None, 0); // Bullets
            //spriteBatch.DrawString(spriteFont, "55", locationNumBullets, Color.Black, 0, origin, 2, SpriteEffects.None, 0);

            spriteBatch.Draw(hud, locationWeapon, weapon, Color.White, 0, origin, 1, SpriteEffects.None, 0); // Weapon

            spriteBatch.End();
        }
    }
}
