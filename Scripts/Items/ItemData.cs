using Godot;

public enum ItemType
{
    Key,
    Readable
}

[GlobalClass]
public partial class ItemData : Resource
{
    [Export] public string ItemId { get; set; } = "";
    [Export] public string DisplayName { get; set; } = "";
    [Export] public ItemType Type { get; set; } = ItemType.Key;
    [Export] public Texture2D Icon { get; set; }

    [Export] public string KeyId { get; set; } = "";

    [Export(PropertyHint.MultilineText)]
    public string ReadableText { get; set; } = "";
}