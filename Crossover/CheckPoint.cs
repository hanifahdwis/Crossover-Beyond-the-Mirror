using System.Drawing;
using System.IO;

namespace Crossover
{
    public class CheckPoint
    {
        public Point Position;
        public Rectangle Bounds;
        private Image checkpointImage;

        public CheckPoint(Point position)
        {
            Position = position;
            Bounds = new Rectangle(position.X, position.Y, 80, 60);
            LoadCheckpointImage();
        }

        private void LoadCheckpointImage()
        {
            var imageBytes = Properties.Resources.Checkpoint;
            if (imageBytes != null)
            {
                using (var ms = new MemoryStream(imageBytes))
                {
                    checkpointImage = Image.FromStream(ms);
                }
            }
        }

        public void Draw(Graphics g)
        {
            if (checkpointImage != null)
            {
                g.DrawImage(checkpointImage, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
            }
        }
    }
}