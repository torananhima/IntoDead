using System;
using System.Collections.Generic;
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
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            pickupRange,
            interactLayer,
            QueryTriggerInteraction.Collide
        );

        InteractableObject closestObj = null;
        float closestDistance = float.MaxValue;

        HashSet<InteractableObject> checkedObjects = new();

        foreach (Collider collider in colliders)
        {
            if (!collider.TryGetComponent(out InteractableObject interactable))
            {
                interactable = collider.GetComponentInParent<InteractableObject>();

                if (interactable == null)
                    continue;
            }

            if (!checkedObjects.Add(interactable))
                continue;

            float distance = Vector3.Distance(
                transform.position,
                collider.ClosestPoint(transform.position)
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObj = interactable;
            }
        }

        closestInteractable = closestObj;

        if (closestObj == null)
        {
            OnNoInteractableInRange?.Invoke();
            return;
        }

        OnInteractableInRange?.Invoke(
            closestObj.transform,
            closestObj.actionName
        );
    }
    private void Interact()
    {
        if (closestInteractable == null) return;

        closestInteractable.Interact(gameObject);

        Debug.Log($"Can interact: {closestInteractable?.CanInteract(gameObject) ?? false}");
        //droppedItem.PickupCompleted();
        TryInteract();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
