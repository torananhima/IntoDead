using UnityEngine;

public class ItemInstance : MonoBehaviour
{
    [SerializeField] public ItemData itemData;
    [SerializeField] private int amount;
    [SerializeField] public int currentAmmo;
    [SerializeField] private string instanceId;

    public ItemInstance(ItemData data, int initialAmount)
    {
        itemData = data;
        amount = initialAmount;
        instanceId = System.Guid.NewGuid().ToString();
    }
    public int AddAmount(int amountToAdd)
    {
        amount += amountToAdd;
        return amount;
    }

    public int RemoveAmount(int amountToRemove)
    {
        amount -= amountToRemove;
        if(amount < 0)
        {
            amount = 0;
        }
        return amount;
    }

    public bool IsEmpty()
    {
        return amount <= 0;
    }

    public ItemInstance Clone()
    {
        ItemInstance clone = new ItemInstance(itemData, amount);
        clone.currentAmmo = this.currentAmmo;
        clone.instanceId = this.instanceId;
        return clone;
    }
}
