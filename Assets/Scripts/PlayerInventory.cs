using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int maxSlots = 10;
    public int selectedSlot = 0;

    public readonly List<ItemInstance> items = new List<ItemInstance>();

    public event Action OnInventoryChanged;
    Player player;

    private void Start()
    {
        player = GetComponent<Player>();
        player._input.InputOnChangeSlot += SelectSlot;
    }

    private void OnDisable()
    {
        player._input.InputOnChangeSlot -= SelectSlot;
    }
    public bool TryAddItem(ItemInstance item)
    {
        if (item == null || !HasSpaceForItem(item))
        {
            return false;
        }

        items.Add(item);

        if (selectedSlot >= maxSlots)
        {
            selectedSlot = maxSlots - 1;
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool TryRemoveItem(ItemInstance item)
    {
        if (item == null || !items.Remove(item))
        {
            return false;
        }

        ClampSelectedSlot();
        OnInventoryChanged?.Invoke();
        return true;
    }

    public ItemInstance GetItem(int slot)
    {
        if (slot < 0 || slot >= items.Count)
        {
            return null;
        }

        return items[slot];
    }

    public void DropItem(int slot, int amount)
    {
        if (slot < 0 || slot >= items.Count || amount <= 0)
        {
            return;
        }

        ItemInstance item = items[slot];

        if (item == null)
        {
            items.RemoveAt(slot);
            ClampSelectedSlot();
            OnInventoryChanged?.Invoke();
            return;
        }

        item.RemoveAmount(amount);

        if (item.IsEmpty())
        {
            items.RemoveAt(slot);
            ClampSelectedSlot();
        }

        OnInventoryChanged?.Invoke();
    }

    public bool HasSpaceForItem(ItemInstance item)
    {
        return item != null && items.Count < maxSlots;
    }

    public ItemInstance GetSelectedItem()
    {
        return GetItem(selectedSlot);
    }

    private void SelectSlot(int slot)
    {
        TrySelectSlot(slot);
    }
    public bool TrySelectSlot(int slot)
    {
        selectedSlot = slot;
        OnInventoryChanged?.Invoke();

        if (slot < 0 || slot >= items.Count)
        {
            return false;
        }

        return true;
    }

    private void ClampSelectedSlot()
    {
        if (items.Count == 0)
        {
            selectedSlot = 0;
        }
        else
        {
            selectedSlot = Mathf.Clamp(selectedSlot, 0, items.Count - 1);
        }
    }
}