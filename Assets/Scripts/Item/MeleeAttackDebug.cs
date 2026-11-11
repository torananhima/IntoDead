
using UnityEngine;

public class MeleeAttackDebug : MonoBehaviour
{
    [SerializeField] private bool showDebug = true;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float hitboxRadius = 1f;
    [SerializeField, Range(0f, 360f)] private float attackAngle = 120f;

    private Vector3 attackDirection;

    public void UpdateAttackDirection(Vector3 direction)
    {
        attackDirection = direction.normalized;
    }

    private void OnDrawGizmos()
    {
        if (!showDebug)
            return;

        if (attackDirection.sqrMagnitude <= 0.001f)
            return;

        Vector3 origin = transform.position;
        Vector3 center = origin + attackDirection * attackRange;

        // Attack hitbox
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, hitboxRadius);

        // Attack direction
        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin, center);

        // Attack angle boundaries
        Vector3 left = Quaternion.Euler(
            0f, -attackAngle * 0.5f, 0f
        ) * attackDirection;

        Vector3 right = Quaternion.Euler(
            0f, attackAngle * 0.5f, 0f
        ) * attackDirection;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + left * attackRange);
        Gizmos.DrawLine(origin, origin + right * attackRange);
    }
}
