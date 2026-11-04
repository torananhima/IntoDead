using UnityEngine;
using UnityEngine.Windows;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interact")]

    [SerializeField] float pickupRange = 3f;
    Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        player._input.InputOnInteract += Interact;
    }
    private void OnDisable()
    {
        player._input.InputOnInteract -= Interact;
    }

    private void Interact()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRange);
        foreach (var collider in colliders)
        {
            DroppedItem droppedItem = collider.GetComponent<DroppedItem>();
            Debug.Log($"Found dropped item: {droppedItem?.name ?? "null"}");
            Debug.Log($"Can interact: {droppedItem?.CanInteract(gameObject) ?? false}");
            if (droppedItem != null && droppedItem.CanInteract(gameObject))
            {
                droppedItem.Interact(gameObject);
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
