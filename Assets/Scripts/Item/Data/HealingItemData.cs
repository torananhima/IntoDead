using UnityEngine;

[CreateAssetMenu(fileName = "HealingItemData", menuName = "ItemData/HealingItemData")]
public class HealingItemData : ItemData
{
    [SerializeField] private float healingAmount;
    [SerializeField] private float usedDuration;

    public float GetHealingAmount()
    {
        return healingAmount;
    }
}
