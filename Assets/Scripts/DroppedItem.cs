using UnityEngine;

public class DroppedItem : MonoBehaviour, IInteractable
{
    [Header("Debug")]
    [SerializeField] private ItemData itemData;
    [Header("Pickup")]
    [SerializeField] private ItemInstance item;
    [SerializeField] private bool pickupEnabled;
    [SerializeField] private float pickupDelay = 0.25f;

    private float pickupTime;

    private void Start()
    {
        if (itemData == null)
        {
            Debug.LogWarning("DroppedItem has no item assigned.");
            Destroy(gameObject);
            return;
        }
        else
        {
            item = new ItemInstance(itemData, 1);
            Debug.Log($"{item}");
            //ItemInstance itemInstance = new ItemInstance(itemData, 1);
            //Initialize(itemInstance);
        }
    }

    public void Initialize(ItemInstance itemInstance)
    {
        item = itemInstance;
        pickupEnabled = false;
        pickupTime = Time.time + pickupDelay;
    }

    public bool CanInteract(GameObject interactor)
    {
        if (!pickupEnabled && Time.time >= pickupTime)
        {
            pickupEnabled = true;
        }

        Debug.Log($"CanInteract check for {interactor.name}: pickupEnabled={pickupEnabled}, item={(item != null ? item.itemData.name : "null")}");
        Debug.Log($"{item}");
        return pickupEnabled &&
               item != null &&
               interactor != null &&
               interactor.GetComponent<PlayerInventory>() != null;
    }

    public void Interact(GameObject interactor)
    {
        Debug.Log($"Player {interactor.name} is trying to pick up {item.itemData.name}.");
        TryPickup(interactor);
    }

    public bool TryPickup(GameObject interactor)
    {
        if (!CanInteract(interactor))
        {
            return false;
        }

        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();

        if (!inventory.TryAddItem(item))
        {
            return false;
        }

        item.transform.SetParent(inventory.transform);
        Destroy(gameObject);

        return true;
    }

    public void DespawnIfEmpty()
    {
        if (item == null || item.IsEmpty())
        {
            Destroy(gameObject);
        }
    }
}