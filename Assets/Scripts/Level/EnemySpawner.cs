using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField, Min(0)] private int minSpawnCount = 1;
    [SerializeField, Min(0)] private int maxSpawnCount = 3;

    [Header("Spawn Points")]
    [SerializeField] private List<Transform> spawnPoints;

    public void Spawn()
    {
        if (zombiePrefab == null)
        {
            Debug.LogWarning("Zombie prefab is missing!", this);
            return;
        }

        // Remove null and duplicate references.
        List<Transform> availablePoints = new();
        HashSet<Transform> uniquePoints = new();

        foreach (Transform point in spawnPoints)
        {
            if (point != null && uniquePoints.Add(point))
                availablePoints.Add(point);
        }

        if (availablePoints.Count == 0)
            return;

        int min = Mathf.Clamp(minSpawnCount, 0, availablePoints.Count);
        int max = Mathf.Clamp(maxSpawnCount, min, availablePoints.Count);

        int spawnCount = Random.Range(min, max + 1);

        for (int i = 0; i < spawnCount; i++)
        {
            int randomIndex = Random.Range(0, availablePoints.Count);
            Transform point = availablePoints[randomIndex];

            // Remove selected point so it cannot be selected again.
            availablePoints.RemoveAt(randomIndex);

            ObjectPoolManager.Instance.Spawn(zombiePrefab, point.position, point.rotation).transform.SetParent(transform);
        }
    }
}
