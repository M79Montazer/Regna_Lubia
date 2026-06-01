using Godot;

public partial class PlayerCombat : Node2D
{
	[Export] public int MeleeDamage { get; set; } = 25;
	[Export] public float MeleeActiveTime { get; set; } = 0.08f;
	[Export] public float ProjectileSpawnOffset { get; set; } = 24f;
	[Export] public float ProjectileCooldown { get; set; } = 0.35f;
	[Export] public PackedScene ProjectileScene { get; set; } = null!;

	private PlayerController _player = null!;
	private Area2D _meleeHitbox = null!;
	private CollisionShape2D _meleeShape = null!;
	private Timer _meleeCooldownTimer = null!;
	private Timer _rangedCooldownTimer = null!;

	private bool _facingRight = true;
	private bool _meleeActive = false;

	public override void _Ready()
	{
		_player = GetParent<PlayerController>();

		_meleeHitbox = GetNode<Area2D>("MeleeHitbox");
		_meleeShape = _meleeHitbox.GetNode<CollisionShape2D>("CollisionShape2D");
		_meleeCooldownTimer = GetNode<Timer>("MeleeCooldownTimer");
		_rangedCooldownTimer = GetNode<Timer>("RangedCooldownTimer");

		_meleeHitbox.BodyEntered += OnMeleeBodyEntered;
		_meleeShape.Disabled = true;

		SetFacingRight(true);
	}

	public void SetFacingRight(bool facingRight)
	{
		_facingRight = facingRight;

		var pos = _meleeHitbox.Position;
		pos.X = Mathf.Abs(pos.X) * (_facingRight ? 1 : -1);
		_meleeHitbox.Position = pos;
	}

	public void TryMeleeAttack()
	{
		if (!_meleeCooldownTimer.IsStopped())
			return;

		_meleeCooldownTimer.Start(0.25);
		ActivateMeleeHitbox();
	}

	public void TryRangedAttack()
	{
		if (!_rangedCooldownTimer.IsStopped())
			return;

		if (ProjectileScene == null)
			return;

		_rangedCooldownTimer.Start(ProjectileCooldown);

		var projectile = ProjectileScene.Instantiate<Projectile>();
		projectile.GlobalPosition = _player.GlobalPosition + new Vector2(_facingRight ? ProjectileSpawnOffset : -ProjectileSpawnOffset, -4f);
		projectile.SetDirection(_facingRight ? Vector2.Right : Vector2.Left);

		GetTree().CurrentScene.AddChild(projectile);
	}

	private async void ActivateMeleeHitbox()
	{
		_meleeActive = true;
		_meleeShape.Disabled = false;

		await ToSignal(GetTree().CreateTimer(MeleeActiveTime), SceneTreeTimer.SignalName.Timeout);

		_meleeShape.Disabled = true;
		_meleeActive = false;
	}

	private void OnMeleeBodyEntered(Node2D body)
	{
		if (!_meleeActive)
			return;

		var health = body.GetNodeOrNull<Health>("Health");
		health?.Damage(MeleeDamage);
	}
}
