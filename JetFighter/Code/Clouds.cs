using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace JetFighter
{
    class Clouds
    {
        public static int Width, Height;
        public static Random rnd = new Random();
        static public SpriteBatch SpriteBatch { get; set; }
        static Cloud[] clouds;

        static public int GetIntRnd(int min, int max)
        {
            return rnd.Next(min, max);
        }

        static public void Init(SpriteBatch spriteBatch, int width, int height, int numClouds)
        {
            Clouds.Width = width;
            Clouds.Height = height;
            Clouds.SpriteBatch = spriteBatch;
            clouds = new Cloud[numClouds];
            for (int i = 0; i < clouds.Length; i++)
                clouds[i] = new Cloud(new Vector2(0, rnd.Next(1, 5)));
        }

        static public void Draw()
        {
            foreach (Cloud cloud in clouds)
                cloud.Draw();
        }

        static public void Update()
        {
            foreach (Cloud cloud in clouds)
                cloud.Update();
        }
    }

    class Cloud
    {
        Vector2 Pos;
        Vector2 Dir;
        Color color;

        public static Texture2D Texture2D { get; set; }

        public Cloud(Vector2 Dir)
        {
            this.Dir = Dir;
            RandomSet();
        }

        public void Update()
        {
            Pos += Dir;
            if (Pos.Y > Clouds.Height)
            {
                RandomSet();
            }
        }

        public void RandomSet()
        {
            Pos = new Vector2(Clouds.GetIntRnd(0, Clouds.Width), Clouds.GetIntRnd(-300, 0));
            color = Color.White;
        }

        public void Draw()
        {
            Clouds.SpriteBatch.Draw(Texture2D, Pos, color);
        }
    }
}
