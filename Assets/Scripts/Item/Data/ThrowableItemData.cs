
using UnityEngine;

[CreateAssetMenu(fileName = "ThrowableItemData", menuName = "ItemData/ThrowableItemData")]
public class ThrowableItemData : ItemData
{
    [Header("Projectile")]
    public GameObject projectilePrefab;

    [Header("Throw Settings")]
    public float throwForce = 10f;
    public float upwardForce = 3f;

    [Header("Effect Settings")]
    public float effectDuration = 3f;
    public float noiseRadius = 8f;
}
