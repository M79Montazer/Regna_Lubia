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
        var bro = this.GetTree().GetFirstNodeInGroup("brother") as Npc;
        if ((state.GetFlag(bro.WellFlag) ?? false))
            return $"{LabelText} (played)";

        return $"Examine {LabelText}";
    }

    public bool CanInteract(PlayerController player)
    {
        var state = GameStateLocator.Find(this);

        return state.GetFlag(RequiredFlag) ?? false;
    }

    public void Interact(PlayerController player)
    {
        if (GramophonePanelScene == null)
        {
            GD.PrintErr("GramophonePanel Scene is not assigned.");
            return;
        }

        if (GetTree().GetFirstNodeInGroup("hud") is not Hud hud)
        {
            GD.PrintErr("Hud not found");
            return;
        }

        hud.OpenInteractionPanel(GramophonePanelScene, null);
    }
}