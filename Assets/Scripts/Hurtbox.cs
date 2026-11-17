using UnityEngine;

public class Hurtbox : MonoBehaviour, IDamageable
{
    public Character owner {  get; private set; }

    private void Awake()
    {
        owner = GetComponentInParent<Character>();
    }
    public void TakeDamage(int damage)
    {
        owner.TakeDamage(damage);
    }
}
