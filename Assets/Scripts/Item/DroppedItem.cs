
using UnityEngine;

public class DroppedItem : InteractableObject, IPoolable
{
    [Header("Pickup")]
    [SerializeField] private ItemInstance item;
    [SerializeField] private float pickupDelay = 0.25f;

    private float pickupTime;
    private bool initialized;

    [Header("On Drop")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Vector2 spawnForce = new Vector2(1f, 2f);
    [SerializeField] private GameObject model;

    public ItemInstance Item => item;

    // Called after the object has been retrieved from the pool.
    public bool Initialize(ItemInstance itemInstance)
    {
        if (itemInstance == null || itemInstance.IsEmpty)
            return false;

        if (itemInstance.itemData == null)
            return false;

        ClearModel();

        item = itemInstance;
        initialized = true;
        pickupTime = Time.time + pickupDelay;

        GameObject prefab = item.itemData.worldPrefab;

        if (prefab != null)
        {
            model = Instantiate(prefab, transform);
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
        }
        else
        {
            Debug.LogWarning($"Item {item.itemData.name} has no world prefab.");
        }

        BounceOff();
        return true;
    }

    private void BounceOff()
    {
        if (rb == null)
            return;

        Vector3 randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            0f,
            Random.Range(-1f, 1f)
        ).normalized;

        Vector3 force = randomDirection * spawnForce.x;
        force.y = spawnForce.y;

        rb.AddForce(force, ForceMode.Impulse);
    }

    public override bool CanInteract(GameObject interactor)
    {
        if (!initialized || item == null || item.IsEmpty)
            return false;

        if (Time.time < pickupTime)
            return false;

        return interactor.GetComponent<PlayerInventory>() != null;
    }

    public override void Interact(GameObject interactor)
    {
        if (!CanInteract(interactor))
            return;

        PlayerInventory inventory =
            interactor.GetComponent<PlayerInventory>();

        if (inventory == null)
            return;

        bool fullyAccepted = inventory.TryAddItem(item);

        if (fullyAccepted)
        {
            // Inventory now owns the item instance.
            if (ObjectPoolManager.Instance != null)
                ObjectPoolManager.Instance.Despawn(gameObject);
        }
    }

    // Called when this object is retrieved from the pool.
    public void OnPoolSpawn()
    {
        item = null;
        initialized = false;
        pickupTime = 0f;

        ClearModel();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    // Called when this object returns to the pool.
    public void OnPoolDespawn()
    {
        item = null;
        initialized = false;
        pickupTime = 0f;

        ClearModel();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void ClearModel()
    {
        if (model == null)
            return;

        Destroy(model);
        model = null;
    }
}
