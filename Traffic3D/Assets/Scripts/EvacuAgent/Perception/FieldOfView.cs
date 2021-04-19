using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Range(0, 360)]
    public float viewAngle = 90;

    [SerializeField]
    public LayerMask targetLayer;

    public float findAgentsDelay = 0.1f;
    public List<Pedestrian> allVisiblePedestrians = new List<Pedestrian>();
    public float viewRadius;
    public float numberOfSegments = 21;
    private float peripheralRayDistance = 5f;

    private const string obstacleLayer = "Obstacle";
    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3[] normals;
    private Vector2[] uvs;
    private int[] triangles;

    private float segmentAngle;
    private const float heightOfFieldOfView = 0.1f;
    private const float opacityFieldOfView = 0.1f;
    private float convertedAngle;
    private int obstacleBitmask;

    private bool isDebugginOn = false;

    void Start()
    {
        obstacleBitmask = 1 << LayerMask.NameToLayer(obstacleLayer);
        mesh = gameObject.GetComponent<MeshFilter>().mesh;
        BuildMesh();
        StartCoroutine(FindAgentsAndObstaclesWithDelay());
    }

    public IEnumerator FindAgentsAndObstaclesWithDelay()
    {
        while (true)
        {
            yield return new WaitForSeconds(findAgentsDelay);
            GetAllAgentsInViewAngle();
        }
    }

    public Vector3 DirectionFromAngle(float angleInDegrees)
    {
        angleInDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void GetAllAgentsInViewAngle()
    {
        allVisiblePedestrians.Clear();
        Collider[] agentsInRadius = Physics.OverlapSphere(transform.position, viewRadius, targetLayer);

        for (int index = 0; index < agentsInRadius.Length; index++)
        {
            Transform agentTransform = agentsInRadius[index].transform;
            Vector3 angleToAgent = (agentTransform.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, angleToAgent) < viewAngle / 2)
            {
                float distanceToAgent = Vector3.Distance(transform.position, agentTransform.position);

                if(distanceToAgent > viewRadius)
                    continue;

                // Ensure pedestrain does not detect self
                if (distanceToAgent == 0f)
                    continue;

                RaycastHit raycastHit;
                if (!Physics.Raycast(transform.position, angleToAgent, out raycastHit, distanceToAgent, obstacleBitmask))
                {
                    Debug.DrawRay(transform.position, agentTransform.position);
                    allVisiblePedestrians.Add(agentTransform.GetComponentInParent<Pedestrian>());
                }
            }
        }
    }

    public List<RaycastHit> GetPeripheralObjects()
    {
        Vector3[] leftAndRight = new Vector3[] { transform.right * -1, transform.right };

        List<RaycastHit> raycastHits = new List<RaycastHit>();

        foreach(Vector3 direction in leftAndRight)
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(transform.position, direction, out raycastHit, peripheralRayDistance, obstacleBitmask))
            {
                raycastHits.Add(raycastHit);
            }
        }

        return raycastHits;
    }

    private void BuildMesh()
    {
        InitialiseArrays();
        CreateVertices();
        CreateUVCoordinates();
        CreateTriangles();
        this.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, opacityFieldOfView);
    }

    private void InitialiseArrays()
    {
        vertices = new Vector3[(int)numberOfSegments * 3];
        normals = new Vector3[(int)numberOfSegments * 3];
        triangles = new int[(int)numberOfSegments * 3];
        uvs = new Vector2[(int)numberOfSegments * 3];
    }

    private void CreateVertices()
    {
        // In unity 0 degrees is east and travels anti-clockwise. The 90.0f is to account for this
        convertedAngle = 90.0f - viewAngle / 2;
        segmentAngle = viewAngle / numberOfSegments;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(0, heightOfFieldOfView, 0);
            normals[i] = Vector3.up;
        }

        float temporaryAngle = convertedAngle;

        for (int i = 1; i < vertices.Length; i += 3)
        {
            vertices[i] = new Vector3(Mathf.Cos(Mathf.Deg2Rad * temporaryAngle) * viewRadius, heightOfFieldOfView, Mathf.Sin(Mathf.Deg2Rad * temporaryAngle) * viewRadius);

            temporaryAngle += segmentAngle;

            vertices[i + 1] = new Vector3(Mathf.Cos(Mathf.Deg2Rad * temporaryAngle) * viewRadius, heightOfFieldOfView, Mathf.Sin(Mathf.Deg2Rad * temporaryAngle) * viewRadius);

            normals[i] = Vector3.up;
            normals[i + 1] = Vector3.up;
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
    }

    private void CreateUVCoordinates()
    {
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        mesh.uv = uvs;
    }

    private void CreateTriangles()
    {
        for (int i = 0; i < triangles.Length; i += 3)
        {
            triangles[i] = 0;
            triangles[i + 1] = i + 2;
            triangles[i + 2] = i + 1;
        }

        mesh.normals = normals;
        mesh.triangles = triangles;
    }
}
