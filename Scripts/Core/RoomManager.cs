using System;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class RoomManager : Node
{
	[Export] public NodePath RoomContainerPath { get; set; }
	[Export] public NodePath PlayerPath { get; set; }

	[Export] public string InitialRoomScene = "Bedroom";

	[Export] public string InitialSpawnPath = "Spawns/Spawn_Start";

	private Node2D _roomContainer = null!;
	private PlayerController _player = null!;
	private Node2D _currentRoom;

	public static Dictionary<string, string> RoomMap = new()
	{
		{"Bedroom","res://Scenes/World/bedroom.tscn"},
		{"Hallway","res://Scenes/World/hallway.tscn"},

	};


	public override void _Ready()
	{
		AddToGroup("room_manager");

		_roomContainer = GetNode<Node2D>(RoomContainerPath);
		_player = GetNode<PlayerController>(PlayerPath);

		if (InitialRoomScene != null)
		{
			LoadRoom(GetSceneFromId(InitialRoomScene), InitialSpawnPath);
		}
	}

	public void LoadRoom(PackedScene roomScene, NodePath spawnMarkerPath)
	{
		if (roomScene == null)
			return;

		if (_currentRoom != null)
		{
			_currentRoom.QueueFree();
			_currentRoom = null;
		}

		_currentRoom = roomScene.Instantiate<Node2D>();
		_roomContainer.AddChild(_currentRoom);

		var spawn = _currentRoom.GetNodeOrNull<Marker2D>(spawnMarkerPath);
		if (spawn != null)
		{
			_player.TeleportTo(spawn.GlobalPosition);
		}
		else
		{
			GD.PushWarning($"Spawn marker '{spawnMarkerPath}' not found in room '{roomScene.ResourcePath}'.");
		}
	}

	public static PackedScene GetSceneFromId(string id)
	{
		try
		{
			var location = RoomMap.First(a =>string.Equals(a.Key, id)).Value;
			var roomScene = ResourceLoader.Load<PackedScene>(location);
			if (roomScene == null)
				throw new Exception();
			return roomScene;
		}
		catch (Exception e)
		{
			GD.PushError(e);
			return null;
		}
	}
}
