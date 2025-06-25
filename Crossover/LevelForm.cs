using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Media;

namespace Crossover
{
    public enum PlayerState { Idle, Run, Jump, Attack }
    public enum EnemyState { Idle }

    public class LevelForm : Form
    {
        private List<Enemy> activeEnemies = new List<Enemy>();

        Timer gameTimer = new Timer();
        Timer animationTimer = new Timer();

        Player player;
        List<Candy> candies = new List<Candy>();
        Rectangle ground = new Rectangle(0, 350, 800, 100);

        int cameraOffsetX = 0;
        List<Room> rooms = new List<Room>();
        int currentRoomIndex = 0;

        Point lastCheckpoint = new Point(50, 300);
        private Image backgroundImage;
        Image[] cachedBackgrounds = new Image[6];
        private bool warning = false;
        private bool touchDoor = false;

        private bool isPaused = false;
        private Panel pausePanel;
        private Button resumeButton;
        private Button menuButton;
        private Label pauseLabel;
        private Button pauseMenuButton;
        private SoundPlayer backgroundMusicPlayer;

        public LevelForm()
        {
            this.DoubleBuffered = true;
            this.Width = 800;
            this.Height = 450;
            this.Text = "Crossover Beyond the Mirror";

            InitializeSounds();

            Image groundImage = ByteArrayToImage(Properties.Resources.ground);

            for (int i = 0; i < 6; i++)
                rooms.Add(new Room(i * 800, i, groundImage));

            gameTimer.Interval = 16;
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            animationTimer.Interval = 100;
            animationTimer.Tick += Animate;
            animationTimer.Start();

            this.KeyDown += OnKeyDown;
            this.KeyUp += OnKeyUp;

            player = new Player(50, 300);
            player.OnDie = () =>
            {
                player.Respawn(lastCheckpoint);
            };

            using (var ms = new MemoryStream(Properties.Resources.rpg_background_sprite))
            {
                backgroundImage = Image.FromStream(ms);
            }

            InitializePauseMenu();

            this.Focus();
            this.KeyPreview = true;

            PlayBackgroundMusic();
        }

        private void InitializeSounds()
        {
            using (var ms = new MemoryStream(Properties.Resources._6__Track_6))
            {
                backgroundMusicPlayer = new SoundPlayer(ms);
                backgroundMusicPlayer.Load();
            }
        }

        private void PlayBackgroundMusic()
        {
                backgroundMusicPlayer?.PlayLooping(); 
            
        }

        private void InitializePauseMenu()
        {
            pauseMenuButton = new Button();
            pauseMenuButton.Text = "⋮"; 
            pauseMenuButton.Size = new Size(40, 40);
            pauseMenuButton.Location = new Point(this.ClientSize.Width - 50, 10);
            pauseMenuButton.Font = new Font("Arial", 16, FontStyle.Bold);
            pauseMenuButton.BackColor = Color.Pink;
            pauseMenuButton.ForeColor = Color.White;
            pauseMenuButton.FlatStyle = FlatStyle.Flat;
            pauseMenuButton.FlatAppearance.BorderSize = 1;
            pauseMenuButton.FlatAppearance.BorderColor = Color.White;
            pauseMenuButton.Click += PauseMenuButton_Click;
            pauseMenuButton.TabStop = false; 

            this.Controls.Add(pauseMenuButton);
            pauseMenuButton.BringToFront();

            pausePanel = new Panel();
            pausePanel.Size = new Size(300, 200);
            pausePanel.Location = new Point((this.ClientSize.Width - 300) / 2, (this.ClientSize.Height - 200) / 2);
            pausePanel.BackColor = Color.FromArgb(220, 0, 0, 0); 
            pausePanel.BorderStyle = BorderStyle.FixedSingle;
            pausePanel.Visible = false;

            pauseLabel = new Label();
            pauseLabel.Text = "GAME PAUSED";
            pauseLabel.Font = new Font("Poppins", 16, FontStyle.Bold);
            pauseLabel.ForeColor = Color.White;
            pauseLabel.Size = new Size(280, 40);
            pauseLabel.Location = new Point(10, 20);
            pauseLabel.TextAlign = ContentAlignment.MiddleCenter;

            resumeButton = new Button();
            resumeButton.Text = "Resume";
            resumeButton.Size = new Size(120, 35);
            resumeButton.Location = new Point(90, 70);
            resumeButton.Font = new Font("Poppins", 10, FontStyle.Bold);
            resumeButton.BackColor = Color.Pink;
            resumeButton.ForeColor = Color.White;
            resumeButton.FlatStyle = FlatStyle.Flat;
            resumeButton.TabStop = false;
            resumeButton.Click += ResumeButton_Click;

            menuButton = new Button();
            menuButton.Text = "Main Menu";
            menuButton.Size = new Size(120, 35);
            menuButton.Location = new Point(90, 120);
            menuButton.Font = new Font("Poppins", 10, FontStyle.Bold);
            menuButton.BackColor = Color.Pink;
            menuButton.ForeColor = Color.White;
            menuButton.FlatStyle = FlatStyle.Flat;
            menuButton.TabStop = false;
            menuButton.Click += MenuButton_Click;

            pausePanel.Controls.Add(pauseLabel);
            pausePanel.Controls.Add(resumeButton);
            pausePanel.Controls.Add(menuButton);

            this.Controls.Add(pausePanel);
            pausePanel.BringToFront();
        }

