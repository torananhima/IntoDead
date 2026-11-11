using UnityEngine;

[CreateAssetMenu(fileName = "MeleeWeaponData", menuName = "ItemData/MeleeWeaponData")]
public class MeleeWeaponData : ItemData
{
    [Header("Attack")]
    public int damage = 20;
    public float attackRange = 1.5f;
    public float hitboxRadius = 1f;
    [Range(0f, 360f)]
    public float attackAngle = 120f;
    public float attackCooldown = 0.8f;
    public LayerMask attackLayer;

    [Header("Noise")]
    public float noiseRadius = 5f;
}
