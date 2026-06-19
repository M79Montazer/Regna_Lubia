using Godot;

public partial class StatefulButton : Area2D, IInteractable
{
	[Export] public string PersistentId { get; set; } = "";
	[Export] public string LabelText { get; set; } = "Button";
	[Export] public string PuzzleId { get; set; } = "";
	[Export] public string Value { get; set; } = "";
	[Export] public bool StartsPressed { get; set; } = false;
	[Export] public bool ToggleMode { get; set; } = false;
	[Export] public bool ExclusiveWithinPuzzle { get; set; } = true;

	[Export] public string PressedFlagKey { get; set; } = "";
	[Export] public string ActiveValueKey { get; set; } = "";
	[Export] public string SolvedFlagKey { get; set; } = "";
	[Export] public string CorrectValue { get; set; } = "";

	private bool _pressed;
	private GameState _state;

	public override void _Ready()
	{
		_state = GameStateLocator.Find(this);

		if (!string.IsNullOrEmpty(PersistentId) && _state != null)
		{
			var key = GetPressedFlagKey();
			_pressed = _state.HasFlag(key) ? _state.GetFlag(key) : StartsPressed;
		}
		else
		{
			_pressed = StartsPressed;
		}
	}

	public string GetPromptText(PlayerController player)
	{
		if (_pressed)
			return $"{LabelText} (pressed)";

		return $"Press {LabelText}";
	}

	public bool CanInteract(PlayerController player)
	{
		return true;
	}

	public void Interact(PlayerController player)
	{
		if (_state == null)
		{
			_state = GameStateLocator.Find(this);
			if (_state == null)
			{
				GD.PrintErr("StatefulButton: GameState not available.");
				return;
			}
		}

		if (ToggleMode)
			_pressed = !_pressed;
		else
			_pressed = true;

		var pressedKey = GetPressedFlagKey();
		_state.SetFlag(pressedKey, _pressed);

		if (_pressed)
		{
			var activeKey = GetActiveValueKey();
			_state.SetString(activeKey, Value ?? "");

			var solvedKey = GetSolvedFlagKey();
			if (!string.IsNullOrEmpty(solvedKey))
			{
				bool solved = !string.IsNullOrEmpty(CorrectValue) && string.Equals(Value, CorrectValue, System.StringComparison.Ordinal);
				_state.SetFlag(solvedKey, solved);
			}
		}
	}

	protected virtual string GetPressedFlagKey()
	{
		if (!string.IsNullOrEmpty(PressedFlagKey))
			return PressedFlagKey;

		if (!string.IsNullOrEmpty(PersistentId))
			return $"button.{PersistentId}.pressed";

		return $"button.{Name}.pressed";
	}

	private string GetActiveValueKey()
	{
		if (!string.IsNullOrEmpty(ActiveValueKey))
			return ActiveValueKey;

		if (!string.IsNullOrEmpty(PuzzleId))
			return $"puzzle.{PuzzleId}.active";

		if (!string.IsNullOrEmpty(PersistentId))
			return $"button.{PersistentId}.active";

		return $"button.{Name}.active";
	}

	private string GetSolvedFlagKey()
	{
		if (!string.IsNullOrEmpty(SolvedFlagKey))
			return SolvedFlagKey;

		if (!string.IsNullOrEmpty(PuzzleId))
			return $"puzzle.{PuzzleId}.solved";

		return "";
	}
}
