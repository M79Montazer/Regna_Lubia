using System;
using Godot;

public partial class CombinationLockPanel : PanelContainer, IInteractablePanel
{
	[Export] public string CorrectCode { get; set; }

	private Label[] _digits = new Label[4];
	private Button _openButton;

	public event Action Closed;

	public override void _Ready()
	{
		for (int i = 0; i < 4; i++)
		{
			_digits[i] = GetNode<Label>($"VBoxContainer/HBoxContainer/Digit{i}/Value");

			int captured = i;
			GetNode<Button>($"VBoxContainer/HBoxContainer/Digit{i}/+").Pressed += () => OnDigitUp(captured);
			GetNode<Button>($"VBoxContainer/HBoxContainer/Digit{i}/-").Pressed += () => OnDigitDown(captured);
		}

		_openButton = GetNode<Button>("VBoxContainer/OpenButton");
		_openButton.Pressed += OnOpenPressed;
	}

	public void Open(ItemData? context)
	{
		if (context is CombinationLockItemData lockData)
			CorrectCode = lockData.CorrectCode;

		for (int i = 0; i < 4; i++)
			_digits[i].Text = "0";
	}

	public void Close()
	{
		Closed?.Invoke();
	}

	public void OnDigitUp(int index)
	{
		var val = int.Parse(_digits[index].Text);
		val = (val + 1) % 10;
		_digits[index].Text = val.ToString();
	}

	public void OnDigitDown(int index)
	{
		var val = int.Parse(_digits[index].Text);
		val = (val + 9) % 10;
		_digits[index].Text = val.ToString();
	}

	private void OnOpenPressed()
	{
		var entered = "";
		for (int i = 0; i < 4; i++)
			entered += _digits[i].Text;

		if (entered == CorrectCode)
		{
			GD.Print("Combination correct!");
		}
		else
		{
			GD.Print(entered + ":" + CorrectCode);
			GD.Print("Wrong combination.");
		}

		Closed?.Invoke();
	}
}
