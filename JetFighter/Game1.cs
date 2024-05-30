using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using JetFighter.Code;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;



namespace JetFighter
{
    enum Stat
    {
        SplashScreen,
        Game,
        Final,
        Pause,
        Lose
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
        private Stat Stat = Stat.SplashScreen;
        private Random rand;
        private SpriteFont pauseFont;
        private SpriteFont scoreFont;
        private SpriteFont loseFont;
        private Texture2D loseTexture;
        private Texture2D healthTexture; // Текстура для визуализации жизней
        private float volume = 0.5f; // Начальная громкость
        private Texture2D increaseVolumeTexture; // Текстура кнопки увеличения громкости
        private Texture2D decreaseVolumeTexture; // Текстура кнопки уменьшения громкости
        private KeyboardState previousKeyboardState;
        private int score;
        private int playerHits; // Количество попаданий по игроку
        private const int maxLives = 3; // Максимальное количество жизней игрока
        private Song backgroundMusic; // Фоновая музыка
        public static SoundEffect ShootSound;

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

            Texture2D airplaneTexture = Content.Load<Texture2D>("Jet");
            Vector2 airplaneStartPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight - airplaneTexture.Height);
            airplane = new Airplane(airplaneStartPosition);
            Airplane.Texture = airplaneTexture;

            Bullet.Texture = Content.Load<Texture2D>("Bullet");
            EnemyBullet.Texture = Content.Load<Texture2D>("EnemyBullet");

            Enemy.Textures.Add(Content.Load<Texture2D>("Enemy1"));
            Enemy.Textures.Add(Content.Load<Texture2D>("Enemy2"));
            Enemy.Textures.Add(Content.Load<Texture2D>("Enemy3"));
            Enemy.Textures.Add(Content.Load<Texture2D>("Enemy4"));

            pauseFont = Content.Load<SpriteFont>("PauseFont");
            scoreFont = Content.Load<SpriteFont>("ScoreFont");
            loseFont = Content.Load<SpriteFont>("LoseFont");

            loseTexture = Content.Load<Texture2D>("Lose");

            healthTexture = Content.Load<Texture2D>("health");

            increaseVolumeTexture = Content.Load<Texture2D>("IncreaseVolume");
            decreaseVolumeTexture = Content.Load<Texture2D>("DecreaseVolume");

            backgroundMusic = Content.Load<Song>("backgroundMusic");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(backgroundMusic);
            ShootSound = Content.Load<SoundEffect>("Shoot");

            enemies = new List<Enemy>();
            for (int i = 0; i < 4; i++)
            {
                Vector2 enemyStartPosition = new Vector2(rand.Next(0, ScreenWidth - Enemy.Textures[i].Width), rand.Next(-ScreenHeight, 0));
                var enemy = new Enemy(enemyStartPosition, i, rand, this);

                switch (i)
                {
                    case 0:
                        enemy.ShootCooldown = float.PositiveInfinity;
                        break;
                    case 1:
                        enemy.ShootCooldown = 0.3f;
                        enemy.maxShots = 2;
                        enemy.reloadTime = 2f;
                        break;
                    case 2:
                        enemy.ShootCooldown = 0.1f;
                        enemy.maxShots = 5;
                        enemy.reloadTime = 3f;
                        break;
                    case 3:
                        enemy.ShootCooldown = 2f;
                        break;
                }

                enemies.Add(enemy);
            }

