using System;
using System.Collections.Generic;
using Godot;

public partial class GameState : Node
{
    public static GameState Instance { get; private set; }

    private readonly Dictionary<string, bool> _flags = new();
	private readonly Dictionary<string, string> _strings = new();
	private readonly Dictionary<string, int> _ints = new();
	private readonly HashSet<string> _pickedItems = [];

	public event Action<string> StateChanged;
	public event Action<string, bool> FlagChanged;
	public event Action<string, string> StringChanged;
	public event Action<string, int> IntChanged;
	public event Action<string> WorldItemPicked;

	public override void _Ready()
	{
		if (Instance != null)
		{
			GD.PushWarning("Duplicate GameState detected. Keeping existing instance.");
			QueueFree();
			return;
		}
		Instance = this;
		AddToGroup("game_state");
	}

	public bool GetFlag(string key, bool defaultValue = false)
	{
		if (string.IsNullOrEmpty(key))
		{
			GD.PushWarning("GameState.GetFlag called with null/empty key.");
			return defaultValue;
		}
		return _flags.GetValueOrDefault(key, defaultValue);
	}

	public void SetFlag(string key, bool value)
	{
		if (string.IsNullOrEmpty(key))
		{
			GD.PushWarning("GameState.SetFlag called with null/empty key.");
			return;
		}
		_flags[key] = value;
		FlagChanged?.Invoke(key, value);
		StateChanged?.Invoke(key);
	}

	public bool HasFlag(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			GD.PushWarning("GameState.HasFlag called with null/empty key.");
			return false;
		}
		return _flags.ContainsKey(key);
	}

	public string GetString(string key, string defaultValue = "")
	{
		if (string.IsNullOrEmpty(key))
		{
			GD.PushWarning("GameState.GetString called with null/empty key.");
			return defaultValue;
		}
		return _strings.GetValueOrDefault(key, defaultValue);
	}

	public void SetString(string key, string value)
	{
		if (string.IsNullOrEmpty(key))
		{
			GD.PushWarning("GameState.SetString called with null/empty key.");
			return;
		}
		_strings[key] = value ?? "";
		StringChanged?.Invoke(key, value ?? "");
		StateChanged?.Invoke(key);
	}

	public int GetInt(string key, int defaultValue = 0)
	{
		if (string.IsNullOrEmpty(key))
		{
			GD.PushWarning("GameState.GetInt called with null/empty key.");
			return defaultValue;
		}
		return _ints.GetValueOrDefault(key, defaultValue);
	}

	public void SetInt(string key, int value)
	{
		if (string.IsNullOrEmpty(key))
		{
			GD.PushWarning("GameState.SetInt called with null/empty key.");
			return;
		}
		_ints[key] = value;
		IntChanged?.Invoke(key, value);
		StateChanged?.Invoke(key);
	}

	public bool IsWorldItemPicked(string persistentId)
	{
		if (string.IsNullOrEmpty(persistentId))
		{
			GD.PushWarning("GameState.IsWorldItemPicked called with null/empty persistentId.");
			return false;
		}
		return _pickedItems.Contains(persistentId);
	}

	public void MarkWorldItemPicked(string persistentId)
	{
		if (string.IsNullOrEmpty(persistentId))
		{
			GD.PushWarning("GameState.MarkWorldItemPicked called with null/empty persistentId.");
			return;
		}
		_pickedItems.Add(persistentId);
		WorldItemPicked?.Invoke(persistentId);
		StateChanged?.Invoke(persistentId);
	}
}
