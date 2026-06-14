using Godot;

[GlobalClass]
public partial class KeyItemData : ItemData
{
	[Export] public string KeyId { get; set; } = "";
}