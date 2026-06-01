using Godot;

public partial class PickupItem : Area2D, IInteractable
{
	[Export] public string ItemId { get; set; } = "rusty_key";
	[Export] public string ItemName { get; set; } = "Rusty Key";

	public string PromptText => $"Pick up {ItemName}";

	public bool CanInteract(PlayerController player) => true;

	public void Interact(PlayerController player)
	{
		player.Inventory.Add(ItemId);
		GD.Print("player picked a "+ ItemId);
		QueueFree();
	}
}
