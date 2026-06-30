using Godot;

public partial class PlayerController : CharacterBody2D
{
	[Export] public float MoveSpeed { get; set; } = 220f;
	[Export] public float Gravity { get; set; } = 1400f;

	public Inventory Inventory { get; private set; } = null!;
	public Health Health { get; private set; } = null!;
	public PlayerCombat Combat { get; private set; } = null!;
	public InteractionDetector InteractionDetector { get; private set; } = null!;

	private Sprite2D _sprite = null!;
	private Camera2D _camera = null!;
	public bool FacingRight { get; private set; } = true;

	public override void _Ready()
	{
		AddToGroup("player");

		_sprite = GetNode<Sprite2D>("Sprite2D");
		_camera ??= GetNode<Camera2D>("Camera2D");
		Inventory = GetNode<Inventory>("Inventory");
		Health = GetNode<Health>("Health");
		Combat = GetNode<PlayerCombat>("Combat");
		InteractionDetector = GetNode<InteractionDetector>("InteractionDetector");

		Health.Died += OnDied;
		Combat.SetFacingRight(true);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Health.CurrentHealth <= 0)
			return;

		float moveInput = Input.GetAxis("move_left", "move_right");

		if (Mathf.Abs(moveInput) > 0.01f)
		{
			Velocity = new Vector2(moveInput * MoveSpeed, Velocity.Y);
			SetFacing(moveInput > 0);
		}
		else
		{
			Velocity = new Vector2(Mathf.MoveToward(Velocity.X, 0, MoveSpeed * 6f * (float)delta), Velocity.Y);
		}

		if (!IsOnFloor())
			Velocity = new Vector2(Velocity.X, Velocity.Y + Gravity * (float)delta);
		else if (Velocity.Y > 0)
			Velocity = new Vector2(Velocity.X, 0);

		MoveAndSlide();

		if (Input.IsActionJustPressed("interact"))
			InteractionDetector.TryInteract(this);

		if (Input.IsActionJustPressed("attack_melee"))
			Combat.TryMeleeAttack();

		if (Input.IsActionJustPressed("attack_ranged"))
			Combat.TryRangedAttack();
	}

	private void SetFacing(bool facingRight)
	{
		FacingRight = facingRight;
		_sprite.FlipH = !facingRight;
		Combat.SetFacingRight(facingRight);
	}

	public void SetCameraLimits(float left, float right, float top, float bottom)
	{
		_camera ??= GetNode<Camera2D>("Camera2D");
		_camera.LimitLeft = Mathf.RoundToInt(left);
		_camera.LimitRight = Mathf.RoundToInt(right);
		_camera.LimitTop = Mathf.RoundToInt(top);
		_camera.LimitBottom = Mathf.RoundToInt(bottom);
	}

	public void TeleportTo(Vector2 worldPosition)
	{
		GlobalPosition = worldPosition;
		Velocity = Vector2.Zero;
	}

	private void OnDied()
	{
		GD.Print("Player died");
		SetPhysicsProcess(false);
	}
}
