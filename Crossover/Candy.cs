using System;
using System.Drawing;

namespace Crossover
{
    public class Candy
    {
        public int X, Y;
        int speed = 10;
        int direction;
        int lifetime = 0;
        public bool IsAlive = true;
        public int Damage;

        public Candy(int x, int y, int direction)
        {
            this.X = x;
            this.Y = y;
            this.direction = direction;
        }

        public void Move()
        {
            X += speed * direction;
            lifetime++;
            if (lifetime > 300) IsAlive = false;
        }

        public void DealDamage(Enemy enemy)
        {
            enemy.TakeDamage(20);
            IsAlive = false;
        }

        public void Draw(Graphics g)
        {
            g.FillEllipse(Brushes.Pink, X, Y, 20, 20);
        }

        public Rectangle Bounds => new Rectangle(X, Y, 20, 20);
    }
}
