using UnityEngine;

public interface IState
{
    public void Enter();
    public void Loop();
    public void Exit();
}
