using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JetFighter
{
    class Enemy
    {
        public static List<Texture2D> Textures { get; set; } = new List<Texture2D>();
        public Vector2 Position { get; private set; }
        private float speed;
        private int textureIndex;
        private Random rand;
        public bool IsVisible { get; set; }
        public int Health { get; private set; }

        public Enemy(Vector2 startPosition, int textureIndex, Random rand)
        {
            Position = startPosition;
            this.textureIndex = textureIndex;
            this.rand = rand;
            speed = 1.5f; // скорость врагов
            IsVisible = true;
            Health = textureIndex + 1;
        }

        public void Update()
        {
            if (IsVisible)
            {
                Position += new Vector2(0, speed);
                if (Position.Y > Game1.ScreenHeight)
                {
                    ResetPosition();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsVisible)
            {
                spriteBatch.Draw(Textures[textureIndex], Position, Color.White);
            }
        }

        public Rectangle BoundingBox()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, Textures[textureIndex].Width, Textures[textureIndex].Height);
        }

        public void ResetPosition()
        {
            Position = new Vector2(rand.Next(0, Game1.ScreenWidth - Textures[textureIndex].Width), -Textures[textureIndex].Height);
            IsVisible = true;
            Health = textureIndex + 1; // Сбрасываем здоровье врага
        }

        public void TakeDamage()
        {
            Health--;
            if (Health <= 0)
            {
                IsVisible = false;
                ResetPosition();
            }
        }
    }


}
