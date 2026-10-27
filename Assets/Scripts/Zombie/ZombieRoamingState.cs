using UnityEngine;
using UnityEngine.AI;

public class ZombieRoamingState : ZombieState
{
    public ZombieRoamingState(Zombie zombie, NavMeshAgent agent, Player player) : base(zombie, agent, player) { }

    public override void Enter()
    {

    }
    public override void Loop()
    {
        float distance = Vector3.Distance(zombie.transform.position, player.transform.position);
        if (distance < zombie.visionDetectRange)
        {
            if (zombie.IsInLineOfSight())
            {
                zombie.ChangeState(new ZombieChasingState(zombie, agent, player));
            }
        }
        else if(agent.isStopped)
        {
            zombie.ChangeState(new ZombieIdleState(zombie, agent, player));
        }
    }
    public override void Exit()
    {

    }
    public override void OnHeardNoise(Vector3 noisePos)
    {
        agent.SetDestination(noisePos);
    }
}
