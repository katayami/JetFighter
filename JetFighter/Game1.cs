using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using JetFighter.Code;



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
        public static GameTime GameTime;
        Stat Stat = Stat.SplashScreen;
        private Random rand;
        private SpriteFont pauseFont;
        private KeyboardState previousKeyboardState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            rand = new Random();
            previousKeyboardState = Keyboard.GetState();
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

            // Initialize the airplane
            Texture2D airplaneTexture = Content.Load<Texture2D>("Jet");
            Vector2 airplaneStartPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight - airplaneTexture.Height);
            airplane = new Airplane(airplaneStartPosition);
            Airplane.Texture = airplaneTexture;

            // Load bullet textures
            Bullet.Texture = Content.Load<Texture2D>("Bullet");
            EnemyBullet.Texture = Content.Load<Texture2D>("EnemyBullet");

            // Load enemy textures
            Enemy.Textures.Add(Content.Load<Texture2D>("Enemy1"));
            Enemy.Textures.Add(Content.Load<Texture2D>("Enemy2"));
            Enemy.Textures.Add(Content.Load<Texture2D>("Enemy3"));
            Enemy.Textures.Add(Content.Load<Texture2D>("Enemy4"));

            // Load pause font
            pauseFont = Content.Load<SpriteFont>("PauseFont");

            // Initialize enemies
            enemies = new List<Enemy>();
            for (int i = 0; i < 4; i++)
            {
                Vector2 enemyStartPosition = new Vector2(rand.Next(0, ScreenWidth - Enemy.Textures[i].Width), rand.Next(-ScreenHeight, 0));
                var enemy = new Enemy(enemyStartPosition, i, rand);

                // Установите значения ShootCooldown для каждого типа врага
                switch (i)
                {
                    case 0:
                        enemy.ShootCooldown = float.PositiveInfinity; // Enemy1 не стреляет
                        break;
                    case 1:
                        enemy.ShootCooldown = 0.8f; // Настройка для Enemy2
                        break;
                    case 2:
                        enemy.ShootCooldown = 0.1f; // Настройка для Enemy3
                        enemy.maxShots = 10; // Быстро выпускает 10 снарядов
                        enemy.reloadTime = 3f; // Перезарядка в течение 3 секунд
                        break;
                    case 3:
                        enemy.ShootCooldown = 2f; // Настройка для Enemy4
                        break;
                }

                enemies.Add(enemy);
            }
        }




        protected override void Update(GameTime gameTime)
        {
            GameTime = gameTime;
            KeyboardState keyboardState = Keyboard.GetState();

            switch (Stat)
            {
                case Stat.SplashScreen:
                    SplashScreen.Update();
                    if (keyboardState.IsKeyDown(Keys.Space)) Stat = Stat.Game;
                    break;
                case Stat.Game:
                    if (keyboardState.IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyUp(Keys.Escape))
                    {
                        Stat = Stat.Pause;
                    }
                    Clouds.Update();
                    airplane.Update(gameTime, enemies);
                    foreach (var enemy in enemies)
                    {
                        enemy.Update();
                    }
                    break;
                case Stat.Pause:
                    if (keyboardState.IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyUp(Keys.Escape))
                    {
                        Stat = Stat.Game;
                    }
                    break;
            }

            previousKeyboardState = keyboardState;
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
                case Stat.Pause:
                    Clouds.Draw();
                    airplane.Draw(_spriteBatch);
                    foreach (var enemy in enemies)
                    {
                        if (enemy.IsVisible)
                        {
                            enemy.Draw(_spriteBatch);
                        }
                    }
                    // Draw paused text
                    string pausedText = "Paused";
                    Vector2 pausedTextSize = pauseFont.MeasureString(pausedText);
                    Vector2 pausedTextPosition = new Vector2((ScreenWidth - pausedTextSize.X) / 2, (ScreenHeight - pausedTextSize.Y) / 2);
                    _spriteBatch.DrawString(pauseFont, pausedText, pausedTextPosition, Color.White);
                    break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }

}
