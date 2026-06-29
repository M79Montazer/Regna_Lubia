using Godot;

public partial class DisplayGlass : Area2D, IInteractable
{
	[Export] public string LabelText { get; set; } = "Display Glass";
	[Export] public string InitialText { get; set; } = "Behind the glass, four vinyl discs sit on velvet cushions. Each has a distinct color: red, blue, green, and purple.";
	[Export] public string EmptyText { get; set; } = "The glass case is now empty.";

	[Export] public VinylRecordItemData[] Discs { get; set; } = [];

	public string GetPromptText(PlayerController player)
	{
		var state = GameStateLocator.Find(this);
		if (state != null && (state.GetFlag("discs_collected") ?? false))
			return $"{LabelText} (empty)";
		return $"Examine {LabelText}";
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

		if ((state.GetFlag("discs_collected") ?? false))
		{
			ShowBookPanel(LabelText, EmptyText);
			return;
		}

		if (!(state.GetFlag("dream_visited") ?? false))
		{
			ShowBookPanel(LabelText, InitialText);
			return;
		}

		foreach (var disc in Discs)
		{
			if (disc == null || string.IsNullOrEmpty(disc.ItemId))
				continue;

			bool alreadyHas = false;
			for (int i = 0; i < player.Inventory.SlotCount; i++)
			{
				var slot = player.Inventory.GetSlot(i);
				if (slot != null && slot.ItemId == disc.ItemId)
				{
					alreadyHas = true;
					break;
				}
			}

			if (!alreadyHas)
				player.Inventory.AddItem(disc);
		}

		state.SetFlag("discs_collected", true);
		ShowBookPanel(LabelText, "You take the four vinyl discs from the display case.");
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
