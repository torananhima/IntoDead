using UnityEngine;

public class Hurtbox : MonoBehaviour, IDamageable
{
    Character owner;

    private void Awake()
    {
        owner = GetComponentInParent<Character>();
    }
    public void TakeDamage(int damage)
    {
        owner.TakeDamage(damage);
    }
}
