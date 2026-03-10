using System.Collections.Generic;
using System.Drawing;

namespace B08_AsteroidsEngine.Engine
{
    public class GameWorld
    {
        private readonly List<Entity> entities;

        public GameWorld()
        {
            entities = new List<Entity>();
        }

        public IReadOnlyList<Entity> Entities => entities;

        public void AddEntity(Entity entity)
        {
            if (entity != null)
            {
                entities.Add(entity);
            }
        }

        public void Update(float dt)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].IsActive)
                {
                    entities[i].Update(dt);
                }
            }

            entities.RemoveAll(entity => !entity.IsActive);
        }

        public void Render(Graphics graphics)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].IsActive)
                {
                    entities[i].Render(graphics);
                }
            }
        }

        public void Clear()
        {
            entities.Clear();
        }
    }
}