using System;
using System.Collections.Generic;
using Godot;

public partial class Inventory : Node
{
	private readonly HashSet<string> _items = new();

	public IReadOnlyCollection<string> Items => _items;

	public event Action<string>? ItemAdded;
	public event Action<string>? ItemRemoved;

	public bool Has(string itemId) => _items.Contains(itemId);

	public void Add(string itemId)
	{
		if (string.IsNullOrWhiteSpace(itemId))
			return;

		if (_items.Add(itemId))
			ItemAdded?.Invoke(itemId);
	}

	public bool Remove(string itemId)
	{
		var removed = _items.Remove(itemId);
		if (removed)
			ItemRemoved?.Invoke(itemId);

		return removed;
	}
}
