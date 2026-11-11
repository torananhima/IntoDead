using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ContainerData", menuName = "Scriptable Objects/ContainerData")]
public class ContainerData : ScriptableObject
{
    public List<LootEntry> lootEntries = new();
}