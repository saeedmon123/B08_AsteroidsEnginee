using System.Collections.Generic;
using System.Drawing;

namespace B08_AsteroidsEngine.Engine
{
    public class GameWorld
    {
        // From Gemini Verification
        // Thread-safety fix: introduce a lock object to synchronize access to Entities
        // between the GameLoop background thread and the WinForms UI thread.
        private readonly List<Entity> entities;
        private readonly object worldLock = new object();

        public GameWorld()
        {
            entities = new List<Entity>();
        }

        public IReadOnlyList<Entity> Entities => entities;

        public void AddEntity(Entity entity)
        {
            if (entity != null)
            {
                lock (worldLock) { entities.Add(entity); }
            }
        }

        public void Update(float dt)
        {
            lock (worldLock)
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
        }
        public void Render(Graphics graphics)
        {
            lock (worldLock)
            {
                for (int i = 0; i < entities.Count; i++)
                {
                    if (entities[i].IsActive)
                    {
                        entities[i].Render(graphics);
                    }
                }
            }
        }

        public void Clear()
        {
            lock (worldLock) { entities.Clear(); }
        }
    }
}