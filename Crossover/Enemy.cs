using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Crossover;

public class Enemy : CharacterBase
{
    public int MoveSpeed = 2;
    public int RangePatrol = 100;
    private int patrolOrigin;
    public bool IsDead { get; private set; } = false;
    public Rectangle Bounds => new Rectangle(X, Y, Width, Height);
    private int shootCooldown = 0;
    private Dictionary<EnemyState, List<Image>> Animations;
    public EnemyState CurrentState = EnemyState.Idle;
    public int attackCooldown = 0;
    private int currentFrame = 0;
    private int frameCounter = 0;
    private int frameSpeed = 10;
    public int VelocityY = 0;
    public bool IsGrounded = false;

    public Enemy(int x, int y) : base(x, y)
    {
        patrolOrigin = x;
        LoadAnimations();

    }
    private void LoadAnimations()
    {
        Animations = new Dictionary<EnemyState, List<Image>>
        {
            [EnemyState.Idle] = LoadImages("enemy_", 5),
                                                       
        };
    }

    private List<Image> LoadImages(string prefix, int count)
    {
        var list = new List<Image>();
        for (int i = 1; i <= count; i++)
        {
            var name = $"{prefix}{i:D2}";
            var obj = Crossover.Properties.Resources.ResourceManager.GetObject(name);
            if (obj is byte[] bytes)
            {
                using (var ms = new MemoryStream(bytes))
                    list.Add(Image.FromStream(ms));
            }
        }
        return list;
    }
    public void UpdateGravity(List<Platform> platforms)
    {
        Rectangle feet = new Rectangle(X, Y + Height, Width, 5);
        IsGrounded = false;

        foreach (var platform in platforms)
        {
            if (feet.IntersectsWith(platform.Bounds) && VelocityY >= 0)
            {
                Y = platform.Y - Height;
                VelocityY = 0;
                IsGrounded = true;
                break;
            }
        }

        if (!IsGrounded && Y + Height < 350)
        {
            VelocityY += 1; 
            Y += VelocityY;
        }
        else if (Y + Height >= 350)
        {
            Y = 350 - Height;
            VelocityY = 0;
            IsGrounded = true;
        }
    }

    public void Patrol()
    {
        CurrentState = EnemyState.Idle;
        X += IsLeft ? -MoveSpeed : MoveSpeed;

        if (X < patrolOrigin - RangePatrol || X > patrolOrigin + RangePatrol)
            IsLeft = !IsLeft;
    }

    public void ChasePlayer(Player player)
    {
        IsLeft = player.X < X;
        X += IsLeft ? -MoveSpeed : MoveSpeed;

    }

    public void AttackPlayer(Player player)
    {
        if (attackCooldown <= 0)
        {
            player.TakeDamage(30);
            attackCooldown = 60;
        }
        else
        {
            attackCooldown--; 
        }
    }


    public override void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health <= 0)
            Die();
    }

    public override void Die()
    {
        IsDead = true;
    }

    public override void Draw(Graphics g)
    {
        if (!Animations.ContainsKey(CurrentState))
            return;

        var frames = Animations[CurrentState];

        frameCounter++;
        if (frameCounter >= frameSpeed)
        {
            currentFrame = (currentFrame + 1) % frames.Count;
            frameCounter = 0;
        }

        var sprite = new Bitmap(frames[currentFrame]);
        if (IsLeft)
            sprite.RotateFlip(RotateFlipType.RotateNoneFlipX);


        g.DrawImage(sprite, X, Y, Width, Height);
    }
}
