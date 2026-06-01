public interface IInteractable
{
    string PromptText { get; }
    bool CanInteract(PlayerController player);
    void Interact(PlayerController player);
}