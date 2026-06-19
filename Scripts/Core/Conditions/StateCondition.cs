using Godot;

[GlobalClass]
public partial class StateCondition : Resource
{
	public enum ValueType
	{
		Flag,
		String,
		Int
	}

	public enum OperatorKind
	{
		Equals,
		NotEquals,
		GreaterThan,
		GreaterOrEqual,
		LessThan,
		LessOrEqual,
		Exists,
		NotExists
	}

	[Export] public string Key { get; set; } = "";
	[Export] public ValueType Type { get; set; } = ValueType.Flag;
	[Export] public OperatorKind Operator { get; set; } = OperatorKind.Equals;
	[Export] public bool ExpectedFlag { get; set; } = true;
	[Export] public string ExpectedString { get; set; } = "";
	[Export] public int ExpectedInt { get; set; } = 0;

	public bool Evaluate(GameState state)
	{
		if (state == null)
		{
			GD.PushWarning("StateCondition.Evaluate: GameState is null.");
			return false;
		}

		if (string.IsNullOrEmpty(Key))
		{
			GD.PushWarning("StateCondition.Evaluate: Key is null or empty.");
			return false;
		}

        return Type switch
        {
            ValueType.Flag => EvaluateFlag(state),
            ValueType.String => EvaluateString(state),
            ValueType.Int => EvaluateInt(state),
            _ => false
        };
    }

	private bool EvaluateFlag(GameState state)
	{
		switch (Operator)
		{
			case OperatorKind.Equals:
				return state.GetFlag(Key) == ExpectedFlag;
			case OperatorKind.NotEquals:
				return state.GetFlag(Key) != ExpectedFlag;
			case OperatorKind.Exists:
				return state.HasFlag(Key);
			case OperatorKind.NotExists:
				return !state.HasFlag(Key);
            case OperatorKind.GreaterThan:
            case OperatorKind.GreaterOrEqual:
            case OperatorKind.LessThan:
            case OperatorKind.LessOrEqual:
            default:
				GD.PushWarning($"StateCondition: Operator {Operator} not supported for Flag type on key '{Key}'.");
				return false;
		}
	}

	private bool EvaluateString(GameState state)
	{
		var current = state.GetString(Key);

		switch (Operator)
		{
			case OperatorKind.Equals:
				return string.Equals(current, ExpectedString, System.StringComparison.Ordinal);
			case OperatorKind.NotEquals:
				return !string.Equals(current, ExpectedString, System.StringComparison.Ordinal);
			case OperatorKind.Exists:
				return !string.IsNullOrEmpty(current);
			case OperatorKind.NotExists:
				return string.IsNullOrEmpty(current);
            case OperatorKind.GreaterThan:
            case OperatorKind.GreaterOrEqual:
            case OperatorKind.LessThan:
            case OperatorKind.LessOrEqual:
            default:
				GD.PushWarning($"StateCondition: Operator {Operator} not supported for String type on key '{Key}'.");
				return false;
		}
	}

	private bool EvaluateInt(GameState state)
	{
		var current = state.GetInt(Key);

		switch (Operator)
		{
			case OperatorKind.Equals:
				return current == ExpectedInt;
			case OperatorKind.NotEquals:
				return current != ExpectedInt;
			case OperatorKind.GreaterThan:
				return current > ExpectedInt;
			case OperatorKind.GreaterOrEqual:
				return current >= ExpectedInt;
			case OperatorKind.LessThan:
				return current < ExpectedInt;
			case OperatorKind.LessOrEqual:
				return current <= ExpectedInt;
			case OperatorKind.Exists:
				return state.GetInt(Key, int.MinValue) != int.MinValue || state.HasFlag(Key) || !string.IsNullOrEmpty(state.GetString(Key));
			case OperatorKind.NotExists:
				return !state.HasFlag(Key) && state.GetInt(Key, int.MinValue) == int.MinValue && string.IsNullOrEmpty(state.GetString(Key));
			default:
				GD.PushWarning($"StateCondition: Operator {Operator} not supported for Int type on key '{Key}'.");
				return false;
		}
	}
}
