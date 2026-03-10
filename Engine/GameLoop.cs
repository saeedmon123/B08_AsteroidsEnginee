using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace B08_AsteroidsEngine.Engine
{
    public class GameLoop
    {
        private readonly Control renderTarget;
        private Thread loopThread;
        private bool running;

        public event Action<float> Update;

        public GameLoop(Control renderTarget)
        {
            this.renderTarget = renderTarget;
        }

        public void Start()
        {
            if (running)
            {
                return;
            }

            running = true;

            loopThread = new Thread(Run);
            loopThread.IsBackground = true;
            loopThread.Start();
        }

        public void Stop()
        {
            running = false;
        }

        private void Run()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            long previousTime = stopwatch.ElapsedMilliseconds;

            while (running)
            {
                long currentTime = stopwatch.ElapsedMilliseconds;
                float deltaTime = (currentTime - previousTime) / 1000f;
                previousTime = currentTime;

                Update?.Invoke(deltaTime);

                if (!running)
                {
                    break;
                }

                if (renderTarget.IsDisposed || renderTarget.Disposing)
                {
                    break;
                }

                try
                {
                    if (renderTarget.IsHandleCreated)
                    {
                        renderTarget.BeginInvoke(new Action(() =>
                        {
                            if (!renderTarget.IsDisposed && !renderTarget.Disposing)
                            {
                                renderTarget.Invalidate();
                            }
                        }));
                    }
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (InvalidOperationException)
                {
                    break;
                }

                Thread.Sleep(1);
            }
        }
    }
}