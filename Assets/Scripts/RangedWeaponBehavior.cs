using UnityEngine;

public class RangedWeaponBehavior : MonoBehaviour, IItemBehavior
{
    public void Use(ItemUseContext context)
    {
        RangedWeaponData rangedData = context.itemInstance.itemData as RangedWeaponData;
        float damage = rangedData != null ? rangedData.damage : 0f;
        if (context.itemInstance.currentAmmo <= 0)
        {
            Debug.Log("No ammo left!");
            return;
        }
        context.itemInstance.currentAmmo--;
        Debug.Log($"Ranged item used! Damage: {damage}");
        // Implement ranged weapon usage logic here
    }

    public bool CanUse(ItemUseContext context)
    {
        return true; // Placeholder implementation
    }
}
