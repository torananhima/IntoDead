using UnityEngine;
using UnityEngine.AI;

public class ZombieIdleState : ZombieState
{
    public ZombieIdleState(Zombie zombie, NavMeshAgent agent, Player player) : base(zombie, agent, player) { }

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
            Vector3 newRoamingPos = zombie.transform.position;
            Vector2 randomPos = Random.insideUnitCircle * zombie.roamingDistance;
            newRoamingPos.x += randomPos.x;
            newRoamingPos.y += randomPos.y;
        }
    }
    public override void Exit()
    {

    }
    public override void OnHeardNoise(Vector3 noisePos)
    {
        zombie.ChangeState(new ZombieRoamingState(zombie, agent, player));
    }
}
