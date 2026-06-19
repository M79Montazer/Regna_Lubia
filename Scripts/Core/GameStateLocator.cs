using Godot;

public static class GameStateLocator
{
	public static GameState Find(Node from)
	{
		if (GameState.Instance != null)
			return GameState.Instance;

		var found = from?.GetTree()?.GetFirstNodeInGroup("game_state") as GameState;
		return found;
	}

	public static GameState Require(Node from)
	{
		var state = Find(from);
		if (state == null)
			GD.PushError(
				"GameState not found. Add GameState.cs as an Autoload (named 'GameState') " +
				"in Project > Project Settings > Autoload, or place a GameState node in the main scene."
			);
		return state;
	}
}
