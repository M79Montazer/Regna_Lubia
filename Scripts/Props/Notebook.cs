using Godot;

public partial class Notebook : Area2D, IInteractable
{
	[Export] public string LabelText { get; set; } = "Notebook";
	[Export] public string CorrectCode { get; set; } = "1234";
	[Export] public string LockId { get; set; } = "notebook1";
	[Export] public string InitialText { get; set; } = "";
	[Export] public string DreamBedroomScene { get; set; } = "dream_bedroom";
	[Export] public string DreamSpawnPath { get; set; } = "Spawns/Spawn_Start";

	public string GetPromptText(PlayerController player)
	{
		var state = GameStateLocator.Find(this);
		if (state == null)
			return $"Open {LabelText}";

		if (!(state.GetFlag($"lock.{LockId}.unlocked")??false))
			return $"Open {LabelText} (locked)";

		return $"Open {LabelText}";
	}

	public bool CanInteract(PlayerController player)
	{
		return true;
	}

	public void Interact(PlayerController player)
	{
		var state = GameStateLocator.Find(this);
		if (state == null)
			return;

		if (!(state.GetFlag($"lock.{LockId}.unlocked") ?? false))
		{
			var hud = GetTree().GetFirstNodeInGroup("hud") as Hud;
			var panelScene = GD.Load<PackedScene>("res://Scenes/UI/Panels/CombinationLockPanel.tscn");
			if (hud != null && panelScene != null)
			{
				var lockData = new CombinationLockItemData
				{
					CorrectCode = CorrectCode,
					LockId = LockId,
					DisplayName = LabelText
				};
				hud.OpenInteractionPanel(panelScene, lockData);
			}
			return;
		}

		if (!(state.GetFlag("dream_visited") ?? false))
		{
			state.SetFlag("dream_visited", true);
			var roomManager = GetTree().GetFirstNodeInGroup("room_manager") as RoomManager;
			roomManager?.LoadRoom(RoomManager.GetSceneFromId(DreamBedroomScene), DreamSpawnPath);
			return;
		}

		if (!(state.GetFlag("discs_collected") ?? false))
		{
			ShowBookPanel(LabelText, "I should examine the display case first...");
			return;
		}

		var roomManager2 = GetTree().GetFirstNodeInGroup("room_manager") as RoomManager;
		roomManager2?.LoadRoom(RoomManager.GetSceneFromId(DreamBedroomScene), DreamSpawnPath);
	}

	private void ShowBookPanel(string title, string text)
	{
		var hud = GetTree().GetFirstNodeInGroup("hud") as Hud;
		var panelScene = GD.Load<PackedScene>("res://Scenes/UI/Panels/BookPanel.tscn");
		if (hud != null && panelScene != null)
		{
			var data = new ReadableItemData
			{
				DisplayName = title,
				ReadableText = text
			};
			hud.OpenInteractionPanel(panelScene, data);
		}
	}
}