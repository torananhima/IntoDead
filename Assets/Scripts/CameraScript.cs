using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] GameObject followObject;

    [SerializeField] Vector3 offset;
    [SerializeField] float camSpeed = 5f;
    
    private void Start()
    {
        if(offset == Vector3.zero) offset = transform.position;
    }
    private void Update()
    {
        if (followObject == null) return;
        Vector3 targetPos = followObject.transform.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, camSpeed * Time.deltaTime);
    }
}
