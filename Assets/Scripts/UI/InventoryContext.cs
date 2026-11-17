using UnityEngine;

[System.Serializable]
public struct InventoryContext
{
    public ItemData itemData;
    public int amount;

    public InventoryContext(ItemData itemData, int amount)
    {
        this.itemData = itemData;
        this.amount = amount;
    }

    public bool HasItem => itemData != null;
}
