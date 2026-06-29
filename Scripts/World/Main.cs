using Godot;

public partial class Main : Node2D
{
	[Export] public NodePath PlayerPath { get; set; }
	[Export] public NodePath SpawnPath { get; set; }

	public override void _Ready()
	{
		var player = GetNode<PlayerController>(PlayerPath);
		var spawn = GetNode<Marker2D>(SpawnPath);

		player.GlobalPosition = spawn.GlobalPosition;
	}
}
