using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : Character
{
    [Header("Data")]
    [SerializeField] ZombieData data;

    [Header("Detection")]
    [SerializeField, Min(0)] private float _visionDetectClosed = 2f;
    [SerializeField, Range(0,360)] private float _visionDetectAngle = 60f;
    [SerializeField, Min(0)] private float _visionDetectRange = 5f;
    [SerializeField] private LayerMask visionObstacle;

    [Header("Roaming")]
    [SerializeField] private float _roamingDistance = 2f;
    [Min(0)][SerializeField] private Vector2 roamingDelay;

    [Header("Attack")]
    [SerializeField] private float _attackRange = 5f;
    [SerializeField] private float _attackDamage = 10f;
    [SerializeField] private float _attackRecovery = 1f;
    [SerializeField] private LayerMask attackLayer;

    [Header("Reference")]
    [SerializeField] GameObject viewModel;

    [Header("Audio")]
    [SerializeField] private AudioData attackAudio;

    [Header("Debug")]
    public ZombieStates currentActiveState = ZombieStates.Idle;
    public bool isShowDebug;
    public GameObject hitboxPrefab;

    public float visionDetectClosed
    {
        get { return _visionDetectClosed; }
        private set { _visionDetectClosed = value; }
    }
    public float visionDetectAngle
    {
        get { return _visionDetectAngle; }
        private set { _visionDetectAngle = value; }
    }
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
        private set { _attackDamage = value; }
    }
    public float attackRecovery
    {
        get { return _attackRecovery; }
        private set { _attackRecovery = value; }
    }

    public Vector3 lastNoisePos
    {
        get;
        private set;
    }
    ZombieState currentState;
    NavMeshAgent agent;
    public bool IsArrived => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    Player player;

    public bool IsInLineOfSight()
    {
        Vector3 origin = transform.position + Vector3.up * 1f;
        Vector3 target = player.transform.position + Vector3.up * 1f;

        Vector3 dir = target - origin;

        float sqrDistance = dir.sqrMagnitude;
        if (sqrDistance > visionDetectRange * visionDetectRange) return false;

        float angle = Vector3.Angle(dir, transform.forward);
        if (angle > visionDetectAngle / 2)
        {
            return false;
        }

        float distance = Mathf.Sqrt(sqrDistance);
        return !Physics.Raycast(origin, dir / distance, distance, visionObstacle);
    }

    public bool IsPlayerInCloseRange()
    {
        if((player.transform.position - transform.position).magnitude < visionDetectClosed)
        {
            if(IsThereWall(player.transform.position))
            {
                return true;
            }
        }
        return false;
    }
    public bool IsPlayerInAttackRange()
    {
        if (IsInLineOfSight())
        {
            float distance = (transform.position - player.transform.position).sqrMagnitude;
            if (distance < attackRange * attackRange)
            {
                return true;
            }
        }

        return false;
    }
    public bool IsThereWall(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - transform.position).normalized;
        if (Physics.Raycast(transform.position, dir, 30))
        {
            if (Physics.Raycast(transform.position, dir, 30, visionObstacle))
            {
                return false;
            }
            return true;
        }
        return false;
    }
    public float GetRoamingDelay()
    {
        float delay = UnityEngine.Random.Range(roamingDelay.x, roamingDelay.y);
        return delay;
    }
    protected override void Start()
    {
        base.Start();
        if(player == null) player = GameManager.player;
        if(agent == null) agent = GetComponent<NavMeshAgent>();

        if (data) Initilize(data);

        ChangeState(new ZombieIdleState(this, agent, player));
    }
    public void Initilize(ZombieData newData)
    {
        data = newData;
        maxHp = data.hp;
        hp = maxHp;
        walkSpeed = data.moveSpeed;
        runSpeed = data.runSpeed;
        roamingDistance = data.roamingDistance;
        visionDetectAngle = data.visionDetectionAngle;
        visionDetectRange = data.visionDetectionRange;
        isDead = false;

        attackDamage = data.attackDamage;
        attackRange = data.attackRange;
        attackRecovery = data.attackRecovery;
        
        agent.speed = walkSpeed;
        agent.stoppingDistance = attackRange > 1.5 ? attackRange - 1 : attackRange * 0.8f;
    }
    protected override void Update()
    {
        base.Update();
        if (currentState != null)
        {
            currentState.Loop();
        }

        bool visible = player._los.CanSee(transform);
        viewModel.SetActive(visible);
        currentVelocity = agent.velocity.magnitude;
    }

    public override void TakeDamage(int damageTaken)
    {
        base.TakeDamage(damageTaken);
    }
    protected override void Dead()
    {
        base.Dead();
        ObjectPoolManager.Instance.Despawn(gameObject);
    }

    public void ChangeState(ZombieState newState)
    {
        if (currentState != null &&
            currentState.GetType() == newState.GetType())
            return;

        if (currentState != null)currentState.Exit();
        currentState = newState;
        currentState.Enter();

        switch (newState)
        {
            case ZombieIdleState:
                currentActiveState = ZombieStates.Idle;
                break;
            case ZombieRoamingState:
                currentActiveState = ZombieStates.Roaming;
                break;
            case ZombieInvestigateState:
                currentActiveState = ZombieStates.Investigate;
                break;
            case ZombieChasingState:
                currentActiveState = ZombieStates.Chasing;
                break;
            case ZombieAttackState:
                currentActiveState = ZombieStates.Attack;
                break;
        }
    }

    public void DetectNoise(Vector3 noisePos)
    {
        lastNoisePos = noisePos;
        currentState.OnHeardNoise(lastNoisePos);
    }


    public override void ChangeMovement(MovementType newMovement)
    {
        base.ChangeMovement(newMovement);
        agent.speed = moveSpeed;
    }

    public void DoAttack()
    {
        Debug.Log("Do attack");
        Vector3 attackPos = transform.position + transform.forward * data.attackRange;
        Collider[] cols = Physics.OverlapSphere(attackPos, data.attackHitbox, attackLayer, QueryTriggerInteraction.Collide);

        SoundManager.Instance.PlaySFX(attackAudio, transform.position);

        if (isShowDebug && hitboxPrefab != null)
        {
            GameObject hitbox = Instantiate(hitboxPrefab, attackPos, Quaternion.identity);
            hitbox.transform.localScale = data.attackHitbox * Vector3.one;
            Destroy(hitbox, 3f);
        }
        foreach (Collider col in cols)
        {
            if(col.TryGetComponent(out Hurtbox hurtbox))
            {
                if(hurtbox.owner is Player)
                {
                    hurtbox.TakeDamage((int)attackDamage);
                    Debug.Log("<color=red>Hit Player</color>");
                }
            }
        }
    }



    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        //Gizmos.color = Color.aquamarine;
        //Gizmos.DrawWireSphere(transform.position, visionDetectRange);

        Gizmos.color = Color.aquamarine;
        Gizmos.DrawWireSphere(transform.position, roamingDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Vector3 startDirection = Quaternion.Euler(0, -visionDetectAngle * 0.5f, 0) * transform.forward;

        Handles.color = new Color(1f, 0.92f, 0.016f, 0.2f);
        Handles.DrawSolidArc(
            transform.position,
            transform.up,
            startDirection,
            visionDetectAngle,
            visionDetectRange
        );

        Handles.color = Color.yellow;
        Handles.DrawWireArc(
            transform.position,
            transform.up,
            startDirection,
            visionDetectAngle,
            visionDetectRange
        );

        Vector3 endDirection = Quaternion.Euler(0, visionDetectAngle * 0.5f, 0) * transform.forward;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, startDirection * visionDetectRange);
        Gizmos.DrawRay(transform.position, endDirection * visionDetectRange);
        Gizmos.DrawWireSphere(transform.position, visionDetectClosed);

#endif
    }

    private void OnDrawGizmos()
    {
        if (isShowDebug)
        {
            switch (currentActiveState)
            {
                case ZombieStates.Idle:
                    Gizmos.color = Color.white;
                    break;
                case ZombieStates.Roaming:
                    Gizmos.color = Color.aquamarine;
                    break;
                case ZombieStates.Investigate:
                    Gizmos.color = Color.blue;
                    break;
                case ZombieStates.Chasing:
                    Gizmos.color = Color.orange;
                    break;
                case ZombieStates.Attack:
                    Gizmos.color = Color.red;
                    break;
                default:
                    Gizmos.color = Color.white;
                    break;
            }

            Gizmos.DrawSphere(transform.position + Vector3.up * 3, 0.5f);
        }
    }
}