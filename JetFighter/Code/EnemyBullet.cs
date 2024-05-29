using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JetFighter.Code
{
    public class EnemyBullet
    {
        public static Texture2D Texture { get; set; }
        public Vector2 Position { get; private set; }
        private float speed;
        public bool IsVisible { get; set; }
        public Rectangle BoundingBox => new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

        public EnemyBullet(Vector2 startPosition)
        {
            Position = startPosition;
            speed = 5f; // Adjust bullet speed as necessary
            IsVisible = true;
        }

        public void Update()
        {
            Position += new Vector2(0, speed);
            if (Position.Y > Game1.ScreenHeight) IsVisible = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
