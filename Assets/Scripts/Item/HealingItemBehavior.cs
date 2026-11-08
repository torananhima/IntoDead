using UnityEngine;

public class HealingItemBehavior : IItemBehavior
{
    public void Use(ItemUseContext context)
    {
        HealingItemData healingData = context.itemInstance.itemData as HealingItemData;
        float healAmount = healingData != null ? healingData.GetHealingAmount() : 0f;
        Debug.Log($"Healing item used! Healing: {healAmount}");
        context.itemUseHandler.GetComponent<Player>().Heal((int)healAmount);
        context.itemInstance.RemoveAmount(1);
        if (context.itemInstance.IsEmpty()) context.itemUseHandler.inventory.TryRemoveItem(context.itemInstance);
    }

    public bool CanUse(ItemUseContext context)
    {
        if (context.itemInstance.IsEmpty()) return false;
        return true;
    }
    public void AltUse(ItemUseContext context)
    {

    }
}
