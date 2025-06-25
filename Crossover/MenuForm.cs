using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
namespace Crossover
{
    public partial class MenuForm : Form
    {
        private Button startButton;
        private Button exitButton;
        private Label titleLabel;
        private Image backgroundImage;
        private Image startButtonImage;  
        private Image exitButtonImage;   

        public MenuForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "Crossover Beyond the mirror";
            this.Size = new Size(800, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.DoubleBuffered = true;

            this.titleLabel = new Label();
            this.titleLabel.Text = "CROSSOVER\n-Beyond the Mirror-";
            this.titleLabel.Font = new Font("Poppins", 24, FontStyle.Bold);
            this.titleLabel.ForeColor = Color.White;
            this.titleLabel.BackColor = Color.Transparent;
            this.titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.titleLabel.Size = new Size(400, 100);
            this.titleLabel.Location = new Point(200, 100);

            this.startButton = new Button();
            this.startButton.Text = "START";
            this.startButton.Font = new Font("Poppins", 16, FontStyle.Bold);
            this.startButton.Size = new Size(200, 50);
            this.startButton.Location = new Point(300, 250);
            this.startButton.ForeColor = Color.White;
            this.startButton.BackColor = Color.Transparent;
            this.startButton.FlatStyle = FlatStyle.Flat;
            this.startButton.FlatAppearance.BorderSize = 2;
            this.startButton.FlatAppearance.BorderColor = Color.White;
            this.startButton.Click += StartButton_Click;

            this.exitButton = new Button();
            this.exitButton.Text = "EXIT";
            this.exitButton.Font = new Font("Poppins", 16, FontStyle.Bold); 
            this.exitButton.Size = new Size(200, 50);
            this.exitButton.Location = new Point(300, 320);
            this.exitButton.ForeColor = Color.White;
            this.exitButton.BackColor = Color.Transparent;
            this.exitButton.FlatStyle = FlatStyle.Flat;
            this.exitButton.FlatAppearance.BorderSize = 2;
            this.exitButton.FlatAppearance.BorderColor = Color.White;
            this.exitButton.Click += ExitButton_Click;

            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.exitButton);

            this.ResumeLayout(false);

            LoadBackground();

        }

        private void LoadBackground()
        {
            using (var ms = new MemoryStream(Properties.Resources.rpg_background_sprite))
            {
                backgroundImage = Image.FromStream(ms);
            }

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            g.DrawImage(backgroundImage, 0, 0, this.Width, this.Height);

            using (Brush overlay = new SolidBrush(Color.FromArgb(128, Color.Black)))
            {
                g.FillRectangle(overlay, 0, 0, this.Width, this.Height);
            }

            base.OnPaint(e);
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();

                LevelForm gameForm = new LevelForm();
                gameForm.FormClosed += (s, args) => 
                {
                    this.Show();
                };
                gameForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting game: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Show();
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            backgroundImage?.Dispose();
            startButtonImage?.Dispose();
            exitButtonImage?.Dispose();

            Application.Exit();
            base.OnFormClosed(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                backgroundImage?.Dispose();
                startButtonImage?.Dispose();
                exitButtonImage?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}