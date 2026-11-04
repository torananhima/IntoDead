using UnityEngine;

public class HealingItemBehavior : MonoBehaviour, IItemBehavior
{
    public void Use(ItemUseContext context)
    {
        HealingItemData healingData = context.itemInstance.itemData as HealingItemData;
        float healAmount = healingData != null ? healingData.GetHealingAmount() : 0f;
        Debug.Log($"Healing item used! Healing: {healAmount}");
        context.itemUseHandler.GetComponent<Player>().Heal((int)healAmount);
    }

    public bool CanUse(ItemUseContext context)
    {
        return true; // Placeholder implementation
    }
}
