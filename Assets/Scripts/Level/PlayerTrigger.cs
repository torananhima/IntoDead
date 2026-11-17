using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public abstract class PlayerTrigger : MonoBehaviour
{
    [SerializeField] bool isDrawGizmo;
    [SerializeField] protected bool isOnce;
    protected BoxCollider triggerCol;
    private void Start()
    {
        triggerCol = GetComponent<BoxCollider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Execute(other);

            if (isOnce) triggerCol.enabled = false;
        }
    }

    public abstract void Execute(Collider other);
    protected virtual void OnDrawGizmos()
    {
        if (!isDrawGizmo) return;
    }
}
