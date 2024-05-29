using System;
using System.Collections.Generic;
using JetFighter.Code;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace JetFighter
{
    public class Enemy
    {
        public static List<Texture2D> Textures { get; set; } = new List<Texture2D>();
        public Vector2 Position { get; private set; }
        private Vector2 direction;
        private float speed;
        private List<EnemyBullet> bullets;
        private float shootTimer;
        private float reloadTimer;
        private int health;
        private int enemyType;
        public bool IsVisible { get; private set; }
        private Random rand;

        private int shotsFired; // Количество выстрелов, сделанных подряд
        public int maxShots; // Максимальное количество выстрелов перед перезарядкой
        public float reloadTime; // Время перезарядки в секундах
        public float ShootCooldown { get; set; } // Время задержки между выстрелами

        public int EnemyType => enemyType;

        public Enemy(Vector2 startPosition, int enemyType, Random rand)
        {
            Position = startPosition;
            this.enemyType = enemyType;
            this.rand = rand;
            direction = new Vector2(0, 1);
            speed = 2f;
            bullets = new List<EnemyBullet>();
            shootTimer = 0f;
            reloadTimer = 0f;
            IsVisible = true;
            health = enemyType + 1; // Здоровье зависит от типа врага

            if (enemyType == 2) // Настройка для Enemy3
            {
                maxShots = 5;
                reloadTime = 3f;
                ShootCooldown = 0.1f; // Быстрая стрельба
            }
            else
            {
                maxShots = 1;
                reloadTime = 0f;
                ShootCooldown = 1f; // Настройка по умолчанию
            }

            shotsFired = 0;
        }

        public void Update()
        {
            if (!IsVisible) return;

            Position += direction * speed;

            if (Position.Y > Game1.ScreenHeight)
            {
                Position = new Vector2(rand.Next(0, Game1.ScreenWidth - Textures[enemyType].Width), rand.Next(-Game1.ScreenHeight, 0));
            }

            if (ShootCooldown != float.PositiveInfinity)
            {
                if (shotsFired >= maxShots)
                {
                    reloadTimer += (float)Game1.GameTime.ElapsedGameTime.TotalSeconds;

                    if (reloadTimer >= reloadTime)
                    {
                        shotsFired = 0;
                        reloadTimer = 0f;
                    }
                }
                else
                {
                    shootTimer += (float)Game1.GameTime.ElapsedGameTime.TotalSeconds;

                    if (shootTimer >= ShootCooldown)
                    {
                        Shoot();
                        shootTimer = 0f; // Сбросить таймер после выстрела
                        shotsFired++;
                    }
                }

                foreach (var bullet in bullets)
                {
                    bullet.Update();
                }

                bullets.RemoveAll(b => !b.IsVisible);
            }
        }

        private void Shoot()
        {
            var bulletPosition = new Vector2(Position.X + Textures[enemyType].Width / 2 - EnemyBullet.Texture.Width / 2, Position.Y + Textures[enemyType].Height);
            bullets.Add(new EnemyBullet(bulletPosition));
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                IsVisible = false;
                Position = new Vector2(rand.Next(0, Game1.ScreenWidth - Textures[enemyType].Width), rand.Next(-Game1.ScreenHeight, 0));
                health = enemyType + 1; // Сбросить здоровье
                IsVisible = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Textures[enemyType], Position, Color.White);
            foreach (var bullet in bullets)
            {
                bullet.Draw(spriteBatch);
            }
        }
    }
}
