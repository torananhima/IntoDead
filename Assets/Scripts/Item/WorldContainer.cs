
using System.Collections.Generic;
using UnityEngine;

public class WorldContainer : InteractableObject
{
    [Header("Loot")]
    [SerializeField] private ContainerData containerData;
    [SerializeField] private List<ItemInstance> contents = new();

    private bool lootInitialized;
    private bool opened;

    [Header("Interaction")]
    [SerializeField] private Collider interactionCollider;

    [Header("Item Spawning")]
    [SerializeField] private GameObject droppedItemPrefab;
    [SerializeField] private Transform dropPoint;
    [SerializeField] private float dropRadius = 0.5f;

    public bool IsOpened => opened;

    private void Awake()
    {
        InitializeLoot();
    }

    // --------------------------------------------------
    // LOOT GENERATION
    // --------------------------------------------------

    private void InitializeLoot()
    {
        if (lootInitialized)
            return;

        lootInitialized = true;

        if (containerData == null)
        {
            Debug.LogWarning(
                $"ContainerData is missing on {name}.", this);
            return;
        }

        if (containerData.lootEntries == null)
            return;

        foreach (LootEntry lootEntry in containerData.lootEntries)
        {
            if (lootEntry == null)
                continue;

            ItemEntry selectedEntry = SelectWeightedItem(lootEntry);

            if (selectedEntry == null)
                continue;

            ItemInstance instance = CreateItem(selectedEntry);

            if (instance != null)
                contents.Add(instance);
        }
    }

    private ItemEntry SelectWeightedItem(LootEntry lootEntry)
    {
        if (lootEntry.items == null || lootEntry.items.Count == 0)
            return null;

        float totalWeight = 0f;

        foreach (ItemEntry entry in lootEntry.items)
        {
            if (entry == null || entry.item == null || entry.weight <= 0f)
                continue;

            totalWeight += entry.weight;
        }

        if (totalWeight <= 0f)
            return null;

        float randomValue = Random.Range(0f, totalWeight);

        foreach (ItemEntry entry in lootEntry.items)
        {
            if (entry == null || entry.item == null || entry.weight <= 0f)
                continue;

            randomValue -= entry.weight;

            if (randomValue <= 0f)
                return entry;
        }

        return null;
    }

    private ItemInstance CreateItem(ItemEntry entry)
    {
        if (entry == null || entry.item == null)
            return null;

        int minAmount = Mathf.Max(1, entry.minAmount);
        int maxAmount = Mathf.Max(minAmount, entry.maxAmount);

        // Non-stackable items should always have amount 1.
        int amount = entry.item.IsStackable
            ? Random.Range(minAmount, maxAmount + 1)
            : 1;

        int magazineAmmo = -1;

        if (entry.item is RangedWeaponData weaponData)
        {
            int minAmmo = Mathf.Clamp(
                entry.minMagazineAmmo,
                0,
                weaponData.magazineSize
            );

            int maxAmmo = Mathf.Clamp(
                entry.maxMagazineAmmo,
                minAmmo,
                weaponData.magazineSize
            );

            magazineAmmo = Random.Range(minAmmo, maxAmmo + 1);
        }

        return new ItemInstance(
            entry.item,
            amount,
            magazineAmmo
        );
    }

    // --------------------------------------------------
    // INTERACTION
    // --------------------------------------------------

    public override bool CanInteract(GameObject interactor)
    {
        if (opened)
            return false;

        return interactor != null &&
               interactor.GetComponent<PlayerInventory>() != null;
    }

    public override void Interact(GameObject interactor)
    {
        if (!CanInteract(interactor))
            return;

        OpenAndDropContents();
    }

    // --------------------------------------------------
    // RELEASE LOOT
    // --------------------------------------------------

    private void OpenAndDropContents()
    {
        if (ObjectPoolManager.Instance == null)
        {
            Debug.LogError("ObjectPoolManager is missing.");
            return;
        }

        if (droppedItemPrefab == null)
        {
            Debug.LogError("DroppedItem prefab is missing.", this);
            return;
        }

        // Iterate backwards because successfully spawned items
        // are removed from the contents list.
        for (int i = contents.Count - 1; i >= 0; i--)
        {
            ItemInstance item = contents[i];

            if (item == null || item.IsEmpty)
            {
                contents.RemoveAt(i);
                continue;
            }

            Vector3 spawnPosition = GetDropPosition();

            GameObject droppedObject = ObjectPoolManager.Instance.Spawn(
                droppedItemPrefab,
                spawnPosition,
                Quaternion.identity
            );

            if (droppedObject == null)
            {
                Debug.LogWarning(
                    $"Failed to spawn loot from container {name}.",
                    this
                );

                continue;
            }

            DroppedItem droppedItem =
                droppedObject.GetComponent<DroppedItem>();

            if (droppedItem == null || !droppedItem.Initialize(item))
            {
                ObjectPoolManager.Instance.Despawn(droppedObject);

                Debug.LogWarning(
                    "DroppedItem component missing or initialization failed.",
                    droppedObject
                );

                continue;
            }

            // World DroppedItem now owns this instance.
            contents.RemoveAt(i);
        }

        // Only mark opened when all loot has been released.
        if (contents.Count == 0)
            MarkOpened();
    }

    private Vector3 GetDropPosition()
    {
        Vector3 center = dropPoint != null
            ? dropPoint.position
            : transform.position;

        Vector2 randomOffset = Random.insideUnitCircle * dropRadius;

        return center + new Vector3(
            randomOffset.x,
            0f,
            randomOffset.y
        );
    }

    private void MarkOpened()
    {
        opened = true;

        if (interactionCollider != null)
            interactionCollider.enabled = false;
    }
}