        private void PauseMenuButton_Click(object sender, EventArgs e)
        {
            PauseGame();
        }

        private void PauseGame()
        {
            isPaused = true;
            gameTimer.Stop();
            animationTimer.Stop();
            pausePanel.Visible = true;
            pausePanel.BringToFront();
            backgroundMusicPlayer?.Stop();
        }

        private void ResumeGame()
        {
            isPaused = false;
            pausePanel.Visible = false;

            this.Focus();
            this.Activate();

            gameTimer.Start();
            animationTimer.Start();
            PlayBackgroundMusic();
        }

        private void ResumeButton_Click(object sender, EventArgs e)
        {
            ResumeGame();
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Kembali ke menu utama? Progress akan hilang!",
                                                "Konfirmasi",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                gameTimer.Stop();
                animationTimer.Stop();
                backgroundMusicPlayer?.Stop();

                MenuForm menuForm = new MenuForm();
                menuForm.Show();
                this.Hide();
            }
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (isPaused) return; 

            player.Update();

            bool onPlatform = false;

            foreach (var platform in rooms[currentRoomIndex].Platforms)
            {
                Rectangle playerFeet = new Rectangle(player.X, player.Y + player.Height, player.Width, 5);
                if (playerFeet.IntersectsWith(platform.Bounds) && player.VelocityY >= 0)
                {
                    player.Y = platform.Y - player.Height;
                    player.IsGrounded = true;
                    onPlatform = true;

                    if (player.CurrentState == PlayerState.Jump)
                        player.CurrentState = player.VelocityX == 0 ? PlayerState.Idle : PlayerState.Run;

                    break;
                }
            }

            var checkpoint = rooms[currentRoomIndex].Checkpoint;
            if (checkpoint != null)
            {
                Rectangle playerRect = new Rectangle(player.X, player.Y, player.Width, player.Height);
                if (playerRect.IntersectsWith(checkpoint.Bounds))
                {
                    lastCheckpoint = checkpoint.Position;
                }
            }

            bool onGround = false;
            if (player.Y + player.Height >= 350)
            {
                player.Y = 350 - player.Height;
                player.IsGrounded = true;
                onGround = true;

                if (player.CurrentState == PlayerState.Jump)
                    player.CurrentState = player.VelocityX == 0 ? PlayerState.Idle : PlayerState.Run;
            }

            if (!onPlatform && !onGround)
            {
                player.IsGrounded = false;
                player.CurrentState = PlayerState.Jump;
            }

            currentRoomIndex = Math.Max(0, Math.Min(rooms.Count - 1, player.X / 800));
            activeEnemies = rooms[currentRoomIndex].Enemies;
            foreach (var enemy in activeEnemies.ToList())
            {
                if (enemy.IsDead) continue;

                Rectangle enemyRect = new Rectangle(enemy.X, enemy.Y, enemy.Width, enemy.Height);
                Rectangle playerRect = new Rectangle(player.X, player.Y, player.Width, player.Height);

                int distance = Math.Abs(player.X - enemy.X);
                if (distance < 200)
                {
                    enemy.ChasePlayer(player);
                }
                else
                {
                    enemy.Patrol();
                }

                if (enemyRect.IntersectsWith(playerRect))
                {
                    enemy.AttackPlayer(player);
                }
                enemy.UpdateGravity(rooms[currentRoomIndex].Platforms);
            }

