using UnityEngine;

public class Noise : MonoBehaviour, IPoolable
{
    [Header("Debug")]
    [SerializeField] private bool isDrawGizmo;

    [Header("Settings")]
    [SerializeField] private float spreadSpeed = 2f;
    [SerializeField] private float startScale = 0.1f;

    private float targetScale;
    private bool isInitialized;

    public void OnPoolSpawn()
    {
        isInitialized = false;
        transform.localScale = Vector3.one * startScale;
    }

    public void OnPoolDespawn()
    {
        isInitialized = false;
        transform.localScale = Vector3.one;
    }

    public void Initialize(float range)
    {
        targetScale = range / 5f;

        transform.localScale = Vector3.one * startScale;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            Vector3.one * targetScale,
            spreadSpeed * Time.deltaTime
        );

        if (transform.localScale.x >= targetScale - 0.001f)
        {
            ObjectPoolManager.Instance.Despawn(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (!isDrawGizmo) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, targetScale * 5f);
    }
}
