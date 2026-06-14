using Godot;

public partial class ItemData : Resource
{
	[Export] public string ItemId { get; set; } = "";
	[Export] public string DisplayName { get; set; } = "";
	[Export] public Texture2D Icon { get; set; }

	[Export] public PackedScene PanelScene { get; set; }
	[Export] public bool CanBePicked { get; set; }
}
