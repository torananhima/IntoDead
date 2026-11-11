
using UnityEngine;

public class RangedWeaponBehavior : IItemBehavior
{
    public void Use(ItemUseContext context)
    {
        if (!CanUse(context))
            return;

        RangedWeaponData weaponData =
            context.itemInstance.itemData as RangedWeaponData;

        ItemUseHandler handler = context.itemUseHandler;

        Vector3 direction = handler.GetPlayerMouseToward();

        if (direction.sqrMagnitude <= 0.001f)
            return;

        direction.Normalize();

        Transform firePoint = handler.FirePoint;
        Quaternion rotation = Quaternion.LookRotation(context.itemUseHandler.GetPlayerMouseToward());

        GameObject bulletObject = ObjectPoolManager.Instance.Spawn(
            weaponData.bulletPrefab,
            firePoint.position,
            rotation
        );

        if (bulletObject == null)
            return;

        Bullet bullet = bulletObject.GetComponent<Bullet>();

        if (bullet == null)
        {
            Debug.LogError("Bullet prefab is missing Bullet component!");
            ObjectPoolManager.Instance.Despawn(bulletObject);
            return;
        }

        bullet.Initialize(
            Mathf.RoundToInt(weaponData.damage),
            direction
        );

        context.itemInstance.ConsumeAmmo(1);

        NoiseSystem.GetInstance().NoiseOccur(
            NoiseSource.Player,
            handler.transform.position,
            weaponData.noiseRadius
        );

        handler.inventory.NotifyInventoryChanged();

        // Start cooldown only after a successful shot.
        handler.StartAttackCooldown(weaponData.fireCooldown);
    }

    public bool CanUse(ItemUseContext context)
    {
        if (context == null || context.itemInstance == null)
            return false;

        if (context.itemUseHandler == null)
            return false;

        RangedWeaponData weaponData =
            context.itemInstance.itemData as RangedWeaponData;

        if (weaponData == null || weaponData.bulletPrefab == null)
            return false;

        if (ObjectPoolManager.Instance == null)
            return false;

        return context.itemInstance.HasAmmo;
    }

    public void AltUse(ItemUseContext context)
    {
    }
}
