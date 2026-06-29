using System;
using Godot;

public partial class Hud : CanvasLayer
{
	[Export] public NodePath PlayerPath { get; set; }
	[Export] public PackedScene HotbarSlotScene { get; set; }

	private PlayerController _player;
	private HBoxContainer _hotbarSlots;
	private Label _promptLabel;
	private Label _healthLabel;
	private Label _dialogueLabel;
	private InteractionPanelContainer _interactionPanel;

	private HotbarSlot[] _slots;

	private bool _dialogueActive;
	private string[] _dialogueLines;
	private string _dialogueNpcName;
	private int _dialogueIndex;
	private string _dialogueRepeatText;
	private string _dialogueCompleteFlag;
	private bool _showingRepeat;

	public bool IsDialogueActive => _dialogueActive;

	public override void _Ready()
	{
		AddToGroup("hud");

		_player = GetNode<PlayerController>(PlayerPath);

		_hotbarSlots = GetNode<HBoxContainer>("Panel/BottomBar/MarginContainer/HotbarSlots");
		_promptLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/PromptLabel");
		_healthLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/HealthLabel");
		_interactionPanel = GetNode<InteractionPanelContainer>("InteractionPanelContainer");
		_dialogueLabel = GetNode<Label>("DialogueLabel");

		_dialogueLabel.Visible = false;

		BuildHotbar();

		_player.Health.Changed += OnHealthChanged;
		_player.Inventory.InventoryChanged += RefreshHotbar;
		_player.Inventory.SelectionChanged += OnSelectionChanged;
		_player.InteractionDetector.TargetChanged += _ => UpdatePrompt();

		OnHealthChanged(_player.Health.CurrentHealth, _player.Health.MaxHealth);
		RefreshHotbar();
		UpdatePrompt();
	}

	public override void _Input(InputEvent @event)
	{
		if (_dialogueActive && @event.IsActionPressed("ui_cancel"))
		{
			GetViewport().SetInputAsHandled();
			AdvanceDialogue();
		}
	}

	public void OpenInteractionPanel(PackedScene panelScene, ItemData context)
	{
		_interactionPanel.OpenPanel(panelScene, context);
	}

	public void StartDialogue(string npcName, string[] lines, string repeatText, string completeFlag)
	{
		if (lines == null || lines.Length == 0)
			return;

		if (_dialogueActive && _dialogueNpcName == npcName)
			return;

		_dialogueNpcName = npcName;
		_dialogueLines = lines;
		_dialogueRepeatText = repeatText;
		_dialogueCompleteFlag = completeFlag;
		_dialogueIndex = 0;
		_showingRepeat = false;

		_dialogueLabel.Text = $"{npcName}: {lines[0]}";
		_dialogueLabel.Visible = true;
		_dialogueActive = true;
	}

	public void ShowDialogueText(string text)
	{
		_dialogueActive = true;
		_showingRepeat = true;
		_dialogueLines = null;
		_dialogueLabel.Text = text;
		_dialogueLabel.Visible = true;
	}

	private void AdvanceDialogue()
	{
		if (_showingRepeat)
		{
			CloseDialogue();
			return;
		}

		_dialogueIndex++;

		if (_dialogueIndex >= _dialogueLines.Length)
		{
			if (!string.IsNullOrEmpty(_dialogueRepeatText))
			{
				_dialogueLabel.Text = _dialogueRepeatText;
				_showingRepeat = true;
			}
			else
			{
				CloseDialogue();
			}
		}
		else
		{
			_dialogueLabel.Text = $"{_dialogueNpcName}: {_dialogueLines[_dialogueIndex]}";
		}
	}

	private void CloseDialogue()
	{
		_dialogueLabel.Visible = false;
		_dialogueActive = false;

		if (!string.IsNullOrEmpty(_dialogueCompleteFlag))
		{
			var state = GameStateLocator.Find(this);
			if (state != null)
				state.SetFlag(_dialogueCompleteFlag, true);
		}
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

	private void OnSelectionChanged(int index, ItemData item)
	{
		RefreshHotbar();
		UpdatePrompt();

		if (item is VinylRecordItemData vinyl
			&& _interactionPanel.IsOpen
			&& _interactionPanel.ActivePanel is GramophonePanel gp)
		{
			gp.InsertDisc(vinyl);
			_player.Inventory.ClearSelection();
			RefreshHotbar();
			return;
		}

		if (item?.PanelScene != null)
		{
			OpenInteractionPanel(item.PanelScene, item);
		}
		else
			_interactionPanel.ClosePanel();
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
