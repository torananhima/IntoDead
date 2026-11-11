using UnityEngine;


public enum AmmoType
{
    Pistol,
    Rifle,
    Sniper
}
[CreateAssetMenu(fileName = "RangedWeaponData", menuName = "ItemData/RangedWeaponData")]
public class RangedWeaponData : ItemData
{
    public AmmoType ammoType;
    public int damage;
    public int magazineSize;
    [Header("Firing")]
    [Min(0f)]
    public float fireCooldown = 0.2f;

    [Header("Reloading")]
    [Min(0f)]
    public float reloadDuration = 1.5f;
    public float noiseRadius;
    public GameObject bulletPrefab;
}
