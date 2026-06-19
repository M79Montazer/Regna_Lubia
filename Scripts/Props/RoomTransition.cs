using Godot;
using Godot.Collections;

public partial class RoomTransition : Area2D, IInteractable
{
	[Export] public string LabelText { get; set; } = "Door";

	[Export] public string TargetRoomScene = "Bedroom";

	[Export] public string TargetSpawnPath = "Spawns/Spawn_From_Door_01";

	[Export] public bool StartsLocked { get; set; } = false;
	[Export] public string RequiredKeyId { get; set; } = "";

	[Export] public string PersistentId { get; set; } = "";
	[Export] public string UnlockFlagKey { get; set; } = "";
	[Export] public Array<StateCondition> UnlockConditions { get; set; } = new();

	private bool _locked;

	public override void _Ready()
	{
		InitializeLockState();
	}

	private void InitializeLockState()
	{
		var state = GameStateLocator.Find(this);

		if (!string.IsNullOrEmpty(PersistentId) && state != null)
		{
			var lockKey = $"door.{PersistentId}.locked";
			if (state.HasFlag(lockKey))
				_locked = state.GetFlag(lockKey);
			else
				_locked = StartsLocked;
		}
		else
		{
			_locked = StartsLocked;
		}

		EvaluateDynamicUnlocks(state);
	}

	private void EvaluateDynamicUnlocks(GameState state)
	{
		if (state == null)
			return;

		if (!string.IsNullOrEmpty(UnlockFlagKey) && state.GetFlag(UnlockFlagKey))
		{
			_locked = false;
			return;
		}

		if (UnlockConditions != null && UnlockConditions.Count > 0)
		{
			bool allMet = true;
			foreach (var condition in UnlockConditions)
			{
				if (condition == null || !condition.Evaluate(state))
				{
					allMet = false;
					break;
				}
			}
			if (allMet)
				_locked = false;
		}
	}

	public string GetPromptText(PlayerController player)
	{
		InitializeLockState();

		if (!_locked)
			return $"Use {LabelText}";

		if (!string.IsNullOrWhiteSpace(RequiredKeyId) && HasMatchingKey(player))
			return $"Unlock {LabelText}";

		return $"{LabelText} (locked)";
	}

	public bool CanInteract(PlayerController player)
	{
		return true;
	}

	public void Interact(PlayerController player)
	{
		InitializeLockState();

		if (_locked)
		{
			if (!string.IsNullOrWhiteSpace(RequiredKeyId) && HasMatchingKey(player))
			{
				_locked = false;
				GD.Print("player used " + RequiredKeyId + " to open the lock");

				var state = GameStateLocator.Find(this);
				if (state != null && !string.IsNullOrEmpty(PersistentId))
					state.SetFlag($"door.{PersistentId}.locked", false);
			}
			else
			{
				GD.Print("door is locked");
				return;
			}
		}

		var roomManager = GetTree().GetFirstNodeInGroup("room_manager") as RoomManager;
		if (roomManager == null)
			return;

		if (TargetRoomScene == null)
			return;

		roomManager.LoadRoom(RoomManager.GetSceneFromId(TargetRoomScene), TargetSpawnPath);
	}

	private bool HasMatchingKey(PlayerController player)
	{
		return player.Inventory.GetSelectedItem() is KeyItemData key && key.KeyId == RequiredKeyId;
	}
}
