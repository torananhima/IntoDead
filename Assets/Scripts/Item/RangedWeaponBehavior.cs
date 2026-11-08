using UnityEngine;


public class RangedWeaponBehavior : IItemBehavior
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

        Bullet bullet = ObjectPoolManager.Instance.Spawn(rangedData.bulletPrefab, context.itemUseHandler.transform.position, context.itemUseHandler.transform.rotation).GetComponent<Bullet>();
        Vector3 dir = context.itemUseHandler.GetPlayerMouseToward();
        bullet.Initialize(rangedData.damage, dir);
    }

    public bool CanUse(ItemUseContext context)
    {
        if (context.itemInstance.currentAmmo <= 0)
        {
            Debug.Log("Ammo is out");
            return false;
        }
        return true;
    }

    public void AltUse(ItemUseContext context)
    {

    }
}
