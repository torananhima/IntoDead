
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private int maxSlots = 10;
    [SerializeField] private int selectedSlot = 0;

    [SerializeField] private List<ItemInstance> items = new();

    [Header("World Drop")]
    [SerializeField] private GameObject droppedItemPrefab;
    [SerializeField] private Transform dropPoint;

    [Header("Reserve Ammunition")]
    public int pistolAmmo;
    public int rifleAmmo;
    public int sniperAmmo;

    private PlayerInputSystem input;

    public event Action OnInventoryChanged;

    public int MaxSlots => maxSlots;
    public int SelectedSlot => selectedSlot;

    public IReadOnlyList<ItemInstance> Items => items;

    public ItemInstance GetItem(int slot)
    {
        if (!IsValidSlot(slot))
            return null;

        return items[slot];
    }

    public ItemInstance GetSelectedItem()
    {
        return GetItem(selectedSlot);
    }

    private void Awake()
    {
        maxSlots = Mathf.Max(1, maxSlots);

        // Create exactly maxSlots empty slots.
        items = new List<ItemInstance>(maxSlots);

        for (int i = 0; i < maxSlots; i++)
        {
            items.Add(null);
        }

        input = GetComponent<PlayerInputSystem>();
    }

    private void OnEnable()
    {
        if (input == null)
            return;

        input.InputOnChangeSlot += HandleChangeSlot;
        input.InputOnDrop += HandleDrop;
    }

    private void OnDisable()
    {
        if (input == null)
            return;

        input.InputOnChangeSlot -= HandleChangeSlot;
        input.InputOnDrop -= HandleDrop;
    }

    private void HandleChangeSlot(int slot)
    {
        TrySelectSlot(slot);
    }

    private void HandleDrop()
    {
        DropSelectedItem();
    }

    // --------------------------------------------------
    // ADD ITEMS
    // --------------------------------------------------

    public bool TryAddItem(ItemInstance incoming)
    {
        if (incoming == null || incoming.IsEmpty)
            return false;

        // 1. Fill existing compatible stacks.
        if (incoming.itemData.IsStackable)
        {
            for (int i = 0; i < items.Count; i++)
            {
                ItemInstance existing = items[i];

                if (existing == null)
                    continue;

                if (!existing.CanStackWith(incoming))
                    continue;

                incoming.TransferTo(existing, incoming.Amount);

                if (incoming.IsEmpty)
                {
                    NotifyInventoryChanged();
                    return true;
                }
            }
        }

        // 2. Find an empty slot.
        int emptySlot = FindEmptySlot();

        if (emptySlot >= 0)
        {
            // Transfer ownership to inventory.
            items[emptySlot] = incoming;

            NotifyInventoryChanged();
            return true;
        }

        // 3. No empty slot. The incoming instance
        // retains any amount that wasn't transferred.
        NotifyInventoryChanged();
        return false;
    }

    private int FindEmptySlot()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null || items[i].itemData == null)
                return i;
        }

        return -1;
    }

    // --------------------------------------------------
    // REMOVE ITEMS
    // --------------------------------------------------

    public bool TryRemoveItem(int slot)
    {
        if (!IsValidSlot(slot))
            return false;

        if (items[slot] == null)
            return false;

        items[slot] = null;

        NotifyInventoryChanged();
        return true;
    }

    public bool TryRemoveItem(ItemInstance item)
    {
        if (item == null)
            return false;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == item)
                return TryRemoveItem(i);
        }

        return false;
    }

    // --------------------------------------------------
    // DROP ITEMS
    // --------------------------------------------------

    public bool DropSelectedItem()
    {
        return DropItem(selectedSlot);
    }

    public bool DropItem(int slot)
    {
        if (!IsValidSlot(slot))
            return false;

        ItemInstance item = items[slot];

        if (item == null || droppedItemPrefab == null)
            return false;

        if (ObjectPoolManager.Instance == null)
        {
            Debug.LogError("ObjectPoolManager is missing.");
            return false;
        }

        Vector3 position = dropPoint != null
            ? dropPoint.position
            : transform.position;

        GameObject droppedObject = ObjectPoolManager.Instance.Spawn(
            droppedItemPrefab,
            position,
            Quaternion.identity
        );

        if (droppedObject == null)
            return false;

        DroppedItem droppedItem = droppedObject.GetComponent<DroppedItem>();

        if (droppedItem == null)
        {
            Debug.LogError("DroppedItem component is missing.", droppedObject);

            ObjectPoolManager.Instance.Despawn(droppedObject);
            return false;
        }

        // Initialize before transferring ownership.
        if (!droppedItem.Initialize(item))
        {
            ObjectPoolManager.Instance.Despawn(droppedObject);
            return false;
        }

        // Transfer ownership.
        items[slot] = null;

        NotifyInventoryChanged();
        return true;
    }

    // --------------------------------------------------
    // SLOT SELECTION
    // --------------------------------------------------

    public bool TrySelectSlot(int slot)
    {
        if (!IsValidSlot(slot))
            return false;

        if (selectedSlot == slot)
            return true;

        selectedSlot = slot;

        NotifyInventoryChanged();
        return true;
    }

    private bool IsValidSlot(int slot)
    {
        return slot >= 0 && slot < items.Count;
    }

    // --------------------------------------------------
    // RESERVE AMMUNITION
    // --------------------------------------------------

    public int GetAmmo(AmmoType ammoType)
    {
        return ammoType switch
        {
            AmmoType.Pistol => pistolAmmo,
            AmmoType.Rifle => rifleAmmo,
            AmmoType.Sniper => sniperAmmo,
            _ => 0
        };
    }

    public void AddAmmo(AmmoType ammoType, int amount)
    {
        if (amount <= 0)
            return;

        switch (ammoType)
        {
            case AmmoType.Pistol:
                pistolAmmo += amount;
                break;

            case AmmoType.Rifle:
                rifleAmmo += amount;
                break;

            case AmmoType.Sniper:
                sniperAmmo += amount;
                break;

            default:
                return;
        }

        NotifyInventoryChanged();
    }


    public bool RemoveAmmo(AmmoType ammoType, int amount)
    {
        if (amount <= 0 || GetAmmo(ammoType) < amount)
            return false;

        switch (ammoType)
        {
            case AmmoType.Pistol:
                pistolAmmo -= amount;
                break;

            case AmmoType.Rifle:
                rifleAmmo -= amount;
                break;

            case AmmoType.Sniper:
                sniperAmmo -= amount;
                break;

            default:
                return false;
        }

        NotifyInventoryChanged();
        return true;
    }

    // --------------------------------------------------
    // EVENTS
    // --------------------------------------------------

    public void NotifyInventoryChanged()
    {
        OnInventoryChanged?.Invoke();
    }
}
