using Godot;

public partial class InteractionPanelContainer : Panel
{
	private PanelContainer _contentContainer;
	private IInteractablePanel _activePanel;

	public override void _Ready()
	{
		_contentContainer = GetNode<PanelContainer>("ContentContainer");
		Visible = false;
		MouseFilter = MouseFilterEnum.Pass;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_cancel") && Visible)
			ClosePanel();
	}

	public void OpenPanel(PackedScene panelScene, ItemData? context)
	{
		foreach (var child in _contentContainer.GetChildren())
		{
			child.QueueFree();
		}

		var panel = panelScene.Instantiate();
		_contentContainer.AddChild(panel);

		if (panel is IInteractablePanel interactablePanel)
		{
			_activePanel = interactablePanel;
			_activePanel.Closed += ClosePanel;
			_activePanel.Open(context);
		}

		Visible = true;
	}

	public void ClosePanel()
	{
		if (_activePanel != null)
		{
			_activePanel.Closed -= ClosePanel;
			_activePanel.Close();
			_activePanel = null;
		}

		foreach (var child in _contentContainer.GetChildren())
		{
			child.QueueFree();
		}

		Visible = false;
	}
}
