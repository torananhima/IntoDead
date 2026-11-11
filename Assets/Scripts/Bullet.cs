
using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable
{
    [Header("Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 5f;

    private Vector3 direction;
    private int damage;
    private float lifeTimer;
    private bool initialized;

    public void Initialize(int damage, Vector3 dir)
    {
        this.damage = damage;
        direction = dir.normalized;

        lifeTimer = lifetime;
        initialized = direction.sqrMagnitude > 0f;

        if (initialized)
            transform.forward = direction;
    }

    private void Update()
    {
        if (!initialized)
            return;

        transform.position += direction * speed * Time.deltaTime;

        lifeTimer -= Time.deltaTime;

        if (lifeTimer <= 0f)
            Despawn();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!initialized)
            return;

        // Check whether the collider belongs to a zombie.
        Zombie enemy = other.GetComponentInParent<Zombie>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Despawn();
            return;
        }

        // Otherwise, check for environment collision.
        if (other.CompareTag("Environment"))
        {
            Despawn();
        }
    }

    private void Despawn()
    {
        if (!initialized)
            return;

        initialized = false;

        if (ObjectPoolManager.Instance != null)
            ObjectPoolManager.Instance.Despawn(gameObject);
    }

    public void OnPoolSpawn()
    {
        initialized = false;
        lifeTimer = lifetime;
        damage = 0;
        direction = Vector3.zero;
    }

    public void OnPoolDespawn()
    {
        initialized = false;
        lifeTimer = lifetime;
        damage = 0;
        direction = Vector3.zero;
    }
}
