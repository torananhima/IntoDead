using UnityEngine;

public enum NoiseSource
{
    Player,
    Zombie,
    Item,
    Environment
}

public class NoiseSystem : MonoBehaviour
{
    [Header("Prototype")]
    [SerializeField] private Noise noisePrefab;
    [SerializeField] private LayerMask zombieLayer;
    
    private static NoiseSystem instance;
    
    public static NoiseSystem GetInstance()
    {
        if (instance == null)
        {
            instance = new NoiseSystem();
        }
        return instance;
    }
    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void NoiseOccur(NoiseSource source, Vector3 sourcePos, float noiseRange)
    {
        if (source == NoiseSource.Zombie) return;
        Noise noise = Instantiate(noisePrefab, sourcePos, Quaternion.identity);
        noise.Initialize(noiseRange);
        Collider[] hits = Physics.OverlapSphere(sourcePos, noiseRange, zombieLayer);
        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent(out Zombie zombie))
            {
                zombie.DetectNoise(sourcePos);
            }
        } 
    }
}
