using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;



namespace JetFighter
{
    enum Stat
    {
        SplashScreen,
        Game,
        Final,
        Pause
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Airplane airplane;
        private List<Enemy> enemies;
        public static int ScreenWidth;
        public static int ScreenHeight;
        Stat Stat = Stat.SplashScreen;
        private Random rand;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            rand = new Random();
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 840;
            _graphics.PreferredBackBufferHeight = 650;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            ScreenWidth = _graphics.PreferredBackBufferWidth;
            ScreenHeight = _graphics.PreferredBackBufferHeight;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            SplashScreen.Background = Content.Load<Texture2D>("background");
            SplashScreen.Font = Content.Load<SpriteFont>("SplashFont");
            Clouds.Init(_spriteBatch, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, 10);
            Cloud.Texture2D = Content.Load<Texture2D>("cloud");

            Texture2D airplaneTexture = Content.Load<Texture2D>("Jet");
            Vector2 airplaneStartPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight - airplaneTexture.Height);
            airplane = new Airplane(airplaneStartPosition);
            Airplane.Texture = airplaneTexture;

            Bullet.Texture = Content.Load<Texture2D>("Bullet");

            Enemy.Textures.Add(Content.Load<Texture2D>("Enemy1"));
            Enemy.Textures.Add(Content.Load<Texture2D>("Enemy2"));
            Enemy.Textures.Add(Content.Load<Texture2D>("Enemy3"));
            Enemy.Textures.Add(Content.Load<Texture2D>("Enemy4"));

            enemies = new List<Enemy>();
            for (int i = 0; i < 4; i++)
            {
                Vector2 enemyStartPosition = new Vector2(rand.Next(0, ScreenWidth - Enemy.Textures[i].Width), rand.Next(-ScreenHeight, 0));
                enemies.Add(new Enemy(enemyStartPosition, i, rand));
            }
        }

        protected override void Update(GameTime gameTime)
        {
            switch (Stat)
            {
                case Stat.SplashScreen:
                    SplashScreen.Update();
                    if (Keyboard.GetState().IsKeyDown(Keys.Space)) Stat = Stat.Game;
                    break;
                case Stat.Game:
                    Clouds.Update();
                    airplane.Update(gameTime, enemies);
                    foreach (var enemy in enemies)
                    {
                        enemy.Update();
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Stat = Stat.SplashScreen;
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.RoyalBlue);
            _spriteBatch.Begin();
            switch (Stat)
            {
                case Stat.SplashScreen:
                    SplashScreen.Draw(_spriteBatch);
                    break;
                case Stat.Game:
                    Clouds.Draw();
                    airplane.Draw(_spriteBatch);
                    foreach (var enemy in enemies)
                    {
                        if (enemy.IsVisible)
                        {
                            enemy.Draw(_spriteBatch);
                        }
                    }
                    break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
