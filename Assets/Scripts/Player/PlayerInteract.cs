using System;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interact")]
    [SerializeField] LayerMask interactLayer;
    [SerializeField] float pickupRange = 3f;

    InteractableObject closestInteractable;
    Player player;

    public Action<Transform, string> OnInteractableInRange;
    public Action OnNoInteractableInRange;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        player._input.InputOnInteract += Interact;
        player._movement.OnMove += TryInteract;
    }
    private void OnDisable()
    {
        player._input.InputOnInteract -= Interact;
        player._movement.OnMove -= TryInteract;
    }

    private void TryInteract()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRange, interactLayer, QueryTriggerInteraction.Collide);
        InteractableObject closestObj = null;

        float closest = float.MaxValue;
        foreach (var collider in colliders)
        {
            if (!collider.TryGetComponent(out InteractableObject interactableObject))
            {
                interactableObject = collider.GetComponentInParent<InteractableObject>();

                if (interactableObject == null)
                    continue;
            }
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < closest)
            {
                closest = distance;
                closestObj = interactableObject;
            }
        }
        if (closestInteractable != null && closestObj == null) OnNoInteractableInRange?.Invoke();
        else if(closestObj != null) OnInteractableInRange?.Invoke(closestObj.transform, closestObj.actionName);
        closestInteractable = closestObj;
    }
    private void Interact()
    {
        if(closestInteractable.TryGetComponent(out DroppedItem droppedItem))
        {
            TryPickUp(droppedItem);
        }
        Debug.Log($"Can interact: {closestInteractable?.CanInteract(gameObject) ?? false}");
    }

    private void TryPickUp(DroppedItem droppedItem)
    {
        Debug.Log($"Found dropped item: {droppedItem?.name ?? "null"}");
        droppedItem.Interact(gameObject);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
