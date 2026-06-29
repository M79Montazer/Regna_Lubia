using System.Reflection;
using Godot;

public partial class Npc : Area2D, IInteractable
{
    [Export] public string NpcName { get; set; } = "NPC";
    [Export] public string RepeatText { get; set; } = "...";
    [Export] public string RecoveredText { get; set; } = "Wow I like that music!";
    [Export] public string ConversationFlag { get; set; } = "brother.talked";
    [Export] public Godot.Collections.Array<string> DialogueLines { get; set; } = [];
    [Export] public Texture2D UnwellTexture { get; set; }
    [Export] public Texture2D WellTexture { get; set; }
    [Export] public string WellFlag { get; set; } = "brother.well";

    private Sprite2D _sprite;
    private GameState _state;

    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("Sprite2D");
        if (!string.IsNullOrEmpty(WellFlag))
        {
            _state = GameStateLocator.Find(this);
            var isWell = _state.GetFlag(WellFlag);
            if (isWell is null)
            {
                _state.SetFlag(WellFlag, true);
                UpdateAppearance(true);
            }
            else
                UpdateAppearance((bool)isWell);

        }
        AddToGroup("brother");
    }

    public void UpdateAppearance(bool isWell)
    {
        if (_sprite == null) return;
        _sprite.Texture = isWell ? WellTexture : UnwellTexture;
    }

    public string GetPromptText(PlayerController player)
    {
        return $"Talk to {NpcName}";
    }

    public bool CanInteract(PlayerController player)
    {
        if (GetTree().GetFirstNodeInGroup("hud") is Hud { IsDialogueActive: true })
            return false;

        return true;
    }

    public void Interact(PlayerController player)
    {
        if (GetTree().GetFirstNodeInGroup("hud") is not Hud hud)
            return;

        
        var isWell = _state.GetFlag(WellFlag) ?? true;
        var hadConversation = _state.GetFlag(ConversationFlag) ?? false;

        switch (isWell)
        {
            case true when !hadConversation:
                hud.StartDialogue(NpcName, [.. DialogueLines], "He is unwell...", ConversationFlag, () =>
                {
                    _state.SetFlag(WellFlag,false);
                    UpdateAppearance(false);
                });
                break;
            case false when hadConversation:
                hud.ShowDialogueText("He is unwell...");
                break;
            case true when hadConversation:
                hud.ShowDialogueText(RecoveredText);
                break;
        }
    }
}