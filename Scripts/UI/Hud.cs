using Godot;

public partial class Hud : CanvasLayer
{
	[Export] public NodePath PlayerPath { get; set; }

	private Label _promptLabel = null!;
	private Label _healthLabel = null!;
	private Label _inventoryLabel = null!;
	private PlayerController _player = null!;

	public override void _Ready()
	{
		_promptLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/PromptLabel");
		_healthLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/HealthLabel");
		_inventoryLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/InventoryLabel");

		_player = GetNode<PlayerController>(PlayerPath);

		_player.Health.Changed += OnHealthChanged;
		_player.Inventory.ItemAdded += OnInventoryChanged;
		_player.Inventory.ItemRemoved += OnInventoryChanged;
		_player.InteractionDetector.TargetChanged += OnTargetChanged;

		OnHealthChanged(_player.Health.CurrentHealth, _player.Health.MaxHealth);
		OnInventoryChanged("");
		OnTargetChanged(_player.InteractionDetector.CurrentInteractable);
	}

	private void OnHealthChanged(int current, int max)
	{
		_healthLabel.Text = $"HP: {current}/{max}";
	}

	private void OnInventoryChanged(string _)
	{
		_inventoryLabel.Text = _player.Inventory.Items.Count == 0
			? "Inventory: -"
			: "Inventory: " + string.Join(", ", _player.Inventory.Items);
	}

	private void OnTargetChanged(IInteractable? target)
	{
		_promptLabel.Text = target == null ? "" : $"E - {target.PromptText}";
	}
}
