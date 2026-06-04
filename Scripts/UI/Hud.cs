using Godot;

public partial class Hud : CanvasLayer
{
	[Export] public NodePath PlayerPath { get; set; }
	[Export] public PackedScene? HotbarSlotScene { get; set; }

	private PlayerController _player = null!;

	private HBoxContainer _hotbarSlots = null!;
	private Label _promptLabel = null!;
	private Label _healthLabel = null!;
	private PanelContainer _readablePanel = null!;
	private Label _readableTitle = null!;
	private RichTextLabel _readableText = null!;

	private HotbarSlot[] _slots;

	public override void _Ready()
	{
		_player = GetNode<PlayerController>(PlayerPath);

		_hotbarSlots = GetNode<HBoxContainer>("Panel/BottomBar/MarginContainer/HotbarSlots");
		_promptLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/PromptLabel");
		_healthLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/HealthLabel");
		_readablePanel = GetNode<PanelContainer>("ReadablePanel");
		_readableTitle = GetNode<Label>("ReadablePanel/MarginContainer/VBoxContainer/TitleLabel");
		_readableText = GetNode<RichTextLabel>("ReadablePanel/MarginContainer/VBoxContainer/TextLabel");

		BuildHotbar();

		_player.Health.Changed += OnHealthChanged;
		_player.Inventory.InventoryChanged += RefreshHotbar;
		_player.Inventory.SelectionChanged += OnSelectionChanged;
		_player.InteractionDetector.TargetChanged += _ => UpdatePrompt();

		OnHealthChanged(_player.Health.CurrentHealth, _player.Health.MaxHealth);
		RefreshHotbar();
		UpdateReadablePanel(_player.Inventory.GetSelectedItem());
		UpdatePrompt();
	}

	private void BuildHotbar()
	{
		if (HotbarSlotScene == null)
		{
			GD.PushError("HotbarSlotScene is not assigned on Hud.");
			return;
		}

		_slots = new HotbarSlot[_player.Inventory.SlotCount];

		for (var i = 0; i < _player.Inventory.SlotCount; i++)
		{
			var slot = HotbarSlotScene.Instantiate<HotbarSlot>();
			slot.SlotIndex = i;

			var capturedIndex = i;
			slot.Pressed += () => _player.Inventory.SelectSlot(capturedIndex);

			_hotbarSlots.AddChild(slot);
			_slots[i] = slot;
		}
	}

	private void RefreshHotbar()
	{
		for (var i = 0; i < _slots.Length; i++)
		{
			var item = _player.Inventory.GetSlot(i);
			var selected = _player.Inventory.SelectedSlotIndex == i;
			_slots[i].SetItem(item, selected);
		}
	}

	private void OnSelectionChanged(int index, ItemData? item)
	{
		RefreshHotbar();
		UpdateReadablePanel(item);
		UpdatePrompt();
	}

	private void UpdateReadablePanel(ItemData? item)
	{
		if (item is { Type: ItemType.Readable })
		{
			_readableTitle.Text = item.DisplayName;
			_readableText.Text = item.ReadableText;
			_readablePanel.Visible = true;
		}
		else
		{
			_readablePanel.Visible = false;
		}
	}

	private void OnHealthChanged(int current, int max)
	{
		_healthLabel.Text = $"HP: {current}/{max}";
	}

	private void UpdatePrompt()
	{
		var target = _player.InteractionDetector.CurrentInteractable;
		_promptLabel.Text = target == null ? "" : $"E - {target.GetPromptText(_player)}";
	}
}
