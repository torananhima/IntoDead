using UnityEngine;
using UnityEngine.AI;

public class ZombieRoamingState : ZombieState
{
    float roamingDelayTimer;
    public ZombieRoamingState(Zombie zombie, NavMeshAgent agent, Player player) : base(zombie, agent, player) { }

    public override void Enter()
    {
        roamingDelayTimer = zombie.GetRoamingDelay();
        SetRandomRoamingDestination();
    }
    public override void Loop()
    {
        if (zombie.IsInLineOfSight())
        {
            Debug.Log("Start chasing");
            zombie.ChangeState(new ZombieChasingState(zombie, agent, player));   
        }else if(zombie.IsArrived)
        {
            if(roamingDelayTimer <= 0)
            {
                SetRandomRoamingDestination();

                SetDelay();
            }
        }
        if(roamingDelayTimer > 0)
        {
            roamingDelayTimer -= Time.deltaTime;
        }
    }
    public override void Exit()
    {

    }
    public override void OnHeardNoise(Vector3 noisePos)
    {
        agent.SetDestination(noisePos);
        zombie.ChangeState(new ZombieInvestigateState(zombie, agent, player));
    }

    private void SetRandomRoamingDestination()
    {
        bool noWall = false;
        Vector3 newRoamingPos = zombie.transform.position;
        float distance = zombie.roamingDistance;
        int attempt = 0;
        while (!noWall || attempt > 20)
        {
            Vector2 randomPos = Random.insideUnitCircle * distance;

            newRoamingPos.x += randomPos.x;
            newRoamingPos.z += randomPos.y;
            if (zombie.IsThereWall(newRoamingPos))
            {
                newRoamingPos = zombie.transform.position;
            }
            else
            {
                noWall = true;
            }
            attempt++;
            if(attempt > 10)
            {
                distance /= 2;
            }
        }
        agent.SetDestination(newRoamingPos);
    }

    private void SetDelay()
    {
        roamingDelayTimer = zombie.GetRoamingDelay();
    }
}