            foreach (var candy in candies.ToList()) 
            {
                if (!candy.IsAlive) continue; 

                candy.Move();

                foreach (var enemy in activeEnemies.ToList())
                {
                    if (!candy.IsAlive) break; 

                    if (candy.Bounds.IntersectsWith(enemy.Bounds))
                    {
                        candy.DealDamage(enemy);
                        if (enemy.IsDead) activeEnemies.Remove(enemy);
                        break;
                    }
                }
            }

            candies.RemoveAll(c => !c.IsAlive);

            int worldWidth = rooms.Count * 800;
            if (player.X + player.Width >= worldWidth)
            {
                player.X = worldWidth - player.Width;
                player.VelocityX = 0;
            }

            int maxCameraOffset = (rooms.Count * 800) - this.ClientSize.Width;
            cameraOffsetX = Math.Max(0, Math.Min(maxCameraOffset, player.X - 400));

            if (player.X < 0)
                player.X = 0;
            if (currentRoomIndex == 5 && rooms[5].Door.HasValue)
            {
                Rectangle playerRect = new Rectangle(player.X, player.Y, player.Width, player.Height);
                bool playerTouchingDoor = playerRect.IntersectsWith(rooms[5].Door.Value);

                if (playerTouchingDoor)
                {

                    if (!touchDoor)
                    {
                        touchDoor = true;
                        bool allEnemiesDefeated = rooms.SelectMany(r => r.Enemies).All(enemy => enemy.IsDead);

                        if (allEnemiesDefeated)
                        {
                            gameTimer.Stop();
                            animationTimer.Stop();
                            backgroundMusicPlayer?.Stop();

                            MessageBox.Show("GAME SUCCEED!!!", "Congratulations", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                            return;
                        }
                        else
                        {
                            MessageBox.Show("Kalahkan semua musuh terlebih dahulu!", "Tidak bisa lanjut", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else
                {
                    touchDoor = false;
                }
            }
            Invalidate();
        }

        private void Animate(object sender, EventArgs e)
        {
            if (isPaused) return; 

            var anim = player.Animations[player.CurrentState];
            player.CurrentFrame = (player.CurrentFrame + 1) % anim.Count;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.TranslateTransform(-cameraOffsetX, 0);

            for (int i = 0; i < rooms.Count; i++)
            {
                Image bg = GetRoomBackground(i);
                g.DrawImage(bg, i * 800, 0, 800, 450);
            }

            for (int i = 0; i < rooms.Count; i++)
            {
                g.DrawImage(ByteArrayToImage(Properties.Resources.ground), i * 800, 350, 800, 100);
            }

            foreach (var room in rooms)
                room.Draw(g);

            player.Draw(g);

            foreach (var candy in candies)
                candy.Draw(g);

            g.ResetTransform();
        }

        private void HandleInputDown(Keys key)
        {
            if (isPaused) return; 

            switch (key)
            {
                case Keys.A:
                    player.MoveLeft();
                    break;
                case Keys.D:
                    player.MoveRight();
                    break;
                case Keys.Space:
                    player.Jump();
                    break;
                case Keys.J:
                    if (!player.IsAttacking)
                    {
                        player.Attack();
                        ShootCandy();
                    }
                    break;
            }
        }

        private void HandleInputUp(Keys key)
        {
            if (isPaused) return; 

            if (key == Keys.A || key == Keys.D)
                player.Stop();

            if (key == Keys.J)
                player.StopAttack();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (isPaused)
                    ResumeGame();
                else
                    PauseGame();
                return;
            }

            HandleInputDown(e.KeyCode);
        }

        private void OnKeyUp(object sender, KeyEventArgs e) => HandleInputUp(e.KeyCode);

        private void ShootCandy()
        {
            int dir = player.IsLeft ? -1 : 1;
            int startX = player.X + 16 + (dir * 25);
            int startY = player.Y + 20;
            candies.Add(new Candy(startX, startY, dir));
        }

        private Image GetRoomBackground(int roomIndex)
        {
            if (cachedBackgrounds[roomIndex] != null)
                return cachedBackgrounds[roomIndex];

            if (roomIndex % 2 == 1)
            {
                Bitmap flipped = new Bitmap(backgroundImage);
                flipped.RotateFlip(RotateFlipType.RotateNoneFlipX);
                cachedBackgrounds[roomIndex] = flipped;
            }
            else
            {
                cachedBackgrounds[roomIndex] = backgroundImage;
            }

            return cachedBackgrounds[roomIndex];
        }

        private Image ByteArrayToImage(byte[] byteArray)
        {
            using (var ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }
    }
}