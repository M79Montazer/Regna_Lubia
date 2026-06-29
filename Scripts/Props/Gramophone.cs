using Godot;

public partial class Gramophone : Area2D, IInteractable
{
	[Export] public string LabelText { get; set; } = "Gramophone";
	[Export] public PackedScene GramophonePanelScene { get; set; }
	[Export] public string RequiredFlag { get; set; } = "";

	public string GetPromptText(PlayerController player)
	{
		var state = GameStateLocator.Find(this);
		if (state == null)
			return $"Examine {LabelText}";

		if (!string.IsNullOrEmpty(RequiredFlag) && !state.GetFlag(RequiredFlag))
			return "";

		if (state.GetFlag("brother.well"))
			return $"{LabelText} (played)";

		return $"Examine {LabelText}";
	}

	public bool CanInteract(PlayerController player)
	{
		if (string.IsNullOrEmpty(RequiredFlag))
			return true;

		var state = GameStateLocator.Find(this);
		if (state == null)
			return true;

		return state.GetFlag(RequiredFlag);
	}

	public void Interact(PlayerController player)
	{
		if (GramophonePanelScene == null)
		{
			GD.PrintErr("GramophonePanel Scene is not assigned.");
			return;
		}

		var hud = GetTree().GetFirstNodeInGroup("hud") as Hud;
		if (hud == null)
		{
			GD.PrintErr("Hud not found");
			return;
		}

		hud.OpenInteractionPanel(GramophonePanelScene, null);
	}
}