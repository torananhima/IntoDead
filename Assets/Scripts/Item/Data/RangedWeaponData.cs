using UnityEngine;


public enum AmmoType
{
    Pistol,
    Rifle,
    Shotgun,
    Sniper
}
[CreateAssetMenu(fileName = "RangedWeaponData", menuName = "Scriptable Objects/ItemData/RangedWeaponData")]
public class RangedWeaponData : ItemData
{
    public AmmoType ammoType;
    public int damage;
    [SerializeField]private float range;
    [SerializeField]private int magazineSize;
    [SerializeField]private float reloadDuration;
    [SerializeField]private float noiseRadius;
    public GameObject bulletPrefab;
}
