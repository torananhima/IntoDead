using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootEntry", menuName = "LootData/LootEntry")]
public class LootEntry : ScriptableObject
{
    public List<ItemEntry> items = new();
}

[System.Serializable]
public class ItemEntry
{
    public ItemData item;

    [Min(0.01f)]
    public float weight = 1f;

    [Header("Amount")]
    [Min(1)]
    public int minAmount = 1;

    [Min(1)]
    public int maxAmount = 1;

    [Header("Weapon Magazine Ammo")]
    [Min(0)]
    public int minMagazineAmmo = 0;

    [Min(0)]
    public int maxMagazineAmmo = 0;
}