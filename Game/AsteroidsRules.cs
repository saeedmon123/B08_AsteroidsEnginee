using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using B08_AsteroidsEngine.Engine;

namespace B08_AsteroidsEngine.Game
{
    public class AsteroidsRules
    {
        private readonly GameWorld gameWorld;
        private readonly Input input;
        private readonly Size playAreaSize;
        private readonly Random random;

        private Ship ship;
        private float shootCooldownRemaining;
        private float shipSpawnProtectionRemaining;
        private bool shipCollisionEnabled;
        private int score;
        private int lives;
        private bool isGameOver;
        private bool restartKeyLatch;

        public AsteroidsRules(GameWorld gameWorld, Input input, Size playAreaSize)
        {
            this.gameWorld = gameWorld;
            this.input = input;
            this.playAreaSize = playAreaSize;
            random = new Random();
        }

        public void Initialize()
        {
            gameWorld.Clear();

            shootCooldownRemaining = 0f;
            shipSpawnProtectionRemaining = 0f;
            shipCollisionEnabled = false;
            score = 0;
            lives = 3;
            isGameOver = false;
            restartKeyLatch = false;

            SpawnShip();
            SpawnInitialAsteroids();
        }

        public void Update(float dt)
        {
            HandleRestartInput();

            if (isGameOver)
            {
                gameWorld.Update(dt);
                return;
            }

            if (shootCooldownRemaining > 0f)
            {
                shootCooldownRemaining -= dt;
            }

            if (shipSpawnProtectionRemaining > 0f)
            {
                shipSpawnProtectionRemaining -= dt;
            }

            UpdateShipCollisionState();
            TryShoot();
            gameWorld.Update(dt);
            HandleCollisions();
        }

        public void RenderHud(Graphics graphics, Font font)
        {
            using (Brush brush = new SolidBrush(Color.White))
            {
                graphics.DrawString("Lives + game over active", font, brush, 20, 20);
                graphics.DrawString("Left / Right = rotate", font, brush, 20, 50);
                graphics.DrawString("Up = thrust", font, brush, 20, 80);
                graphics.DrawString("Space = shoot", font, brush, 20, 110);
                graphics.DrawString("R = restart", font, brush, 20, 140);
                graphics.DrawString("Score: " + score, font, brush, 20, 170);
                graphics.DrawString("Lives: " + lives, font, brush, 20, 200);

                if (ship != null && ship.IsActive && !shipCollisionEnabled)
                {
                    graphics.DrawString("Safe start active", font, brush, 20, 230);
                }

                if (ship != null && ship.IsActive && shipSpawnProtectionRemaining > 0f)
                {
                    graphics.DrawString("Spawn protection", font, brush, 20, 260);
                }

                if (isGameOver)
                {
                    DrawCenteredText(graphics, font, "GAME OVER", 0);
                    DrawCenteredText(graphics, font, "Press R to restart", 35);
                }
            }
        }

        private void HandleRestartInput()
        {
            bool restartDown = input.IsKeyDown(Keys.R);

            if (restartDown && !restartKeyLatch)
            {
                Initialize();
            }

            restartKeyLatch = restartDown;
        }

        private void SpawnShip()
        {
            ship = new Ship(
                playAreaSize.Width / 2f,
                playAreaSize.Height / 2f,
                input,
                playAreaSize);

            gameWorld.AddEntity(ship);

            shootCooldownRemaining = 0f;
            shipSpawnProtectionRemaining = 2f;
            shipCollisionEnabled = false;
        }

        private void UpdateShipCollisionState()
        {
            if (ship == null || !ship.IsActive)
            {
                return;
            }

            if (shipCollisionEnabled)
            {
                return;
            }

            bool playerHasInteracted =
                input.IsKeyDown(Keys.Left) ||
                input.IsKeyDown(Keys.Right) ||
                input.IsKeyDown(Keys.Up) ||
                input.IsKeyDown(Keys.Space);

            if (playerHasInteracted && shipSpawnProtectionRemaining <= 0f)
            {
                shipCollisionEnabled = true;
            }
        }

        private void SpawnInitialAsteroids()
        {
            gameWorld.AddEntity(new Asteroid(150f, 120f, 60f, 30f, playAreaSize, 3));
            gameWorld.AddEntity(new Asteroid(850f, 140f, -50f, 40f, playAreaSize, 3));
            gameWorld.AddEntity(new Asteroid(180f, 620f, 35f, -55f, playAreaSize, 3));
        }

