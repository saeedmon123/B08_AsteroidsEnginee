public class Input
{
    private readonly HashSet<Keys> pressedKeys;
    // From Gemini Verification
    // Thread-safety fix: introduce a lock object to synchronize access to pressedKeys
    // between the GameLoop background thread and the WinForms UI thread.
    private readonly object lockObj = new object();

    public Input()
    {
        pressedKeys = new HashSet<Keys>();
    }

    public void KeyDown(Keys key)
    {
        lock (lockObj) { pressedKeys.Add(key); }
    }

    public void KeyUp(Keys key)
    {
        lock (lockObj) { pressedKeys.Remove(key); }
    }

    public bool IsKeyDown(Keys key)
    {
        lock (lockObj) { return pressedKeys.Contains(key); }
    }

    public void Clear()
    {
        lock (lockObj) { pressedKeys.Clear(); }
    }
}