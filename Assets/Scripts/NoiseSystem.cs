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
    [SerializeField] private static LayerMask zombieLayer;
    private static NoiseSystem instance;

    public static NoiseSystem GetInstance()
    {
        if (instance == null)
        {
            instance = new NoiseSystem();
        }
        return instance;
    }
    public static void NoiseOccur(NoiseSource source, Vector3 sourcePos, float noiseRange)
    {
        if (source == NoiseSource.Zombie) return;
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
