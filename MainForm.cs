using System.Drawing;
using System.Windows.Forms;
using B08_AsteroidsEngine.Engine;
using B08_AsteroidsEngine.Game;

namespace B08_AsteroidsEngine
{
    public class MainForm : Form
    {
        private readonly GameLoop gameLoop;
        private readonly GameWorld gameWorld;
        private readonly Input input;
        private readonly AsteroidsRules asteroidsRules;

        public MainForm()
        {
            Text = "B08 - Asteroids Engine";
            ClientSize = new Size(1024, 768);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.Black;
            KeyPreview = true;
            DoubleBuffered = true;

            gameWorld = new GameWorld();
            input = new Input();
            asteroidsRules = new AsteroidsRules(gameWorld, input, ClientSize);
            asteroidsRules.Initialize();

            gameLoop = new GameLoop(this);
            gameLoop.Update += UpdateGame;

            KeyDown += MainForm_KeyDown;
            KeyUp += MainForm_KeyUp;

            Load += (s, e) => gameLoop.Start();
            FormClosing += (s, e) =>
            {
                gameLoop.Stop();
                input.Clear();
            };
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            input.KeyDown(e.KeyCode);
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            input.KeyUp(e.KeyCode);
        }

        private void UpdateGame(float dt)
        {
            asteroidsRules.Update(dt);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            gameWorld.Render(e.Graphics);
            asteroidsRules.RenderHud(e.Graphics, Font);
        }
    }
}