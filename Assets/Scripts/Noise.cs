using UnityEngine;

public class Noise : MonoBehaviour
{
    [SerializeField] float spreadSpeed = 2f;
    float targetScale;
    public void Initialize(float range)
    {
        targetScale = range / 5;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale * Vector3.one, spreadSpeed * Time.deltaTime);
        if (transform.localScale.x >= targetScale - 0.001f)
        {
            Destroy(gameObject);
        }
    }
}
