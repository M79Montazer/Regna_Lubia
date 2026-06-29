using Godot;

public partial class PickupItem : Area2D, IInteractable
{
	[Export] public ItemData Item { get; set; }

	[Export] public string PersistentId { get; set; } = "";

	public override void _Ready()
	{
		AddToGroup("pickup_items");

		if (string.IsNullOrEmpty(PersistentId))
			return;

		var state = GameStateLocator.Find(this);
		if (state != null)
		{

			if (state.IsWorldItemPicked(PersistentId))
				QueueFree();
			if (Item is CombinationLockItemData combinationLock)
			{
				var gameFlag = $"lock.{combinationLock.LockId}.unlocked";
				if ((state.GetFlag(gameFlag) ?? false))
				{
					QueueFree();
				}
			}
		}
	}

	public string GetPromptText(PlayerController player)
	{
		return Item == null ? "Pick up item" : $"Pick up {Item.DisplayName}";
	}

	public bool CanInteract(PlayerController player)
	{
		return Item != null && !IsQueuedForDeletion();
	}

	public void Interact(PlayerController player)
	{
		if (Item == null)
			return;

		if (Item.CanBePicked)
		{
			if (string.IsNullOrEmpty(PersistentId))
				GD.PushWarning($"PickupItem '{Item.DisplayName}' has no PersistentId; its state will not persist across room loads.");

			if (player.Inventory.AddItem(Item))
			{
				if (!string.IsNullOrEmpty(PersistentId))
				{
					var state = GameStateLocator.Require(this);
					state?.MarkWorldItemPicked(PersistentId);
				}
				QueueFree();
			}
		}
		else if (Item.PanelScene is not null)
		{
			var hud = GetTree().GetFirstNodeInGroup("hud") as Hud;
			hud?.OpenInteractionPanel(Item.PanelScene, Item);
		}
	}
}
