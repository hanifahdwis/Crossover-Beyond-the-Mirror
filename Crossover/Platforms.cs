using System.Drawing;

namespace Crossover
{
    public class Platform
    {
        public int X, Y, Width, Height;
        public Rectangle Bounds => new Rectangle(X, Y, Width, Height);
        public Image Image { get; set; }

        public Platform(int x, int y, int width, int height, Image image = null)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Image = image;
        }

        public void Draw(Graphics g)
        {
            g.DrawImage(Image, X, Y, Width, Height);
        }
    }
}
