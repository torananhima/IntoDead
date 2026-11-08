using UnityEngine;

[System.Serializable]
public class ObjectPoolConfig
{
    public GameObject prefab;

    [Min(0)]
    public int prewarmCount = 5;

    [Min(1)]
    public int maxRetained = 100;
}
