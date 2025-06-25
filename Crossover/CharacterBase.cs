using Crossover;
using System.Collections.Generic;
using System.Drawing;

public abstract class CharacterBase
{
    public int X, Y;
    public int Width = 48, Height = 48;

    public int VelocityX = 0, VelocityY = 0;
    public bool IsGrounded = false;
    public bool IsLeft = false;

    public PlayerState CurrentState = PlayerState.Idle;
    public int CurrentFrame = 0;

    public Dictionary<PlayerState, List<Image>> Animations;

    public int Health = 50;

    public CharacterBase(int x, int y, Dictionary<PlayerState, List<Image>> animations)
    {
        X = x;
        Y = y;
        Animations = animations;
    }

    protected CharacterBase(int x, int y)
    {
        X = x;
        Y = y;
    }

    public virtual void Update()
    {
        if (!IsGrounded)
            VelocityY += 1;

        X += VelocityX;
        Y += VelocityY;
    }

    public virtual void Draw(Graphics g)
    {
        var anim = Animations[CurrentState];
        if (CurrentFrame >= anim.Count)
            CurrentFrame = 0;

        var image = anim[CurrentFrame];
        var flipped = new Bitmap(image);
        if (IsLeft)
            flipped.RotateFlip(RotateFlipType.RotateNoneFlipX);

        g.DrawImage(flipped, X, Y, Width, Height);
    }

    public virtual void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health <= 0)
            Die();
    }

    public abstract void Die();
}
