using System;
using Godot;

public partial class InteractionDetector : Area2D
{
	public IInteractable? CurrentInteractable { get; private set; }

	public event Action<IInteractable?>? TargetChanged;

	public override void _Ready()
	{
		Monitoring = true;
		Monitorable = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		var current = FindBestInteractable();

		if (!ReferenceEquals(current, CurrentInteractable))
		{
			CurrentInteractable = current;
			TargetChanged?.Invoke(CurrentInteractable);
		}
	}

	public void TryInteract(PlayerController player)
	{
		CurrentInteractable?.Interact(player);
	}

	private IInteractable? FindBestInteractable()
	{
		var player = GetParent<PlayerController>();

		foreach (var area in GetOverlappingAreas())
		{
			if (area is IInteractable interactable && interactable.CanInteract(player))
				return interactable;
		}

		return null;
	}
}
