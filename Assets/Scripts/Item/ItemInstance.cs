using UnityEngine;

public class ItemInstance
{
    public ItemData itemData;
    [SerializeField] private int amount;
    public int maxMagazine;
    public int currentAmmo;
    [SerializeField] private string instanceId;

    public int Amount => amount;
    public string InstanceId => instanceId;

    public ItemInstance(ItemData data, int initialAmount)
    {
        itemData = data;
        amount = initialAmount;
        instanceId = System.Guid.NewGuid().ToString();
    }
    public int AddAmount(int amountToAdd)
    {
        if (amountToAdd <= 0) return amount;

        amount += amountToAdd;
        return amount;
    }

    public int RemoveAmount(int amountToRemove)
    {
        if (amountToRemove <= 0) return amount;

        amount = Mathf.Max(0, amount - amountToRemove);
        return amount;
    }

    public int Reload(int amount)
    {
        int neededAmmo = maxMagazine - currentAmmo;
        if (amount < neededAmmo) neededAmmo = amount;
        currentAmmo += neededAmmo;
        Debug.Log("Reload");
        return neededAmmo;
    }

    public bool IsMagazineEmpty()
    {
        if (currentAmmo > 0) return true;
        return false;
    }
    public bool IsEmpty()
    {
        return amount <= 0;
    }

    public ItemInstance Clone()
    {
        ItemInstance clone = new ItemInstance(itemData, amount);
        clone.currentAmmo = currentAmmo;

        return clone;
    }
}
