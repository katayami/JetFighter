using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace JetFighter
{
    class Airplane
    {
        public static Texture2D Texture { get; set; }
        public Vector2 Position { get; private set; }
        private Vector2 direction;
        private float speed;
        private List<Bullet> bullets;
        private float bulletDelay;

        public Airplane(Vector2 startPosition)
        {
            Position = startPosition;
            speed = 5f;
            bullets = new List<Bullet>();
            bulletDelay = 0;
        }

        public void Update(GameTime gameTime, List<Enemy> enemies)
        {
            direction = Vector2.Zero;

            if (Keyboard.GetState().IsKeyDown(Keys.A) || Keyboard.GetState().IsKeyDown(Keys.Left))
                direction.X = -1;
            if (Keyboard.GetState().IsKeyDown(Keys.D) || Keyboard.GetState().IsKeyDown(Keys.Right))
                direction.X = 1;

            Position += direction * speed;

            if (Position.X < 0) Position = new Vector2(0, Position.Y);
            if (Position.X > Clouds.Width - Texture.Width) Position = new Vector2(Clouds.Width - Texture.Width, Position.Y);

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                Shoot(gameTime);
            }

            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Update();
                if (!bullets[i].IsVisible)
                {
                    bullets.RemoveAt(i);
                    i--;
                }
            }

            foreach (var bullet in bullets)
            {
                foreach (var enemy in enemies)
                {
                    if (bullet.IsVisible && enemy.IsVisible && bullet.BoundingBox().Intersects(enemy.BoundingBox()))
                    {
                        bullet.IsVisible = false;
                        enemy.TakeDamage();
                    }
                }
            }
        }

        private void Shoot(GameTime gameTime)
        {
            bulletDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (bulletDelay <= 0)
            {
                Vector2 bulletPosition = new Vector2(Position.X + Texture.Width / 2 - Bullet.Texture.Width / 2, Position.Y);
                bullets.Add(new Bullet(bulletPosition));
                bulletDelay = 0.25f; // скорость перезарядки
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
            foreach (var bullet in bullets)
            {
                bullet.Draw(spriteBatch);
            }
        }
    }
}
