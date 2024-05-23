//using UnityEngine;

//public class OceanMovement : MonoBehaviour
//{
//    public float waveSpeed = 1.0f;
//    public float waveHeight = 0.01f;
//    public float waveFrequency = 1.0f;

//    private MeshFilter meshFilter;
//    private Vector3[] baseVertices;

//    void Start()
//    {
//        meshFilter = GetComponent<MeshFilter>();
//        if (meshFilter != null)
//        {
//            baseVertices = meshFilter.mesh.vertices;
//        }
//    }

//    void Update()
//    {
//        if (meshFilter != null)
//        {
//            Vector3[] vertices = new Vector3[baseVertices.Length];
//            for (int i = 0; i < vertices.Length; i++)
//            {
//                Vector3 vertex = baseVertices[i];
//                vertex.y += Mathf.Sin(Time.time * waveSpeed + vertex.x * waveFrequency) * waveHeight;
//                vertices[i] = vertex;
//            }

//            Mesh mesh = meshFilter.mesh;
//            mesh.vertices = vertices;
//            mesh.RecalculateNormals();
//        }
//    }
//}


using UnityEngine;

public class OceanMovement : MonoBehaviour
{
    public float waveSpeed = 1.0f;
    public float waveHeight = 0.5f;
    public float waveFrequency = 1.0f;
    public float moveSpeed = 3f;
    public Vector3 moveDirection = Vector3.forward;

    private MeshFilter meshFilter;
    private Vector3[] baseVertices;
    private Vector3 initialPosition;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            baseVertices = meshFilter.mesh.vertices;
        }
        initialPosition = transform.position;
    }

    void Update()
    {
        if (meshFilter != null)
        {
            Vector3[] vertices = new Vector3[baseVertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vertex = baseVertices[i];
                vertex.y += Mathf.Sin(Time.time * waveSpeed + vertex.x * waveFrequency) * waveHeight;
                vertices[i] = vertex;
            }

            Mesh mesh = meshFilter.mesh;
            mesh.vertices = vertices;
            mesh.RecalculateNormals();
        }

        // Move the plane in the specified direction
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Reset position if needed (optional)
        if (Vector3.Distance(initialPosition, transform.position) > 50.0f) // Adjust the distance as needed
        {
            transform.position = initialPosition;
        }
    }
}
