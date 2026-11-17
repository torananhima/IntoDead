using UnityEngine;
using UnityEngine.AI;

public class ZombieChasingState : ZombieState
{
    float attackRange;
    public ZombieChasingState(Zombie zombie, NavMeshAgent agent, Player player) : base(zombie, agent, player) { }

    public override void Enter()
    {
        attackRange = zombie.attackRange;
    }
    public override void Loop()
    {
        if (zombie.IsInLineOfSight())
        {
            if (zombie.IsPlayerInAttackRange())
            {
                zombie.ChangeState(new ZombieAttackState(zombie, agent, player));
            }
            agent.SetDestination(player.transform.position);
        }
        else if(zombie.IsArrived)
        {
            zombie.ChangeState(new ZombieRoamingState(zombie, agent, player));
        }
    }
    public override void Exit()
    {

    }
    public override void OnHeardNoise(Vector3 noisePos)
    {

    }
}
