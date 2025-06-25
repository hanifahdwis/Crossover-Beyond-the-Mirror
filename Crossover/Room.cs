using Crossover;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

public class Room
{
    public int OffsetX;
    public List<Platform> Platforms = new List<Platform>();
    public Rectangle? Door = null;
    public List<Enemy> Enemies = new List<Enemy>();
    public CheckPoint Checkpoint = null;
    private Image doorSprite;

    public Room(int offsetX, int roomIndex, Image groundImage)
    {
        OffsetX = offsetX;  
        using (var ms = new MemoryStream(Crossover.Properties.Resources.Door))
        {
            doorSprite = Image.FromStream(ms);
        }

        switch (roomIndex)
        {
            case 0:
                Platforms.Add(new Platform(offsetX + 200, 280, 100, 20, groundImage));
                Platforms.Add(new Platform(offsetX + 400, 200, 100, 20, groundImage));
                break;
            case 1:
                Platforms.Add(new Platform(offsetX + 100, 300, 150, 20, groundImage));
                Platforms.Add(new Platform(offsetX + 350, 250, 100, 20, groundImage));
                Platforms.Add(new Platform(offsetX + 600, 180, 120, 20, groundImage));
                Enemies.Add(new Enemy(offsetX + 350, 250));
                Enemies.Add(new Enemy(offsetX + 550, 180));
                break;
            case 2:
                Platforms.Add(new Platform(offsetX + 250, 250, 120, 20, groundImage));
                Enemies.Add(new Enemy(offsetX + 300, 200));
                break;
            case 3:
                Platforms.Add(new Platform(offsetX + 150, 280, 150, 20, groundImage));
                Platforms.Add(new Platform(offsetX + 450, 210, 100, 20, groundImage));
                Enemies.Add(new Enemy(offsetX + 400, 200));
                Checkpoint = new CheckPoint(new Point(offsetX + 800, 300));
                break;
            case 4:
                Platforms.Add(new Platform(offsetX + 100, 300, 100, 20, groundImage));
                Platforms.Add(new Platform(offsetX + 300, 260, 80, 20, groundImage));
                Platforms.Add(new Platform(offsetX + 550, 220, 100, 20, groundImage));
                Enemies.Add(new Enemy(offsetX + 200, 300));
                Enemies.Add(new Enemy(offsetX + 500, 220));
                break;
            case 5:
                Platforms.Add(new Platform(offsetX + 350, 200, 200, 20, groundImage));
                Platforms.Add(new Platform(offsetX + 150, 250, 120, 20, groundImage)); 
                Door = new Rectangle(offsetX + 420, 120, 120, 100);
                Enemies.Add(new Enemy(offsetX + 500, 200));
                break;
        }
    }

    public void Draw(Graphics g)
    {
        foreach (var platform in Platforms)
            platform.Draw(g);

        foreach (var enemy in Enemies)
            enemy.Draw(g);

        if (Checkpoint != null)
            Checkpoint.Draw(g);

        if (Door != null && doorSprite != null)
        {
            g.DrawImage(doorSprite, Door.Value);
        }
    }
}
