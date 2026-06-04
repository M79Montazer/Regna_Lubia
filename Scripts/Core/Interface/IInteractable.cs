public interface IInteractable
{
    string GetPromptText(PlayerController player);
    bool CanInteract(PlayerController player);
    void Interact(PlayerController player);
}