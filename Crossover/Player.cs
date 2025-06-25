using Crossover;
using System.Collections.Generic;
using System.Drawing;
using Crossover;
using System.IO;
using System;

public class Player : CharacterBase
{
    public bool IsAttacking = false;
    public Action OnDie;
    public Player(int x, int y) : base(x, y) 
    { 
        LoadAnimations(); 
    }

    private void LoadAnimations()
    {
        Animations = new Dictionary<PlayerState, List<Image>>
        {
            [PlayerState.Idle] = LoadImagesFromBytes("idle_", 10),
            [PlayerState.Run] = LoadImagesFromBytes("walk_", 6),
            [PlayerState.Jump] = LoadImagesFromBytes("jump_", 4),
            [PlayerState.Attack] = LoadImagesFromBytes("attack_", 4),
        };
    }

    private List<Image> LoadImagesFromBytes(string prefix, int count)
    {
        var list = new List<Image>();
        for (int i = 1; i <= count; i++)
        {
            var name = $"{prefix}{i:D2}";
            var obj = Crossover.Properties.Resources.ResourceManager.GetObject(name);
            if (obj is byte[] bytes)
            {
                var ms = new System.IO.MemoryStream(bytes);
                list.Add(Image.FromStream(ms));
            }
        }
        return list;
    }

    public void Jump()
    {
        if (IsGrounded)
        {
            VelocityY = -15;
            IsGrounded = false;
            CurrentState = PlayerState.Jump;
        }
    }

    public void MoveLeft()
    {
        VelocityX = -5;
        IsLeft = true;
        if (IsGrounded) CurrentState = PlayerState.Run;
    }

    public void MoveRight()
    {
        VelocityX = 5;
        IsLeft = false;
        if (IsGrounded) CurrentState = PlayerState.Run;
    }

    public void Stop()
    {
        VelocityX = 0;
        if (IsGrounded) CurrentState = PlayerState.Idle;
    }

    public void Attack()
    {
        if (!IsAttacking)
        {
            CurrentState = PlayerState.Attack;
            IsAttacking = true;
        }
    }

    public void StopAttack()
    {
        IsAttacking = false;
        if (IsGrounded) CurrentState = PlayerState.Idle;
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
    }

    public override void Die()
    {
        OnDie?.Invoke();
    }

    public void Respawn(Point position)
    {
        X = position.X;
        Y = position.Y;
        VelocityX = VelocityY = 0;
        Health = 50;
        CurrentState = PlayerState.Idle;
    }
}
