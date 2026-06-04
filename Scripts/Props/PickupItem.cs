using Godot;

public partial class PickupItem : Area2D, IInteractable
{
	[Export] public ItemData? Item { get; set; }

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

		if (player.Inventory.AddItem(Item))
			QueueFree();
	}
}
