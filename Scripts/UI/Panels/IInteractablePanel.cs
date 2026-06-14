using System;

public interface IInteractablePanel
{
	void Open(ItemData? context);
	void Close();
	event Action Closed;
}