using System;
using Godot;

public partial class GramophonePanel : PanelContainer, IInteractablePanel
{
	[Export] public string CorrectDiscId { get; set; } = "disc_blue";
	[Export] public string ReturnRoomId { get; set; } = "Workroom";
	[Export] public string ReturnSpawnPath { get; set; } = "Spawns/Spawn_Start";

	private Button _slotButton;
	private TextureRect _slotIcon;
	private Button _playButton;
	private Button _closeButton;
	private Label _feedbackLabel;

	public event Action Closed;

	public override void _Ready()
	{
		_slotButton = GetNode<Button>("VBoxContainer/SlotButton");
		_slotIcon = GetNode<TextureRect>("VBoxContainer/SlotButton/SlotIcon");
		_playButton = GetNode<Button>("VBoxContainer/PlayButton");
		_closeButton = GetNode<Button>("VBoxContainer/CloseButton");
		_feedbackLabel = GetNode<Label>("VBoxContainer/FeedbackLabel");

		_slotButton.Pressed += OnSlotPressed;
		_playButton.Pressed += OnPlayPressed;
		_closeButton.Pressed += OnClosePressed;
	}

	public void Open(ItemData context)
	{
		var state = GameStateLocator.Find(this);
		if (state == null)
			return;

		_feedbackLabel.Text = "";
		UpdateDisplay(state);
	}

	private void UpdateDisplay(GameState state)
	{
		string currentDisc = state.GetString("gramophone.current_disc", "");

		if (string.IsNullOrEmpty(currentDisc))
		{
			_slotIcon.Texture = null;
			_playButton.Disabled = true;
		}
		else
		{
			var discData = LoadDiscData(currentDisc);
			if (discData != null)
				_slotIcon.Texture = discData.Icon;
			else
				_slotIcon.Texture = null;

			bool alreadyPlayed = state.GetFlag("brother.well");
			_playButton.Disabled = alreadyPlayed;
		}
	}

	private void OnSlotPressed()
	{
		var state = GameStateLocator.Require(this);
		if (state == null)
			return;

		var player = GetTree().GetFirstNodeInGroup("player") as PlayerController;
		if (player == null)
			return;

		string currentDisc = state.GetString("gramophone.current_disc", "");

		if (string.IsNullOrEmpty(currentDisc))
		{
			var selected = player.Inventory.GetSelectedItem();
			if (selected == null)
			{
				_feedbackLabel.Text = "Select a disc from your hotbar first.";
				return;
			}

			if (selected is VinylRecordItemData vinyl)
			{
				state.SetString("gramophone.current_disc", vinyl.DiscId);
				player.Inventory.ClearSelection();

				for (int i = 0; i < player.Inventory.SlotCount; i++)
				{
					if (player.Inventory.GetSlot(i) == selected)
					{
						player.Inventory.RemoveItemAt(i);
						break;
					}
				}

				_feedbackLabel.Text = $"Inserted: {selected.DisplayName}";
			}
			else
			{
				_feedbackLabel.Text = "That's not a vinyl record!";
				return;
			}
		}
		else
		{
			var discData = LoadDiscData(currentDisc);
			if (discData == null)
			{
				_feedbackLabel.Text = "Error: disc data not found.";
				return;
			}

			if (!player.Inventory.AddItem(discData))
			{
				_feedbackLabel.Text = "Inventory is full!";
				return;
			}

			state.SetString("gramophone.current_disc", "");
			_playButton.Disabled = true;
			_feedbackLabel.Text = $"Took: {discData.DisplayName}";
		}

		UpdateDisplay(state);
	}

	private VinylRecordItemData LoadDiscData(string discId)
	{
		string color = discId.Replace("disc_", "");
		return ResourceLoader.Load<VinylRecordItemData>($"res://Resources/vinyl_disc_{color}.tres");
	}

	private void OnPlayPressed()
	{
		var state = GameStateLocator.Find(this);
		if (state == null)
			return;

		string currentDisc = state.GetString("gramophone.current_disc", "");

		if (string.IsNullOrEmpty(currentDisc))
		{
			_feedbackLabel.Text = "Nothing to play.";
			return;
		}

		if (currentDisc == CorrectDiscId)
		{
			state.SetFlag("brother.well", true);
			_playButton.Disabled = true;
			_feedbackLabel.Text = "The melody fills the room... Brother is recovering!";
		}
		else
		{
			_feedbackLabel.Text = "The music plays, but nothing happens...";
		}
	}

	private void OnClosePressed()
	{
		var state = GameStateLocator.Require(this);

		bool wasFirstClose = state != null && !state.GetFlag("gramophone.examined");
		state?.SetFlag("gramophone.examined", true);

		Closed?.Invoke();

		if (wasFirstClose)
		{
			var roomManager = GetTree().GetFirstNodeInGroup("room_manager") as RoomManager;
			roomManager?.LoadRoom(RoomManager.GetSceneFromId(ReturnRoomId), ReturnSpawnPath);
		}
	}

	public void Close()
	{
	}
}
