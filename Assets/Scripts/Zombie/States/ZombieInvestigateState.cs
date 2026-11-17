using UnityEngine;
using UnityEngine.AI;

public class ZombieInvestigateState : ZombieState
{
    public ZombieInvestigateState(Zombie zombie, NavMeshAgent agent, Player player) : base(zombie, agent, player) { }

    public override void Enter()
    {

    }
    public override void Loop()
    {
        if (zombie.IsInLineOfSight())
        {
            Debug.Log("Start chasing");
            zombie.ChangeState(new ZombieChasingState(zombie, agent, player));
        }
        else if (zombie.IsArrived)
        {
            zombie.ChangeState(new ZombieRoamingState(zombie, agent, player));
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
