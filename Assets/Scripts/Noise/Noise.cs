using UnityEngine;

public class Noise : MonoBehaviour, IPoolable
{
    [SerializeField] float spreadSpeed = 2f;
    float maxDistance;
    Vector3 defaultScale = Vector3.zero;
    public void OnPoolSpawn()
    {
        if (defaultScale == Vector3.zero)
        {
            defaultScale = transform.localScale;
        }
        gameObject.SetActive(true);
    }
    public void OnPoolDespawn()
    {
        transform.localScale = defaultScale;
        gameObject.SetActive(false);
    }

    public void Initialize(float maxDistance)
    {
        this.maxDistance = maxDistance;
    }

    private void Update()
    {
        transform.localScale += Vector3.Lerp(transform.localScale, Vector3.one * maxDistance, spreadSpeed * Time.deltaTime);
        if (transform.localScale.x >= maxDistance)
        {
            ObjectPoolManager.Instance.Despawn(gameObject);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out Zombie zombie))
        {
            zombie.DetectNoise(transform.position);
        }
    }
}
