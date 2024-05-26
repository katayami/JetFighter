using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JetFighter
{
    class Bullet
    {
        public static Texture2D Texture { get; set; }
        public Vector2 Position { get; private set; }
        public float Speed { get; private set; }
        public bool IsVisible { get; set; }

        public Bullet(Vector2 startPosition)
        {
            Position = startPosition;
            Speed = 10f; // скорость пули
            IsVisible = true;
        }

        public void Update()
        {
            Position -= new Vector2(0, Speed);
            if (Position.Y < 0)
            {
                IsVisible = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsVisible)
            {
                spriteBatch.Draw(Texture, Position, Color.White);
            }
        }

        public Rectangle BoundingBox()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
        }
    }
}
