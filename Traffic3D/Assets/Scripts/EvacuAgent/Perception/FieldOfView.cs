using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Range(0, 360)]
    public float viewAngle = 90;

    [SerializeField]
    public LayerMask targetLayer;

    public float findAgentsDelay = 1;
    public List<Pedestrian> visiblePedestrians = new List<Pedestrian>();
    public GameObject nearestObstacleAhead { get; private set; }
    public List<GameObject> peripheralGameObjects { get; private set; }
    public float viewRadius;
    public float numberOfSegments = 21;

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
    private List<float> anglesForPeripheralVision; // Used for more realistic object avoidance when moving near to objects that are not directly ahead
    private float aheadObstacleAvoidDistance;
    private float peripheralObstacleAvoidDistance;
    private List<KeyValuePair<Vector3, Vector3>> lineStartEndPoints; // Used for Gizmo based debugging
    private bool isGizmoDebuggingActive = true; // Turn on to use gizmo debugging

    void Start()
    {
        obstacleBitmask = 1 << LayerMask.NameToLayer(obstacleLayer);
        mesh = gameObject.GetComponent<MeshFilter>().mesh;
        BuildMesh();
        StartCoroutine(FindAgentsAndObstaclesWithDelay());
        peripheralGameObjects = new List<GameObject>();
        lineStartEndPoints = new List<KeyValuePair<Vector3, Vector3>>();

        anglesForPeripheralVision = new List<float>
        {
            270f,
            315f,
            45f,
            90f
        };

        // These values were the best after several rounds of paramater tuning
        aheadObstacleAvoidDistance = viewRadius * 0.2f;
        peripheralObstacleAvoidDistance = viewRadius * 0.13f;

        // For debugging
        StartCoroutine(ClearLineStartEndsAfterTime());
    }

    public IEnumerator FindAgentsAndObstaclesWithDelay()
    {
        while (true)
        {
            yield return new WaitForSeconds(findAgentsDelay);
            GetAllAgentsInViewAngle();
            FindNearestObstacleAhead();
            FindPeripheralObstacles();
        }
    }

    public Vector3 DirectionFromAngle(float angleInDegrees)
    {
        angleInDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void GetAllAgentsInViewAngle()
    {
        visiblePedestrians.Clear();
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
                    visiblePedestrians.Add(agentTransform.GetComponentInParent<Pedestrian>());
                }
            }
        }
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

    private void FindNearestObstacleAhead()
    {
        nearestObstacleAhead = null;

        Vector3 endPoint = transform.position + (transform.forward * aheadObstacleAvoidDistance);
        lineStartEndPoints.Add(new KeyValuePair<Vector3, Vector3>(transform.position, endPoint));

        RaycastHit raycastHit;
        if (Physics.Raycast(transform.position, transform.forward, out raycastHit, aheadObstacleAvoidDistance, obstacleBitmask))
        {
            nearestObstacleAhead = raycastHit.collider.gameObject;
        }
    }

    private void FindPeripheralObstacles()
    {
        peripheralGameObjects.Clear();

        foreach(float angle in anglesForPeripheralVision)
        {
            Vector3 rayDirection = Quaternion.Euler(0f, angle, 0f) * transform.forward;
            Vector3 endPoint = transform.position + (rayDirection * peripheralObstacleAvoidDistance);
            lineStartEndPoints.Add(new KeyValuePair<Vector3, Vector3>(transform.position, endPoint));

            RaycastHit raycastHit;
            if (Physics.Raycast(transform.position, rayDirection, out raycastHit, peripheralObstacleAvoidDistance, obstacleBitmask))
            {
                if (!IsObstacleObjectAlreadySeen(raycastHit.collider.gameObject))
                    peripheralGameObjects.Add(raycastHit.collider.gameObject);
            }
        }
    }

    private bool IsObstacleObjectAlreadySeen(GameObject obstacleObject)
    {
        if (nearestObstacleAhead != null && nearestObstacleAhead.Equals(obstacleObject))
            return true;

        if (peripheralGameObjects.Contains(obstacleObject))
            return true;

        return false;
    }

    private IEnumerator ClearLineStartEndsAfterTime()
    {
        while (true)
        {
            // Time is arbitrary
            yield return new WaitForSeconds(3);
            lineStartEndPoints.Clear();
        }
    }

    private void OnDrawGizmos()
    {
        if (!isGizmoDebuggingActive)
            return;

        foreach (KeyValuePair<Vector3, Vector3> lineStartEnd in lineStartEndPoints)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(lineStartEnd.Key, lineStartEnd.Value);
            Gizmos.color = Color.white;
        }
    }
}
