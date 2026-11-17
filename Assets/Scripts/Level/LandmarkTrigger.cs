using System.Collections.Generic;
using UnityEngine;

public class LandmarkTrigger : PlayerTrigger
{
    [Header("Enemy Spawners")]
    [SerializeField] private List<EnemySpawner> enemySpawners;

    private bool hasSpawned;

    public override void Execute(Collider other)
    {
        if (hasSpawned)
            return;

        foreach (EnemySpawner spawner in enemySpawners)
        {
            if (spawner != null)
                spawner.Spawn();
        }

        hasSpawned = true;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if (triggerCol != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, triggerCol.size);
        }
    }
}
