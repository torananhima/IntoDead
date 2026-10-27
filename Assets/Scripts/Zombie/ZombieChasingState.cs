using UnityEngine;
using UnityEngine.AI;

public class ZombieChasingState : ZombieState
{
    public ZombieChasingState(Zombie zombie, NavMeshAgent agent, Player player) : base(zombie, agent, player) { }

    public override void Enter()
    {
        
    }
    public override void Loop()
    {
        float distance = Vector3.Distance(zombie.transform.position, player.transform.position);
        if (zombie.IsInLineOfSight())
        {
            agent.SetDestination(player.transform.position);
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
