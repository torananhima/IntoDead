using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    Vector3 direction;
    int damaage;

    public void Initialize(int damage, Vector3 dir)
    {
        direction = dir.normalized;
        this.damaage = damage;
    }
    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Zombie enemy = other.GetComponent<Zombie>();
            if (enemy != null)
            {
                RangedWeaponData weaponData = GetComponentInParent<RangedWeaponData>();
                if (weaponData != null)
                {
                    enemy.TakeDamage(damaage);
                }
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }
    }
}
