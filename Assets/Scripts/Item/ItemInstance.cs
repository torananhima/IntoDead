
using System;
using UnityEngine;

[Serializable]
public class ItemInstance
{
    public ItemData itemData { get; private set; }

    [SerializeField] private int amount;
    [SerializeField] private string instanceId;

    // Ranged weapon state
    [SerializeField] private int maxMagazine;
    [SerializeField] private int currentAmmo;

    public int Amount => amount;
    public string InstanceId => instanceId;

    public int MaxMagazine => maxMagazine;
    public int CurrentAmmo => currentAmmo;

    public bool IsEmpty => amount <= 0;
    public bool HasAmmo => currentAmmo > 0;

    public int StackSpace
    {
        get
        {
            if (itemData == null || !itemData.IsStackable)
                return 0;

            return Mathf.Max(0, itemData.MaxStackSize - amount);
        }
    }

    public ItemInstance(
        ItemData data,
        int initialAmount = 1,
        int initialMagazineAmmo = -1)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        itemData = data;
        instanceId = Guid.NewGuid().ToString();

        int maxAmount = data.IsStackable
            ? Mathf.Max(1, data.MaxStackSize)
            : 1;

        amount = Mathf.Clamp(initialAmount, 0, maxAmount);

        if (data is RangedWeaponData weaponData)
        {
            maxMagazine = Mathf.Max(0, weaponData.magazineSize);

            currentAmmo = initialMagazineAmmo < 0
                ? maxMagazine
                : Mathf.Clamp(initialMagazineAmmo, 0, maxMagazine);
        }
    }

    public bool CanStackWith(ItemInstance other)
    {
        return other != null &&
               itemData != null &&
               other.itemData != null &&
               itemData.CanStackWith(other.itemData) &&
               StackSpace > 0;
    }

    // Returns the new total amount.
    public int AddAmount(int amountToAdd)
    {
        if (amountToAdd <= 0 || itemData == null)
            return amount;

        amount = Mathf.Min(
            amount + amountToAdd,
            itemData.IsStackable ? itemData.MaxStackSize : 1
        );

        return amount;
    }

    // Returns the remaining amount.
    public int RemoveAmount(int amountToRemove)
    {
        if (amountToRemove <= 0)
            return amount;

        amount = Mathf.Max(0, amount - amountToRemove);
        return amount;
    }

    // Returns the amount actually transferred.
    public int TransferTo(ItemInstance target, int requestedAmount)
    {
        if (target == null || !CanStackWith(target))
            return 0;

        int transferAmount = Mathf.Min(
            requestedAmount,
            amount,
            target.StackSpace
        );

        if (transferAmount <= 0)
            return 0;

        target.AddAmount(transferAmount);
        RemoveAmount(transferAmount);

        return transferAmount;
    }

    // Returns the amount loaded into the magazine.
    public int Reload(int availableAmmo)
    {
        if (maxMagazine <= 0 || availableAmmo <= 0)
            return 0;

        int neededAmmo = maxMagazine - currentAmmo;
        int loadedAmmo = Mathf.Min(neededAmmo, availableAmmo);

        currentAmmo += loadedAmmo;

        return loadedAmmo;
    }

    public bool ConsumeAmmo(int amountToConsume = 1)
    {
        if (amountToConsume <= 0 || currentAmmo < amountToConsume)
            return false;

        currentAmmo -= amountToConsume;
        return true;
    }

    // Splits a stack into a new instance.
    // Intended for stackable items only.
    public ItemInstance SplitStack(int splitAmount)
    {
        if (itemData == null ||
            !itemData.IsStackable ||
            splitAmount <= 0 ||
            splitAmount >= amount)
        {
            return null;
        }

        RemoveAmount(splitAmount);

        return new ItemInstance(itemData, splitAmount);
    }
}
