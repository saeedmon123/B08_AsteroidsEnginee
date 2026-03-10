using System.Collections.Generic;
using System.Windows.Forms;

namespace B08_AsteroidsEngine.Engine
{
    public class Input
    {
        private readonly HashSet<Keys> pressedKeys;

        public Input()
        {
            pressedKeys = new HashSet<Keys>();
        }

        public void KeyDown(Keys key)
        {
            pressedKeys.Add(key);
        }

        public void KeyUp(Keys key)
        {
            pressedKeys.Remove(key);
        }

        public bool IsKeyDown(Keys key)
        {
            return pressedKeys.Contains(key);
        }

        public void Clear()
        {
            pressedKeys.Clear();
        }
    }
}