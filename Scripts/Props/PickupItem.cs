using Godot;

public partial class PickupItem : Area2D, IInteractable
{
	[Export] public ItemData Item { get; set; }

	public string GetPromptText(PlayerController player)
	{
		return Item == null ? "Pick up item" : $"Pick up {Item.DisplayName}";
	}

	public bool CanInteract(PlayerController player)
	{
		return Item != null;
	}

	public void Interact(PlayerController player)
	{
		if (Item == null)
			return;
		if (Item.CanBePicked)
		{
			if (player.Inventory.AddItem(Item))
				QueueFree();
		}
		else if (Item.PanelScene is not null)
		{
			var hud = GetTree().GetFirstNodeInGroup("hud") as Hud;
			hud?.OpenInteractionPanel(Item.PanelScene, Item);
		}

	}
}
