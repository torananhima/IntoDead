using UnityEngine;
using UnityEngine.AI;

public abstract class ZombieState : IState
{
    protected Zombie zombie;
    protected NavMeshAgent agent;
    protected Player player;
    public ZombieState(Zombie zombie, NavMeshAgent agent, Player player)
    {
        this.zombie = zombie;
        this.agent = agent;
        this.player = player;
    }

    public abstract void Enter();
    public abstract void Loop();
    public abstract void Exit();
    public abstract void OnHeardNoise(Vector3 noisePos);
}
