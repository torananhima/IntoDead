
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
}
