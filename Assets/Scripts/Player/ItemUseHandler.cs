using UnityEngine;

public class ItemUseHandler : MonoBehaviour 
{
    [SerializeField] private PlayerInventory inventory;

    private HealingItemBehavior healingBehavior;
    private RangedWeaponBehavior rangedWeaponBehavior;

    private void Awake()
    {
        healingBehavior = new HealingItemBehavior();
        rangedWeaponBehavior = new RangedWeaponBehavior();
    }

    public bool TryUseSelectedItem()
    {
        ItemInstance item = inventory.GetSelectedItem();

        if (item == null || item.itemData == null)
            return false;

        ItemUseContext context = new ItemUseContext(
            gameObject,
            item
        );

        IItemBehavior behavior = GetBehavior(item.itemData);

        if (behavior == null)
            return false;

        if (!behavior.CanUse(context))
            return false;

        behavior.Use(context);

        return true;
    }

    private IItemBehavior GetBehavior(ItemData data)
    {
        if (data is HealingItemData)
            return healingBehavior;

        if (data is RangedWeaponData)
            return rangedWeaponBehavior;

        return null;
    }
}
