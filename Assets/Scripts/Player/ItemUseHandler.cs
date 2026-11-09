using System;
using UnityEngine;

public class ItemUseHandler : MonoBehaviour 
{
    public PlayerInventory inventory { get; private set; }

    private HealingItemBehavior healingBehavior;
    private RangedWeaponBehavior rangedWeaponBehavior;
    Player player;
    Camera mainCam;

    private void Awake()
    {
        healingBehavior = new HealingItemBehavior();
        rangedWeaponBehavior = new RangedWeaponBehavior();
        player = GetComponent<Player>();
        inventory = GetComponent<PlayerInventory>();
        mainCam = Camera.main;
    }
    private void Start()
    {
        player._input.InputOnUse += TryUseSelectedItem;
        player._input.InputOnReload += TryReload;
    }

    private void OnDisable()
    {
        player._input.InputOnUse -= TryUseSelectedItem;
        player._input.InputOnReload -= TryReload;
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
        inventory.TryRemoveItem(item);
    }

    public void TryReload()
    {
        ItemInstance item = inventory.GetSelectedItem();
        if (item.itemData is RangedWeaponData rangedWeaponData)
        {
            int ammo = player._inventory.GetAmmo(rangedWeaponData.ammoType);
            player._inventory.RemoveAmmo(rangedWeaponData.ammoType, item.Reload(ammo));
        }
    }

    private IItemBehavior GetBehavior(ItemData data)
    {
        if (data is HealingItemData)
            return healingBehavior;
        if (data is RangedWeaponData)
            return rangedWeaponBehavior;

        return null;
    }

    public Vector3 GetPlayerMouseToward()
    {
        Vector3 worldPos = mainCam.ScreenToWorldPoint(player._input.MousePos);
        return (worldPos - transform.position).normalized;
    }
}
