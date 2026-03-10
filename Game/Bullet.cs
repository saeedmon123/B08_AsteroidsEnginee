using System.Drawing;
using B08_AsteroidsEngine.Engine;

namespace B08_AsteroidsEngine.Game
{
    public class Bullet : Entity
    {
        private readonly Size playAreaSize;
        private float remainingLifetime;

        public Bullet(float x, float y, float velocityX, float velocityY, Size playAreaSize)
            : base(x, y, 3f)
        {
            Velocity = new PointF(velocityX, velocityY);
            this.playAreaSize = playAreaSize;
            remainingLifetime = 1.2f;
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            remainingLifetime -= dt;

            if (remainingLifetime <= 0f)
            {
                Destroy();
                return;
            }

            WrapAroundScreen();
        }

        public override void Render(Graphics graphics)
        {
            float diameter = Radius * 2f;

            using (Brush brush = new SolidBrush(Color.White))
            {
                graphics.FillEllipse(
                    brush,
                    Position.X - Radius,
                    Position.Y - Radius,
                    diameter,
                    diameter);
            }
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
    }
}