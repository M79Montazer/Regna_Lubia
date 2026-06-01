using System;
using Godot;

public partial class Health : Node
{
	[Export] public int MaxHealth { get; set; } = 100;

	public int CurrentHealth { get; private set; }

	public event Action<int, int>? Changed;
	public event Action? Died;

	public override void _Ready()
	{
		CurrentHealth = MaxHealth;
		Changed?.Invoke(CurrentHealth, MaxHealth);
	}

	public void Damage(int amount)
	{
		if (amount <= 0 || CurrentHealth <= 0)
			return;

		CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);
		Changed?.Invoke(CurrentHealth, MaxHealth);

		if (CurrentHealth == 0)
			Died?.Invoke();
	}

	public void Heal(int amount)
	{
		if (amount <= 0 || CurrentHealth <= 0)
			return;

		CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
		Changed?.Invoke(CurrentHealth, MaxHealth);
	}
}
