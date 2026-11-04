using UnityEngine;

public class ItemUseHandler : MonoBehaviour 
{
    [SerializeField] private PlayerInventory inventory;

    private HealingItemBehavior healingBehavior;
    private RangedWeaponBehavior rangedWeaponBehavior;
    Player player;
    private void Awake()
    {
        healingBehavior = new HealingItemBehavior();
        rangedWeaponBehavior = new RangedWeaponBehavior();
        player = GetComponent<Player>();
        inventory = GetComponent<PlayerInventory>();
    }
    private void Start()
    {
        player._input.InputOnUse += TryUseSelectedItem;
    }

    private void OnDisable()
    {
        player._input.InputOnUse -= TryUseSelectedItem;
    }
    public void TryUseSelectedItem()
    {
        ItemInstance item = inventory.GetSelectedItem();

        if (item == null || item.itemData == null)
            return;

        ItemUseContext context = new ItemUseContext(
            gameObject,
            item
        );

        IItemBehavior behavior = GetBehavior(item.itemData);

        if (behavior == null)
            return;

        if (!behavior.CanUse(context))
            return;

        behavior.Use(context);
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
