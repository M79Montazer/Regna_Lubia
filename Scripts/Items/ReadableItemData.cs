using Godot;

[GlobalClass]
public partial class ReadableItemData : ItemData
{
	[Export(PropertyHint.MultilineText)]
	public string ReadableText { get; set; } = "";
}