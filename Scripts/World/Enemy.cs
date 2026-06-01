using Godot;

public partial class Enemy : CharacterBody2D
{
	[Export] public float MoveSpeed { get; set; } = 120f;
	[Export] public float Gravity { get; set; } = 1400f;
	[Export] public float AttackRange { get; set; } = 28f;
	[Export] public float SightRange { get; set; } = 360f;
	[Export] public float AttackCooldown { get; set; } = 1.2f;
	[Export] public int ContactDamage { get; set; } = 10;

	private PlayerController? _player;
	private Health _health = null!;
	private Timer _attackTimer = null!;

	public override void _Ready()
	{
		AddToGroup("enemies");

		_health = GetNode<Health>("Health");
		_attackTimer = GetNode<Timer>("AttackTimer");

		_health.Died += OnDied;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_health.CurrentHealth <= 0)
			return;

		_player ??= GetTree().GetFirstNodeInGroup("player") as PlayerController;
		if (_player == null)
			return;

		float dx = _player.GlobalPosition.X - GlobalPosition.X;
		float absDx = Mathf.Abs(dx);

		if (absDx <= SightRange)
		{
			if (absDx > AttackRange)
			{
				Velocity = new Vector2(Mathf.Sign(dx) * MoveSpeed, Velocity.Y);
			}
			else
			{
				Velocity = new Vector2(0, Velocity.Y);
				TryAttack();
			}
		}
		else
		{
			Velocity = new Vector2(0, Velocity.Y);
		}

		if (!IsOnFloor())
			Velocity = new Vector2(Velocity.X, Velocity.Y + Gravity * (float)delta);
		else if (Velocity.Y > 0)
			Velocity = new Vector2(Velocity.X, 0);

		MoveAndSlide();
	}

	private void TryAttack()
	{
		if (!_attackTimer.IsStopped())
			return;

		_attackTimer.Start(AttackCooldown);

		var playerHealth = _player?.GetNodeOrNull<Health>("Health");
		playerHealth?.Damage(ContactDamage);
	}

	private void OnDied()
	{
		QueueFree();
	}
}
