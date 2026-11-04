using UnityEngine;

[CreateAssetMenu(fileName = "RangedWeaponData", menuName = "Scriptable Objects/ItemData/RangedWeaponData")]
public class RangedWeaponData : ItemData
{
    [SerializeField]public float damage;
    [SerializeField]private float range;
    [SerializeField]private int magazineSize;
    [SerializeField]private float reloadDuration;
    [SerializeField]private float noiseRadius;

    public void CreateShot()
    {
        //Instantiate(bulletPrefab);
    }
}
