using Godot;

public partial class HotbarSlot : Button
{
	[Export] public int SlotIndex { get; set; }

	private TextureRect _icon = null!;

	public override void _Ready()
	{
		FocusMode = FocusModeEnum.None;
		_icon = GetNode<TextureRect>("Icon");
	}

	public void SetItem(ItemData? item, bool selected)
	{
		_icon.Texture = item?.Icon;
		TooltipText = item?.DisplayName ?? "";
		Disabled = item == null;

		Modulate = selected
			? new Color(1.2f, 1.2f, 1.2f, 1f)
			: Colors.White;
	}
}
