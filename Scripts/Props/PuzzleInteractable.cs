using Godot;

public partial class PuzzleInteractable : Area2D, IInteractable
{
	[Export] public PackedScene PanelScene { get; set; }
	[Export] public string LabelText { get; set; } = "Examine";

	public string GetPromptText(PlayerController player)
	{
		return $"Examine {LabelText}";
	}

	public bool CanInteract(PlayerController player)
	{
		return PanelScene != null;
	}

	public void Interact(PlayerController player)
	{
		if (PanelScene == null)
			return;

		var hud = GetTree().GetFirstNodeInGroup("hud") as Hud;
		hud?.OpenInteractionPanel(PanelScene, null);
	}
}