using UnityEngine;

public abstract class InteractableObject : MonoBehaviour, IInteractable
{
    public string actionName = "Interact";
    public abstract bool CanInteract(GameObject interactor);
    public abstract void Interact(GameObject interactor);
}
