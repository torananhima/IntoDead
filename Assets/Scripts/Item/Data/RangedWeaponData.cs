using UnityEngine;


public enum AmmoType
{
    Pistol,
    Rifle,
    Sniper
}

public struct AmmoContext
{
    public int pistolAmmo;
    public int rifleAmmo;
    public int sniperAmmo;

    public AmmoContext(int pistolAmmo, int rifleAmmo, int sniperAmmo)
    {
        this.pistolAmmo = pistolAmmo;
        this.rifleAmmo = rifleAmmo;
        this.sniperAmmo = sniperAmmo;
    }
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
