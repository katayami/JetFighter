using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace JetFighter
{
    public class Airplane
    {
        public static Texture2D Texture { get; set; }
        public Vector2 Position { get; private set; }
        private Vector2 direction;
        private float speed;
        private List<Bullet> bullets;
        private float shootTimer;
        private float shootCooldown = 0.25f;

        public Airplane(Vector2 startPosition)
        {
            Position = startPosition;
            speed = 4f;
            bullets = new List<Bullet>();
            shootTimer = 0f;
        }

        public void Update(GameTime gameTime, List<Enemy> enemies)
        {
            direction = Vector2.Zero;

            if (Keyboard.GetState().IsKeyDown(Keys.A) || Keyboard.GetState().IsKeyDown(Keys.Left))
                direction.X = -1;
            if (Keyboard.GetState().IsKeyDown(Keys.D) || Keyboard.GetState().IsKeyDown(Keys.Right))
                direction.X = 1;
            if (Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.Up))
                direction.Y = -1;
            if (Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.Down))
                direction.Y = 1;

            Position += direction * speed;

            if (Position.X < 0) Position = new Vector2(0, Position.Y);
            if (Position.X > Clouds.Width - Texture.Width) Position = new Vector2(Clouds.Width - Texture.Width, Position.Y);
            if (Position.Y < 0) Position = new Vector2(Position.X, 0);
            if (Position.Y > Clouds.Height - Texture.Height) Position = new Vector2(Position.X, Clouds.Height - Texture.Height);

            shootTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && shootTimer >= shootCooldown)
            {
                Shoot();
                shootTimer = 0f;
            }

            foreach (var bullet in bullets)
            {
                bullet.Update();
                foreach (var enemy in enemies)
                {
                    if (enemy.IsVisible && bullet.BoundingBox.Intersects(new Rectangle((int)enemy.Position.X, (int)enemy.Position.Y, Enemy.Textures[enemy.EnemyType].Width, Enemy.Textures[enemy.EnemyType].Height)))
                    {
                        enemy.TakeDamage(1);
                        bullet.IsVisible = false;
                    }
                }
            }

            bullets.RemoveAll(b => !b.IsVisible);
        }

        private void Shoot()
        {
            var bulletPosition = new Vector2(Position.X + Texture.Width / 2 - Bullet.Texture.Width / 2, Position.Y);
            bullets.Add(new Bullet(bulletPosition));
            Game1.ShootSound.Play();
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
