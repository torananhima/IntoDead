using UnityEngine;

public class FinishTrigger : PlayerTrigger
{
    bool isFinish;
    public override void Execute(Collider other)
    {
        if (isFinish) return;
        //Game End!
        isFinish = true;
        Debug.Log("FINISH");
        GameManager.OnGameEnd?.Invoke();
    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if (triggerCol != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, triggerCol.size);
        }
    }
}
