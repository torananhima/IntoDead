using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ZombieInvestigateState : ZombieState
{
    float investigateTimer = 10f;
    float roamingDelayTimer;
    Vector3 noisePos;
    public ZombieInvestigateState(Zombie zombie, NavMeshAgent agent, Player player) : base(zombie, agent, player) { }

    public override void Enter()
    {
        investigateTimer = 10f;
        noisePos = agent.destination;
        SetDelay();
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
            if (investigateTimer <= 0)
            {
                zombie.ChangeState(new ZombieRoamingState(zombie, agent, player));
            }
            else
            if (roamingDelayTimer <= 0)
            {
                SetRandomRoamingDestination();

                SetDelay();
            }
        }

        if (roamingDelayTimer > 0)
        {
            roamingDelayTimer -= Time.deltaTime;
        }
        if (investigateTimer > 0)
        {
            investigateTimer -= Time.deltaTime;
        }
    }
    public override void Exit()
    {

    }
    public override void OnHeardNoise(Vector3 noisePos)
    {
        agent.SetDestination(noisePos);
        investigateTimer = 10f;
        noisePos = agent.destination;
        SetDelay();
    }

    private void SetRandomRoamingDestination()
    {
        bool noWall = false;
        Vector3 newRoamingPos = noisePos;
        float distance = zombie.roamingDistance;
        int attempt = 0;
        while (!noWall || attempt > 20)
        {
            Vector2 randomPos = Random.insideUnitCircle * distance;

            newRoamingPos.x += randomPos.x;
            newRoamingPos.z += randomPos.y;
            if (zombie.IsThereWall(newRoamingPos))
            {
                newRoamingPos = noisePos;
            }
            else
            {
                noWall = true;
            }
            attempt++;
            if (attempt > 10)
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
