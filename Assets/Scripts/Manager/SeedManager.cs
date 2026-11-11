using UnityEngine;

public class SeedManager : MonoBehaviour
{
    public int masterSeed = 0;

    public System.Random LootRng { get; private set; }
    public System.Random EnemyRng { get; private set; }

    public static SeedManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializeSeeds(masterSeed);
    }

    public void InitializeSeeds(int seed)
    {
        if (seed == 0) masterSeed = (int)Random.value;
        else masterSeed = seed;

        LootRng = new System.Random(masterSeed + 1000);
        EnemyRng = new System.Random(masterSeed + 2000);
    }
}
