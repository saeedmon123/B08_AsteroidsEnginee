using System;
using System.Drawing;
using System.Windows.Forms;
using B08_AsteroidsEngine.Engine;

namespace B08_AsteroidsEngine.Game
{
    public class Ship : Entity
    {
        private readonly Input input;
        private readonly Size playAreaSize;

        private float angle;
        private readonly float rotationSpeed;
        private readonly float thrustPower;
        private readonly float maxSpeed;
        private readonly float friction;

        public Ship(float x, float y, Input input, Size playAreaSize)
            : base(x, y, 12f)
        {
            this.input = input;
            this.playAreaSize = playAreaSize;

            angle = -90f;
            rotationSpeed = 180f;
            thrustPower = 220f;
            maxSpeed = 300f;
            friction = 0.99f;
        }

        public float Angle => angle;

        public PointF Forward
        {
            get
            {
                float radians = DegreesToRadians(angle);
                return new PointF(
                    (float)Math.Cos(radians),
                    (float)Math.Sin(radians));
            }
        }

        public PointF NosePosition
        {
            get
            {
                PointF forward = Forward;
                return new PointF(
                    Position.X + forward.X * 16f,
                    Position.Y + forward.Y * 16f);
            }
        }

        public override void Update(float dt)
        {
            HandleRotation(dt);
            HandleThrust(dt);

            base.Update(dt);

            ApplyFriction();
            ClampSpeed();
            WrapAroundScreen();
        }

        public override void Render(Graphics graphics)
        {
            PointF[] shipPoints = BuildShipShape();

            using (Pen pen = new Pen(Color.White, 2f))
            {
                graphics.DrawPolygon(pen, shipPoints);
            }
        }

        private void HandleRotation(float dt)
        {
            if (input.IsKeyDown(Keys.Left))
            {
                angle -= rotationSpeed * dt;
            }

            if (input.IsKeyDown(Keys.Right))
            {
                angle += rotationSpeed * dt;
            }
        }

        private void HandleThrust(float dt)
        {
            if (!input.IsKeyDown(Keys.Up))
            {
                return;
            }

            PointF forward = Forward;

            float accelerationX = forward.X * thrustPower * dt;
            float accelerationY = forward.Y * thrustPower * dt;

            Velocity = new PointF(
                Velocity.X + accelerationX,
                Velocity.Y + accelerationY);
        }

        private void ApplyFriction()
        {
            Velocity = new PointF(
                Velocity.X * friction,
                Velocity.Y * friction);
        }

        private void ClampSpeed()
        {
            float speed = (float)Math.Sqrt(Velocity.X * Velocity.X + Velocity.Y * Velocity.Y);

            if (speed <= maxSpeed)
            {
                return;
            }

            float scale = maxSpeed / speed;

            Velocity = new PointF(
                Velocity.X * scale,
                Velocity.Y * scale);
        }

        private void WrapAroundScreen()
        {
            float x = Position.X;
            float y = Position.Y;

            if (x < 0)
            {
                x = playAreaSize.Width;
            }
            else if (x > playAreaSize.Width)
            {
                x = 0;
            }

            if (y < 0)
            {
                y = playAreaSize.Height;
            }
            else if (y > playAreaSize.Height)
            {
                y = 0;
            }

            Position = new PointF(x, y);
        }

        private PointF[] BuildShipShape()
        {
            PointF[] localPoints = new PointF[]
            {
                new PointF(16f, 0f),
                new PointF(-10f, -8f),
                new PointF(-4f, 0f),
                new PointF(-10f, 8f)
            };

            PointF[] worldPoints = new PointF[localPoints.Length];
            float radians = DegreesToRadians(angle);

            for (int i = 0; i < localPoints.Length; i++)
            {
                float rotatedX =
                    localPoints[i].X * (float)Math.Cos(radians) -
                    localPoints[i].Y * (float)Math.Sin(radians);

                float rotatedY =
                    localPoints[i].X * (float)Math.Sin(radians) +
                    localPoints[i].Y * (float)Math.Cos(radians);

                worldPoints[i] = new PointF(
                    Position.X + rotatedX,
                    Position.Y + rotatedY);
            }

            return worldPoints;
        }

        private static float DegreesToRadians(float degrees)
        {
            return degrees * (float)Math.PI / 180f;
        }
    }
}