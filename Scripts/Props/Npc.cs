using Godot;

public partial class Npc : Area2D, IInteractable
{
	[Export] public string NpcName { get; set; } = "NPC";
	[Export] public string RepeatText { get; set; } = "...";
	[Export] public string ConversationFlag { get; set; } = "";
	[Export] public Godot.Collections.Array<string> DialogueLines { get; set; } = [];
	[Export] public Texture2D UnwellTexture { get; set; }
	[Export] public Texture2D WellTexture { get; set; }
	[Export] public string WellFlag { get; set; } = "";

	private Sprite2D _sprite;

	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>("Sprite2D");
		if (_sprite != null && !string.IsNullOrEmpty(WellFlag))
		{
			var state = GameStateLocator.Find(this);
			if (state != null)
				UpdateAppearance(state.GetFlag(WellFlag));
			else
				UpdateAppearance(false);
		}
	}

	private void UpdateAppearance(bool isWell)
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
		var hud = GetTree().GetFirstNodeInGroup("hud") as Hud;
		if (hud != null && hud.IsDialogueActive)
			return false;

		if (string.IsNullOrEmpty(ConversationFlag))
			return true;

		var state = GameStateLocator.Find(this);
		if (state == null)
			return true;

		if (!state.GetFlag(ConversationFlag))
			return true;

		return !string.IsNullOrEmpty(RepeatText);
	}

	public void Interact(PlayerController player)
	{
		var hud = GetTree().GetFirstNodeInGroup("hud") as Hud;
		if (hud == null)
			return;

		var state = GameStateLocator.Find(this);
		bool completed = state != null && !string.IsNullOrEmpty(ConversationFlag) && state.GetFlag(ConversationFlag);

		if (completed && !string.IsNullOrEmpty(RepeatText))
		{
			hud.ShowDialogueText(RepeatText);
		}
		else if (!completed)
		{
			hud.StartDialogue(NpcName, [.. DialogueLines], RepeatText, ConversationFlag);
		}

		if (!string.IsNullOrEmpty(WellFlag))
		{
			if (state != null)
				UpdateAppearance(state.GetFlag(WellFlag));
		}
	}
}