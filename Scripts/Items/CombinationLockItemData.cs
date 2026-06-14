using Godot;

[GlobalClass]
public partial class CombinationLockItemData : ItemData
{
	[Export] public string CorrectCode { get; set; } = "0000";
	[Export] public string LockId { get; set; } = "";
}