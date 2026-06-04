using Godot;

public partial class RoomTransition : Area2D, IInteractable
{
	[Export] public string LabelText { get; set; } = "Door";

	[Export] public string TargetRoomScene = "Bedroom";

	[Export] public string TargetSpawnPath = "Spawns/Spawn_From_Door_01";

	[Export] public bool StartsLocked { get; set; } = false;
	[Export] public string RequiredKeyId { get; set; } = "";

	private bool _locked;

	public override void _Ready()
	{
		_locked = StartsLocked;
	}

	public string GetPromptText(PlayerController player)
	{
		if (!_locked)
			return $"Use {LabelText}";

		if (!string.IsNullOrWhiteSpace(RequiredKeyId) && player.Inventory.HasSelectedKey(RequiredKeyId))
			return $"Unlock {LabelText}";

		return $"{LabelText} (locked)";
	}

	public bool CanInteract(PlayerController player)
	{
		return true;
	}

	public void Interact(PlayerController player)
	{
		if (_locked)
		{
			if (!player.Inventory.HasSelectedKey(RequiredKeyId))
			{
				GD.Print("door is locked");
				return;
			}

			_locked = false;
			GD.Print("player used " + RequiredKeyId + " to open the lock");
		}

		if (TargetRoomScene == null)
			return;

		var roomManager = GetTree().GetFirstNodeInGroup("room_manager") as RoomManager;
		roomManager?.LoadRoom(RoomManager.GetSceneFromId(TargetRoomScene), TargetSpawnPath);
	}
}
