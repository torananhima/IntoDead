using UnityEngine;

public class Noise : MonoBehaviour
{
    [SerializeField] float spreadSpeed = 2f;
    float maxDistance;
    public void Initialize(float maxDistance)
    {
        this.maxDistance = maxDistance;
    }

    private void Update()
    {
        transform.localScale += Vector3.Lerp(transform.localScale, Vector3.one * maxDistance, spreadSpeed * Time.deltaTime);
        if (transform.localScale.x >= maxDistance)
        {
            Destroy(gameObject);
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
