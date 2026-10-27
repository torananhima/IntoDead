using UnityEngine;

[CreateAssetMenu(fileName = "ZombieData", menuName = "ZombieData")]
public class ZombieData : ScriptableObject
{
    public int hp = 10;
    public int moveSpeed = 10;
    public float visionDetectionRange = 5f;
    public float roamingDistance = 2f;
    public float attackRange = 3f;
    public int attackDamage = 5;
}
