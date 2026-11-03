using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] GameObject followObject;
    Vector3 camOffset;
    [SerializeField] float camSpeed = 5f;
    
    private void Start()
    {
        camOffset = transform.position;
    }
    private void Update()
    {
        if (followObject == null) return;
        Vector3 targetPos = followObject.transform.position + camOffset;
        transform.position = Vector3.Lerp(transform.position, targetPos, camSpeed * Time.deltaTime);
    }
}
