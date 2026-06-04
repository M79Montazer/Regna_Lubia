using System;
using Godot;

public partial class Inventory : Node
{
	[Export] public int SlotCount { get; set; } = 8;

	private ItemData?[] _slots = Array.Empty<ItemData?>();
	public int SelectedSlotIndex { get; private set; } = -1;

	public event Action? InventoryChanged;
	public event Action<int, ItemData?>? SelectionChanged;

	public override void _Ready()
	{
		if (SlotCount < 1)
			SlotCount = 8;

		_slots = new ItemData?[SlotCount];
	}

	public ItemData? GetSlot(int index)
	{
		if (index < 0 || index >= _slots.Length)
			return null;

		return _slots[index];
	}

	public ItemData? GetSelectedItem()
	{
		return SelectedSlotIndex >= 0 ? _slots[SelectedSlotIndex] : null;
	}

	public bool AddItem(ItemData item)
	{
		if (item == null)
			return false;

		for (int i = 0; i < _slots.Length; i++)
		{
			if (_slots[i] == null)
			{
				_slots[i] = item;
				InventoryChanged?.Invoke();
				return true;
			}
		}

		GD.Print("Inventory full.");
		return false;
	}

	public bool SelectSlot(int index)
	{
		GD.Print("pressed");
		if (index < 0 || index >= _slots.Length)
			return false;

		if (_slots[index] == null)
			return false;

		if (SelectedSlotIndex == index)
			return true;

		SelectedSlotIndex = index;
		SelectionChanged?.Invoke(index, _slots[index]);
		return true;
	}

	public void ClearSelection()
	{
		if (SelectedSlotIndex == -1)
			return;

		SelectedSlotIndex = -1;
		SelectionChanged?.Invoke(-1, null);
	}

	public bool HasSelectedKey(string requiredKeyId)
	{
		var item = GetSelectedItem();
		return item != null
			   && item.Type == ItemType.Key
			   && item.KeyId == requiredKeyId;
	}
}
