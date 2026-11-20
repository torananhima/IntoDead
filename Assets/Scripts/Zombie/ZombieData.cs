using UnityEngine;

[CreateAssetMenu(fileName = "ZombieData", menuName = "ZombieData")]
public class ZombieData : ScriptableObject
{
    public int hp = 10;
    public int moveSpeed = 3;
    public int runSpeed = 9;
    public float visionDetectionAngle = 60f;
    public float visionDetectionRange = 5f;
    public float roamingDistance = 2f;
    public float attackRange = 1f;
    public float attackHitbox = 2f;
    public int attackDamage = 5;
    public float attackRecovery = 1f;
}
