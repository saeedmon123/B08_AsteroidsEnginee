using System;
using System.Drawing;
using B08_AsteroidsEngine.Engine;

namespace B08_AsteroidsEngine.Game
{
    public class Asteroid : Entity
    {
        private readonly Size playAreaSize;
        private readonly PointF[] shapePoints;

        public int SizeLevel { get; }

        public Asteroid(
            float x,
            float y,
            float velocityX,
            float velocityY,
            Size playAreaSize,
            int sizeLevel)
            : base(x, y, GetRadiusForSize(sizeLevel))
        {
            Velocity = new PointF(velocityX, velocityY);
            this.playAreaSize = playAreaSize;
            SizeLevel = sizeLevel;
            shapePoints = BuildLocalShape();
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            WrapAroundScreen();
        }

        public override void Render(Graphics graphics)
        {
            PointF[] worldPoints = new PointF[shapePoints.Length];

            for (int i = 0; i < shapePoints.Length; i++)
            {
                worldPoints[i] = new PointF(
                    Position.X + shapePoints[i].X,
                    Position.Y + shapePoints[i].Y);
            }

            using (Pen pen = new Pen(Color.White, 2f))
            {
                graphics.DrawPolygon(pen, worldPoints);
            }
        }

        private void WrapAroundScreen()
        {
            float x = Position.X;
            float y = Position.Y;

            if (x < -Radius)
            {
                x = playAreaSize.Width + Radius;
            }
            else if (x > playAreaSize.Width + Radius)
            {
                x = -Radius;
            }

            if (y < -Radius)
            {
                y = playAreaSize.Height + Radius;
            }
            else if (y > playAreaSize.Height + Radius)
            {
                y = -Radius;
            }

            Position = new PointF(x, y);
        }

        private PointF[] BuildLocalShape()
        {
            float radius = Radius;

            return new PointF[]
            {
                new PointF(-0.9f * radius, -0.2f * radius),
                new PointF(-0.5f * radius, -0.9f * radius),
                new PointF( 0.2f * radius, -1.0f * radius),
                new PointF( 0.9f * radius, -0.4f * radius),
                new PointF( 1.0f * radius,  0.3f * radius),
                new PointF( 0.4f * radius,  1.0f * radius),
                new PointF(-0.3f * radius,  0.8f * radius),
                new PointF(-1.0f * radius,  0.3f * radius)
            };
        }

        private static float GetRadiusForSize(int sizeLevel)
        {
            switch (sizeLevel)
            {
                case 3:
                    return 40f;
                case 2:
                    return 25f;
                case 1:
                    return 14f;
                default:
                    return 40f;
            }
        }
    }
}