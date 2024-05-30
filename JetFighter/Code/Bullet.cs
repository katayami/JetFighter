using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JetFighter
{
    public class Bullet
    {
        public static Texture2D Texture { get; set; }
        public Vector2 Position { get; private set; }
        private float speed;
        public bool IsVisible { get; set; }
        public Rectangle BoundingBox => new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

        public Bullet(Vector2 startPosition)
        {
            Position = startPosition;
            speed = 10f;
            IsVisible = true;
        }

        public void Update()
        {
            Position += new Vector2(0, -speed);
            if (Position.Y < 0) IsVisible = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
