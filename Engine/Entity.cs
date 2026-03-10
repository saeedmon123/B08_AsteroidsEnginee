using System.Drawing;

namespace B08_AsteroidsEngine.Engine
{
    public abstract class Entity
    {
        public PointF Position { get; protected set; }
        public PointF Velocity { get; protected set; }
        public float Radius { get; protected set; }
        public bool IsActive { get; protected set; }

        protected Entity(float x, float y, float radius)
        {
            Position = new PointF(x, y);
            Velocity = new PointF(0f, 0f);
            Radius = radius;
            IsActive = true;
        }

        public virtual void Update(float dt)
        {
            Position = new PointF(
                Position.X + Velocity.X * dt,
                Position.Y + Velocity.Y * dt);
        }

        public abstract void Render(Graphics graphics);

        public virtual void Destroy()
        {
            IsActive = false;
        }
    }
}