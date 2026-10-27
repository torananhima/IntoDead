using UnityEngine;
using UnityEngine.AI;

public class Zombie : Character
{
    [Header("Data")]
    [SerializeField] ZombieData data;

    [Header("Detection")]
    [SerializeField] private float _visionDetectRange = 5f;

    [Header("Idle")]
    [SerializeField] private float _roamingDistance = 2f;

    [Header("Attack")]
    [SerializeField] private float _attackRange = 5f;
    [SerializeField] private float _attackDamage = 10f;

    public float visionDetectRange
    {
        get { return _visionDetectRange; }
        private set { _visionDetectRange = value; }
    }
    public float roamingDistance
    {
        get { return _roamingDistance; }
        private set { _roamingDistance = value; }
    }
    public float attackRange
    {
        get { return _attackRange; }
        private set { _attackRange = value; }
    }
    public float attackDamage
    {
        get { return _attackDamage; }
        private set {  _attackRange = value; }
    }

    public Vector3 lastNoisePos
    {
        get;
        private set;
    }
    ZombieState currentState;
    NavMeshAgent agent;
    Player player;

    public bool IsInLineOfSight()
    {
        Vector3 dir = (player.transform.position - transform.position).normalized;
        return Physics.Raycast(transform.position, dir, visionDetectRange);
    }
    protected override void Start()
    {
        base.Start();
        player = GameManager.player;
        agent = GetComponent<NavMeshAgent>();

        if (data) Initilize(data);

        ChangeState(new ZombieChasingState(this, agent, player));
    }
    public void Initilize(ZombieData newData)
    {
        data = newData;
        maxHp = data.hp;
        hp = maxHp;
        walkSpeed = data.moveSpeed;
        roamingDistance = data.roamingDistance;
        visionDetectRange = data.visionDetectionRange;
        
        agent.speed = walkSpeed;
    }
    private void Update()
    {
        if (currentState != null)
        {
            currentState.Loop();
        }

        bool visible = player._los.CanSee(transform);
    }

    public void ChangeState(ZombieState newState)
    {
        if (currentState == newState) return;

        if(currentState != null)currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void DetectNoise(Vector3 noisePos)
    {
        lastNoisePos = noisePos;
        currentState.OnHeardNoise(lastNoisePos);
    }

    public void ChangeMovement(bool isRunning)
    {
        isRunning = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.aquamarine;
        Gizmos.DrawWireSphere(transform.position, visionDetectRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, roamingDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}