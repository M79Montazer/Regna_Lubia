using Godot;

[GlobalClass]
public partial class VinylRecordItemData : ReadableItemData
{
	[Export] public string DiscId { get; set; } = "";
}
