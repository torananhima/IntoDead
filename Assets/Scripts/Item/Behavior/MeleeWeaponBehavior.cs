
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponBehavior : IItemBehavior
{
    public void Use(ItemUseContext context)
    {
        if (!CanUse(context))
            return;

        MeleeWeaponData meleeData =
            context.itemInstance.itemData as MeleeWeaponData;

        ItemUseHandler handler = context.itemUseHandler;

        Vector3 origin = handler.transform.position;
        Vector3 direction = handler.GetPlayerMouseToward();

        if (direction.sqrMagnitude <= 0.001f)
            return;

        direction.Normalize();
        handler.UpdateMeleeDebug(direction);

        // Center the attack area in front of the player.
        Vector3 attackCenter = origin + direction * meleeData.attackRange;

        Collider[] colliders = Physics.OverlapSphere(
            attackCenter,
            meleeData.hitboxRadius,
            meleeData.attackLayer,
            QueryTriggerInteraction.Collide
        );

        // Prevent hitting the same enemy more than once.
        HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();

        foreach (Collider col in colliders)
        {
            IDamageable damageable =
                col.GetComponentInParent<IDamageable>();

            if (damageable == null || hitTargets.Contains(damageable))
                continue;

            // Find the target's position.
            Vector3 targetPosition = col.bounds.center;
            Vector3 toTarget = targetPosition - origin;
            toTarget.y = 0f;

            if (toTarget.sqrMagnitude <= 0.001f)
                continue;

            // Check whether the target is inside the attack angle.
            float angle = Vector3.Angle(direction, toTarget.normalized);

            if (angle > meleeData.attackAngle * 0.5f)
                continue;

            hitTargets.Add(damageable);
            damageable.TakeDamage(meleeData.damage);
        }

        handler.StartAttackCooldown(meleeData.attackCooldown);

        NoiseSystem.GetInstance().NoiseOccur(
            NoiseSource.Player,
            origin,
            meleeData.noiseRadius
        );
    }

    public bool CanUse(ItemUseContext context)
    {
        if (context == null || context.itemInstance == null)
            return false;

        if (context.itemInstance.itemData is not MeleeWeaponData)
            return false;

        return true;
    }

    public void AltUse(ItemUseContext context)
    {
        // Not implemented yet.
    }
}
