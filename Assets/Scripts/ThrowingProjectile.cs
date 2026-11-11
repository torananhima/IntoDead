
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThrowableProjectile : MonoBehaviour, IPoolable
{
    [SerializeField] private Rigidbody rb;

    private ThrowableItemData itemData;
    private GameObject thrower;
    private bool initialized;
    private bool hasLanded;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    public void Initialize(
        ThrowableItemData data,
        GameObject thrower,
        Vector3 direction)
    {
        if (data == null || rb == null)
            return;

        itemData = data;
        this.thrower = thrower;

        initialized = true;
        hasLanded = false;

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 force =
            direction.normalized * data.throwForce +
            Vector3.up * data.upwardForce;

        rb.AddForce(force, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!initialized || hasLanded)
            return;

        // Ignore collision with the player who threw it.
        if (thrower != null &&
            collision.transform.root == thrower.transform.root)
            return;

        Land();
    }

    private void Land()
    {
        hasLanded = true;
        initialized = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        // Activate the effect attached to this projectile.
        MonoBehaviour[] components = GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour component in components)
        {
            if (component is IThrowableEffect effect)
            {
                effect.Activate(itemData, thrower);
                break;
            }
        }

        // Create noise at the landing position.
        if (NoiseSystem.GetInstance() != null)
        {
            NoiseSystem.GetInstance().NoiseOccur(
                NoiseSource.Player,
                transform.position,
                itemData.noiseRadius
            );
        }

        // Return projectile to the pool.
        ObjectPoolManager.Instance.Despawn(gameObject);
    }

    public void OnPoolSpawn()
    {
        initialized = false;
        hasLanded = false;
        itemData = null;
        thrower = null;
    }

    public void OnPoolDespawn()
    {
        initialized = false;
        hasLanded = false;
        itemData = null;
        thrower = null;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
