using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class PlayerInventory : MonoBehaviour
{
    public int pistolAmmo;
    public int rifleAmmo;
    public int shotgunAmmo;
    public int sniperAmmo;
    [SerializeField] private int maxSlots = 10;
    public int selectedSlot = 0;

    public readonly List<ItemInstance> items = new List<ItemInstance>();

    public event Action OnInventoryChanged;
    Player player;

    public ItemInstance CurrentItem => items[selectedSlot];

    private void Start()
    {
        player = GetComponent<Player>();
        player._input.InputOnChangeSlot += SelectSlot;
        player._input.InputOnReload += RelaodWeapon;
    }

    private void OnDisable()
    {
        player._input.InputOnChangeSlot -= SelectSlot;
        player._input.InputOnReload -= RelaodWeapon;
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

    private void RelaodWeapon()
    {
        if (items[selectedSlot].itemData is RangedWeaponData weapon)
        {
            RemoveAmmo(weapon.ammoType, CurrentItem.Reload(GetAmmo(weapon.ammoType)));
        }
    }

    public int GetAmmo(AmmoType ammoType)
    {
        switch (ammoType)
        {
            case AmmoType.Pistol:
                return pistolAmmo;
            case AmmoType.Rifle:
                return rifleAmmo;
            case AmmoType.Shotgun:
                return shotgunAmmo;
            case AmmoType.Sniper:
                return sniperAmmo;
            default:
                return 0;
        }
    }
    public void AddAmmo(AmmoType ammoType, int amount)
    {
        switch (ammoType)
        {
            case AmmoType.Pistol:
                pistolAmmo += amount;
                break;
            case AmmoType.Rifle:
                rifleAmmo += amount;
                break;
            case AmmoType.Shotgun:
                shotgunAmmo += amount;
                break;
            case AmmoType.Sniper:
                sniperAmmo += amount;
                break;
            default:
                break;
        }
    }
    public void RemoveAmmo(AmmoType ammoType, int amount)
    {
        switch (ammoType)
        {
            case AmmoType.Pistol:
                pistolAmmo -= amount;
                break;
            case AmmoType.Rifle:
                rifleAmmo -= amount;
                break;
            case AmmoType.Shotgun:
                shotgunAmmo -= amount;
                break;
            case AmmoType.Sniper:
                sniperAmmo -= amount;
                break;
            default:
                break;
        }
    }
}