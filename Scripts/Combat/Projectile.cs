using Godot;

public partial class Projectile : Area2D
{
	[Export] public float Speed { get; set; } = 700f;
	[Export] public int Damage { get; set; } = 15;
	[Export] public float LifeSeconds { get; set; } = 2f;

	private Vector2 _direction = Vector2.Right;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		AreaEntered += OnAreaEntered;

		Lifetime();
	}

	public void SetDirection(Vector2 direction)
	{
		_direction = direction.Normalized();
		Rotation = _direction.Angle();
	}

	public override void _PhysicsProcess(double delta)
	{
		GlobalPosition += _direction * Speed * (float)delta;
	}

	private async void Lifetime()
	{
		await ToSignal(GetTree().CreateTimer(LifeSeconds), SceneTreeTimer.SignalName.Timeout);
		QueueFree();
	}

	private void OnBodyEntered(Node2D body)
	{
		var health = body.GetNodeOrNull<Health>("Health");
		if (health != null)
		{
			health.Damage(Damage);
			QueueFree();
			return;
		}

		if (body is StaticBody2D || body is TileMap || body is TileMapLayer)
			QueueFree();
	}

	private void OnAreaEntered(Area2D area)
	{
		var health = area.GetNodeOrNull<Health>("Health");
		if (health != null)
		{
			health.Damage(Damage);
			QueueFree();
		}
	}
}
