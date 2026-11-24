using UnityEngine;
using UnityEngine.AI;

public class ZombieAttackState : ZombieState
{
    float attackRecoveryTimer;
    public ZombieAttackState(Zombie zombie, NavMeshAgent agent, Player player) : base(zombie, agent, player) { }

    public override void Enter()
    {
        agent.isStopped = true;
        zombie.DoAttack();
        attackRecoveryTimer = zombie.attackRecovery;
    }
    public override void Loop()
    {
        if(attackRecoveryTimer > 0)
        {
            attackRecoveryTimer -= Time.deltaTime;
            if (zombie.IsInLineOfSight())
            {
                agent.SetDestination(player.transform.position);
            }
            return;
        }

        if (zombie.IsInLineOfSight())
        {
            Debug.Log("Start chasing");
            zombie.ChangeState(new ZombieChasingState(zombie, agent, player));
        }
        else
        {
            zombie.ChangeState(new ZombieRoamingState(zombie, agent, player));
        }
    }
    public override void Exit()
    {
        agent.isStopped = false;
    }
    public override void OnHeardNoise(Vector3 noisePos)
    {

    }
}