            playerHits = 0;
        }

        public void UpdateScore(int points)
        {
            score += points;
        }

        protected override void Update(GameTime gameTime)
        {
            GameTime = gameTime;
            KeyboardState keyboardState = Keyboard.GetState();

            switch (Stat)
            {
                case Stat.SplashScreen:
                    SplashScreen.Update();
                    if (keyboardState.IsKeyDown(Keys.Space) && previousKeyboardState.IsKeyUp(Keys.Space))
                    {
                        Stat = Stat.Game;
                    }
                    if (keyboardState.IsKeyDown(Keys.Up) && previousKeyboardState.IsKeyUp(Keys.Up))
                    {
                        volume = MathHelper.Clamp(volume + 0.1f, 0f, 1f);
                        MediaPlayer.Volume = volume;
                    }
                    if (keyboardState.IsKeyDown(Keys.Down) && previousKeyboardState.IsKeyUp(Keys.Down))
                    {
                        volume = MathHelper.Clamp(volume - 0.1f, 0f, 1f);
                        MediaPlayer.Volume = volume;
                    }
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
                        foreach (var bullet in enemy.Bullets)
                        {
                            if (bullet.IsVisible && bullet.BoundingBox.Intersects(new Rectangle((int)airplane.Position.X, (int)airplane.Position.Y, Airplane.Texture.Width, Airplane.Texture.Height)))
                            {
                                playerHits++;
                                bullet.IsVisible = false;

                                if (playerHits >= maxLives)
                                {
                                    Stat = Stat.Lose;
                                }
                            }
                        }

                        if (enemy.IsVisible && new Rectangle((int)enemy.Position.X, (int)enemy.Position.Y, Enemy.Textures[enemy.EnemyType].Width, Enemy.Textures[enemy.EnemyType].Height)
                            .Intersects(new Rectangle((int)airplane.Position.X, (int)airplane.Position.Y, Airplane.Texture.Width, Airplane.Texture.Height)))
                        {
                            playerHits++;

                            if (playerHits >= maxLives)
                            {
                                Stat = Stat.Lose;
                            }

                            enemy.ResetPosition();
                        }
                    }
                    break;
                case Stat.Pause:
                    if (keyboardState.IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyUp(Keys.Escape))
                    {
                        Stat = Stat.Game;
                    }
                    break;
                case Stat.Lose:
                    if (keyboardState.IsKeyDown(Keys.Space) && previousKeyboardState.IsKeyUp(Keys.Space))
                    {
                        playerHits = 0;
                        score = 0;
                        enemies.Clear();
                        LoadContent();
                        Stat = Stat.SplashScreen;
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
                    _spriteBatch.Draw(increaseVolumeTexture, new Vector2(750, 500), Color.White);
                    _spriteBatch.Draw(decreaseVolumeTexture, new Vector2(750, 550), Color.White);
                    _spriteBatch.DrawString(SplashScreen.Font, "Volume: " + (int)(volume * 100), new Vector2(500, 600), Color.White);
                    break;
                case Stat.Game:
                    Clouds.Draw();
                    airplane.Draw(_spriteBatch);
                    foreach (var enemy in enemies)
                    {
                        enemy.Draw(_spriteBatch);
                    }
                    _spriteBatch.DrawString(scoreFont, "Score: " + score, new Vector2(10, 10), Color.White);
                    DrawLives();
                    break;
                case Stat.Pause:
                    Clouds.Draw();
                    airplane.Draw(_spriteBatch);
                    foreach (var enemy in enemies)
                    {
                        enemy.Draw(_spriteBatch);
                    }
                    _spriteBatch.DrawString(scoreFont, "Score: " + score, new Vector2(10, 10), Color.White);
                    _spriteBatch.DrawString(pauseFont, "Paused", new Vector2(ScreenWidth / 2 - 50, ScreenHeight / 2 - 50), Color.White);
                    DrawLives();
                    break;
                case Stat.Lose:
                    _spriteBatch.Draw(loseTexture, new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White);
                    _spriteBatch.DrawString(loseFont, "Final Score: " + score, new Vector2(ScreenWidth / 2 - 100, ScreenHeight / 2 + 100), Color.White);
                    break;
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawLives()
        {
            for (int i = 0; i < maxLives - playerHits; i++)
            {
                _spriteBatch.Draw(healthTexture, new Vector2(10 + i * healthTexture.Width, ScreenHeight - healthTexture.Height - 10), Color.White);
            }
        }
    }
}
