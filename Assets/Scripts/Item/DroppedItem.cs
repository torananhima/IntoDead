using UnityEngine;

public class DroppedItem : InteractableObject
{
    [Header("Pickup")]
    [SerializeField] private ItemInstance item;
    [SerializeField] private bool pickupEnabled;
    [SerializeField] private float pickupDelay = 0.25f;

    private float pickupTime;

    public void Initialize(ItemInstance itemInstance)
    {
        if (itemInstance == null)
        {
            Debug.LogWarning("DroppedItem received null ItemInstance.");
            Destroy(gameObject);
            return;
        }

        item = itemInstance;
        pickupEnabled = false;
        pickupTime = Time.time + pickupDelay;
    }

    private void Update()
    {
        if (!pickupEnabled && Time.time >= pickupTime)
        {
            pickupEnabled = true;
        }
    }

    public override bool CanInteract(GameObject interactor)
    {
        return pickupEnabled &&
               item != null &&
               !item.IsEmpty() &&
               interactor != null &&
               interactor.TryGetComponent<PlayerInventory>(out _);
    }

    public override void Interact(GameObject interactor)
    {
        TryPickup(interactor);
    }

    public void TryPickup(GameObject interactor)
    {
        if (!CanInteract(interactor))
            return;

        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();

        if (!inventory.TryAddItem(item))
            return;
    }

    public void PickupCompleted()
    {
        //Debug.Log("Pickup complete");
        Destroy(gameObject);
    }
    public void DespawnIfEmpty()
    {
        if (item == null || item.IsEmpty())
        {
            Debug.Log("Item empty");
            //Destroy(gameObject);
        }
    }
}