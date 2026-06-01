using Godot;

public partial class RoomTransition : Area2D, IInteractable
{
	[Export] public string LabelText { get; set; } = "Door";

	[Export] public string TargetRoomScene = "Bedroom";

	[Export] public string TargetSpawnPath = "Spawns/Spawn_From_Door_01";

	[Export] public bool StartsLocked { get; set; } = false;
	[Export] public string RequiredKeyId { get; set; } = "";
	[Export] public bool ConsumeKeyOnUse { get; set; } = false;

	private bool _locked;

	public override void _Ready()
	{
		_locked = StartsLocked;
	}

	public string PromptText => _locked ? $"{LabelText} (locked)" : $"Use {LabelText}";

	public bool CanInteract(PlayerController player)
	{
		if (!_locked)
			return true;

		if (string.IsNullOrWhiteSpace(RequiredKeyId))
			return false;

		return player.Inventory.Has(RequiredKeyId);
	}

	public void Interact(PlayerController player)
	{
		if (_locked)
		{
			if (string.IsNullOrWhiteSpace(RequiredKeyId) || !player.Inventory.Has(RequiredKeyId))
			{
				GD.Print("player does not have " + RequiredKeyId);
				return;
			}

			_locked = false;

			if (ConsumeKeyOnUse)
				player.Inventory.Remove(RequiredKeyId);
			GD.Print("player used " + RequiredKeyId + " to open the lock");
		}

		if (TargetRoomScene == null)
			return;

		var roomManager = GetTree().GetFirstNodeInGroup("room_manager") as RoomManager;
		roomManager?.LoadRoom(RoomManager.GetSceneFromId(TargetRoomScene), TargetSpawnPath);
	}
}
