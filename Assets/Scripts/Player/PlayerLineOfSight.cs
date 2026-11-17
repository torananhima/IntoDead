
using UnityEngine;

public class PlayerLineOfSight : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform visionOrigin;
    [SerializeField] private Transform flashlightAim;

    [Header("Ambient Vision")]
    [SerializeField] private float ambientRadius = 7f;

    [Header("Flashlight")]
    [SerializeField] private float flashlightRange = 14f;
    [SerializeField, Range(1f, 179f)]
    private float flashlightAngle = 60f;

    [Header("Occlusion")]
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float eyeHeight = 1f;

    [Header("Gizmos")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private int coneSegments = 30;

    public bool CanSee(Transform target)
    {
        if (target == null)
            return false;

        Vector3 origin = visionOrigin != null
            ? visionOrigin.position
            : transform.position;

        Vector3 targetPosition = target.position;

        Vector3 flatOffset = targetPosition - origin;
        flatOffset.y = 0f;

        float distance = flatOffset.magnitude;

        bool inAmbientRange = distance <= ambientRadius;

        Vector3 forward = flashlightAim != null
            ? flashlightAim.forward
            : transform.forward;

        forward.y = 0f;

        bool inFlashlight =
            distance <= flashlightRange &&
            distance > 0.001f &&
            Vector3.Angle(forward, flatOffset)
                <= flashlightAngle * 0.5f;

        if (!inAmbientRange && !inFlashlight)
            return false;

        Vector3 rayOrigin = origin + Vector3.up * eyeHeight;
        Vector3 rayTarget = target.position + Vector3.up * 0.5f;
        Vector3 rayDirection = rayTarget - rayOrigin;

        float rayDistance = rayDirection.magnitude;

        if (rayDistance <= 0.001f)
            return true;

        return !Physics.Raycast(
            rayOrigin,
            rayDirection / rayDistance,
            rayDistance,
            obstacleMask,
            QueryTriggerInteraction.Ignore
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos)
            return;

        Vector3 origin = visionOrigin != null
            ? visionOrigin.position
            : transform.position;

        Vector3 forward = flashlightAim != null
            ? flashlightAim.forward
            : transform.forward;

        forward.y = 0f;

        if (forward.sqrMagnitude < 0.001f)
            return;

        forward.Normalize();

        // Ambient vision radius
        Gizmos.color = Color.green;
        DrawCircle(origin, ambientRadius);

        // Flashlight cone
        Gizmos.color = Color.yellow;

        Vector3 leftDirection = Quaternion.Euler(
            0f, -flashlightAngle * 0.5f, 0f
        ) * forward;

        Vector3 rightDirection = Quaternion.Euler(
            0f, flashlightAngle * 0.5f, 0f
        ) * forward;

        Vector3 leftEnd = origin + leftDirection * flashlightRange;
        Vector3 rightEnd = origin + rightDirection * flashlightRange;

        Gizmos.DrawLine(origin, leftEnd);
        Gizmos.DrawLine(origin, rightEnd);

        // Draw cone arc
        Vector3 previousPoint = leftEnd;

        for (int i = 1; i <= coneSegments; i++)
        {
            float angle = Mathf.Lerp(
                -flashlightAngle * 0.5f,
                flashlightAngle * 0.5f,
                (float)i / coneSegments
            );

            Vector3 direction =
                Quaternion.Euler(0f, angle, 0f) * forward;

            Vector3 point = origin + direction * flashlightRange;

            Gizmos.DrawLine(previousPoint, point);

            previousPoint = point;
        }
    }

    private void DrawCircle(Vector3 center, float radius)
    {
        const int segments = 60;

        Vector3 previousPoint =
            center + Vector3.forward * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;

            Vector3 point = center + new Vector3(
                Mathf.Sin(angle) * radius,
                0f,
                Mathf.Cos(angle) * radius
            );

            Gizmos.DrawLine(previousPoint, point);
            previousPoint = point;
        }
    }
}
