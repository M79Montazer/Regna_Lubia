using System;
using Godot;

public partial class BookPanel : PanelContainer, IInteractablePanel
{
	private Label _titleLabel;
	private RichTextLabel _contentLabel;
	private Button _closeButton;

	public event Action Closed;

	public override void _Ready()
	{
		_titleLabel = GetNode<Label>("VBoxContainer/TitleLabel");
		_contentLabel = GetNode<RichTextLabel>("VBoxContainer/ContentLabel");
		_closeButton = GetNode<Button>("VBoxContainer/CloseButton");

		_closeButton.Pressed += () => Closed?.Invoke();
	}

	public void Open(ItemData? context)
	{
		_titleLabel.Text = context?.DisplayName ?? "";

		if (context is ReadableItemData readable)
			_contentLabel.Text = readable.ReadableText;
		else
			_contentLabel.Text = "";
	}

	public void Close()
	{
		Closed?.Invoke();
	}
}
