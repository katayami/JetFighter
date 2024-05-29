using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JetFighter
{
    static class SplashScreen
    {
        public static Texture2D Background { get; set; }
        static int TimeCounter = 0;
        static Color color;
        public static SpriteFont Font { get; set; }
        static Vector2 textPosition = new Vector2(100, 500);

        static public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Background, Vector2.Zero, Color.White);
            spriteBatch.DrawString(Font, "Press Space button to start", textPosition, color);
        }

        static public void Update()
        {
            color = Color.FromNonPremultiplied(255, 255, 255, TimeCounter % 256);
            TimeCounter++;
        }

    }
}
