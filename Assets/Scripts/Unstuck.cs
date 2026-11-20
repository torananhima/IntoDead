using UnityEngine;

public class Unstuck : MonoBehaviour
{
    [SerializeField] float unstuckDelay = 3f;
    [SerializeField] float yDefault = 0;
    [SerializeField] float rangeUnstuck = 1f;

    CharacterController controller;
    float unstuckTimer;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float unstuckY = yDefault + rangeUnstuck;
        if (transform.position.y > unstuckY || transform.position.y < -unstuckY)
        {
            unstuckTimer -= Time.deltaTime;
            if (unstuckTimer <= 0)
            {
                Pop();
            }
        }
        else if(unstuckTimer != unstuckDelay)
        {
            unstuckTimer = unstuckDelay;
        }
    }

    private void Pop()
    {
        Vector3 newPos = new Vector3(transform.position.x, yDefault, transform.position.z);
        if(controller)
        {
            controller.enabled = false;
            transform.position = newPos;
            controller.enabled = true;
        }
        else
        {
            transform.position = newPos;

        }
        unstuckTimer = unstuckDelay;
    }
}