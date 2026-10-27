
using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightAim : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float height = 1.5f;
    [SerializeField] private LayerMask groundMask;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (mainCamera == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        Plane groundPlane = new Plane(
            Vector3.up,
            new Vector3(0f, transform.position.y - height, 0f)
        );

        if (!groundPlane.Raycast(ray, out float enter))
            return;

        Vector3 target = ray.GetPoint(enter);
        Vector3 direction = target - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        transform.rotation = Quaternion.LookRotation(direction);
    }
}
