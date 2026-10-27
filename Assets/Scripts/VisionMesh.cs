
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class VisionMesh : MonoBehaviour
{
    [Header("Vision")]
    [SerializeField] private float radius = 7f;
    [SerializeField, Range(30, 360)]
    private int rayCount = 180;

    [Header("Obstacles")]
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float eyeHeight = 1f;

    [Header("Update")]
    [SerializeField] private float updateInterval = 0.05f;

    private Mesh visionMesh;
    private MeshFilter meshFilter;
    private float timer;

    private Vector3[] vertices;
    private int[] triangles;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();

        visionMesh = new Mesh();
        visionMesh.name = "Player Vision Mesh";
        meshFilter.sharedMesh = visionMesh;

        // One center vertex + one vertex per ray + closing vertex.
        vertices = new Vector3[rayCount + 2];
        triangles = new int[rayCount * 3];

        for (int i = 0; i < rayCount; i++)
        {
            int tri = i * 3;
            triangles[tri] = 0;
            triangles[tri + 1] = i + 1;
            triangles[tri + 2] = i + 2;
        }
    }

    private void LateUpdate()
    {
        timer += Time.deltaTime;

        if (timer < updateInterval)
            return;

        timer = 0f;
        RebuildMesh();
    }

    private void RebuildMesh()
    {
        Vector3 origin = transform.position;

        // The mesh is on the XZ plane.
        vertices[0] = Vector3.zero;

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = i * 360f / rayCount;

            Vector3 direction = new Vector3(
                Mathf.Sin(angle * Mathf.Deg2Rad),
                0f,
                Mathf.Cos(angle * Mathf.Deg2Rad)
            );

            Vector3 worldPoint;

            if (Physics.Raycast(
                origin + Vector3.up * eyeHeight,
                direction,
                out RaycastHit hit,
                radius,
                obstacleMask,
                QueryTriggerInteraction.Ignore))
            {
                worldPoint = hit.point;
            }
            else
            {
                worldPoint = origin + direction * radius;
            }

            // Store mesh vertices relative to VisionOrigin.
            Vector3 localPoint =
                transform.InverseTransformPoint(worldPoint);

            localPoint.y = 0f;
            vertices[i + 1] = localPoint;
        }

        visionMesh.Clear();
        visionMesh.vertices = vertices;
        visionMesh.triangles = triangles;
        visionMesh.RecalculateBounds();
        visionMesh.RecalculateNormals();
    }

    private void OnDestroy()
    {
        if (visionMesh != null)
            Destroy(visionMesh);
    }
}
