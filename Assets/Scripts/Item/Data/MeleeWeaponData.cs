using UnityEngine;

[CreateAssetMenu(fileName = "MeleeWeaponData", menuName = "Scriptable Objects/ItemData/MeleeWeaponData")]
public class MeleeWeaponData : ItemData
{
    [SerializeField]private float damage;
    [SerializeField]private float attackRange;
    [SerializeField]private float attackCooldown;
    [SerializeField]private float noiseRadius;

    public void CreatAttack()
    {

    }
}