        private void TryShoot()
        {
            if (ship == null || !ship.IsActive || isGameOver)
            {
                return;
            }

            if (!input.IsKeyDown(Keys.Space))
            {
                return;
            }

            if (shootCooldownRemaining > 0f)
            {
                return;
            }

            PointF forward = ship.Forward;
            PointF spawn = ship.NosePosition;

            float bulletSpeed = 450f;

            Bullet bullet = new Bullet(
                spawn.X,
                spawn.Y,
                forward.X * bulletSpeed,
                forward.Y * bulletSpeed,
                playAreaSize);

            gameWorld.AddEntity(bullet);
            shootCooldownRemaining = 0.2f;
        }

        private void HandleCollisions()
        {
            var entities = gameWorld.Entities;

            for (int i = 0; i < entities.Count; i++)
            {
                if (!entities[i].IsActive) continue;

                // 1. Check Bullet vs Asteroid
                if (entities[i] is Bullet bullet)
                {
                    for (int j = 0; j < entities.Count; j++)
                    {
                        if (entities[j] is Asteroid asteroid && asteroid.IsActive)
                        {
                            if (AreColliding(bullet, asteroid))
                            {
                                bullet.Destroy();
                                asteroid.Destroy();
                                score += GetScoreForAsteroid(asteroid.SizeLevel);
                                SplitAsteroid(asteroid);
                                break; // Bullet is destroyed, stop checking it against other asteroids
                            }
                        }
                    }
                }
                // 2. Check Ship vs Asteroid
                else if (entities[i] is Ship shipEntity && shipCollisionEnabled)
                {
                    for (int j = 0; j < entities.Count; j++)
                    {
                        if (entities[j] is Asteroid asteroid && asteroid.IsActive)
                        {
                            if (AreColliding(shipEntity, asteroid))
                            {
                                DestroyShip();
                                break;
                            }
                        }
                    }
                }
            }
        }
        private void DestroyShip()
        {
            if (ship == null || !ship.IsActive)
            {
                return;
            }

            ship.Destroy();
            lives--;

            if (lives <= 0)
            {
                lives = 0;
                isGameOver = true;
                return;
            }

            SpawnShip();
        }

        private void SplitAsteroid(Asteroid asteroid)
        {
            if (asteroid.SizeLevel <= 1)
            {
                return;
            }

            int newSizeLevel = asteroid.SizeLevel - 1;
            float splitSpeed = GetSplitSpeedForSize(newSizeLevel);

            PointF directionA = GetRandomDirection();
            PointF directionB = new PointF(-directionA.X, -directionA.Y);

            Asteroid childA = new Asteroid(
                asteroid.Position.X,
                asteroid.Position.Y,
                directionA.X * splitSpeed,
                directionA.Y * splitSpeed,
                playAreaSize,
                newSizeLevel);

            Asteroid childB = new Asteroid(
                asteroid.Position.X,
                asteroid.Position.Y,
                directionB.X * splitSpeed,
                directionB.Y * splitSpeed,
                playAreaSize,
                newSizeLevel);

            gameWorld.AddEntity(childA);
            gameWorld.AddEntity(childB);
        }

        private PointF GetRandomDirection()
        {
            double angle = random.NextDouble() * Math.PI * 2.0;

            return new PointF(
                (float)Math.Cos(angle),
                (float)Math.Sin(angle));
        }

        private static float GetSplitSpeedForSize(int sizeLevel)
        {
            switch (sizeLevel)
            {
                case 2:
                    return 90f;
                case 1:
                    return 130f;
                default:
                    return 80f;
            }
        }

        private static int GetScoreForAsteroid(int sizeLevel)
        {
            switch (sizeLevel)
            {
                case 3:
                    return 20;
                case 2:
                    return 50;
                case 1:
                    return 100;
                default:
                    return 0;
            }
        }

        private static bool AreColliding(Entity first, Entity second)
        {
            float dx = first.Position.X - second.Position.X;
            float dy = first.Position.Y - second.Position.Y;

            float distanceSquared = dx * dx + dy * dy;
            float radiusSum = first.Radius + second.Radius;

            return distanceSquared <= radiusSum * radiusSum;
        }

        private void DrawCenteredText(Graphics graphics, Font font, string text, int verticalOffset)
        {
            SizeF textSize = graphics.MeasureString(text, font);

            float x = (playAreaSize.Width - textSize.Width) / 2f;
            float y = (playAreaSize.Height - textSize.Height) / 2f + verticalOffset;

            graphics.DrawString(text, font, Brushes.White, x, y);
        }
    }
}