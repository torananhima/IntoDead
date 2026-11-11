
using UnityEngine;

public class ThrowableItemBehavior : IItemBehavior
{
    public void Use(ItemUseContext context)
    {
        if (!CanUse(context))
            return;

        ThrowableItemData data =
            context.itemInstance.itemData as ThrowableItemData;

        ItemUseHandler handler = context.itemUseHandler;

        Vector3 direction = handler.GetPlayerMouseToward();

        if (direction.sqrMagnitude <= 0.001f)
            return;

        direction.Normalize();

        Transform firePoint = handler.FirePoint;

        Quaternion rotation = Quaternion.LookRotation(direction);

        GameObject projectile = ObjectPoolManager.Instance.Spawn(
            data.projectilePrefab,
            firePoint.position,
            rotation
        );

        if (projectile == null)
            return;

        ThrowableProjectile throwable =
            projectile.GetComponent<ThrowableProjectile>();

        if (throwable == null)
        {
            Debug.LogError(
                "Throwable prefab is missing ThrowableProjectile!"
            );

            ObjectPoolManager.Instance.Despawn(projectile);
            return;
        }

        throwable.Initialize(data, handler.gameObject, direction);

        context.itemInstance.RemoveAmount(1);
    }

    public bool CanUse(ItemUseContext context)
    {
        if (context == null || context.itemInstance == null)
            return false;

        ThrowableItemData data =
            context.itemInstance.itemData as ThrowableItemData;

        if (data == null || data.projectilePrefab == null)
            return false;

        if (ObjectPoolManager.Instance == null)
            return false;

        return !context.itemInstance.IsEmpty;
    }

    public void AltUse(ItemUseContext context)
    {
        // Not implemented yet.
    }
}
