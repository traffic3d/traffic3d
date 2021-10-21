using UnityEngine;

public class RoadNode : MonoBehaviour
{
    public const float boxPadding = 1f;
    public const float floorPadding = 0.5f;
    public const float maxFloorHeight = 10f;

    public bool startNode;
    public Vector3 floorPosition;
    public BoxCollider currentBoxColliderCheck;

    public void Start()
    {
        if (startNode)
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(transform.position, Vector3.down, out raycastHit, maxFloorHeight))
            {
                floorPosition = raycastHit.point;
            }
            else
            {
                throw new System.Exception("Unable to get floor position for start node.");
            }
        }
    }

    public bool CanSpawnVehicle(Vehicle vehicle)
    {
        BoxCollider boxCollider = vehicle.gameObject.GetComponentInChildren<BoxCollider>();
        currentBoxColliderCheck = boxCollider;
        Vector3 boxSize = GetBoxSize(boxCollider);
        Quaternion boxRotation = GetBoxRotation(boxCollider);
        Collider[] colliders = Physics.OverlapBox(GetPosition(boxSize, boxRotation), boxSize, boxRotation);
        return colliders.Length == 0;
    }

    public Vector3 GetBoxSize(BoxCollider boxCollider)
    {
        return (Vector3.Scale(boxCollider.size, boxCollider.transform.lossyScale) / 2) + new Vector3(boxPadding, boxPadding, boxPadding);
    }

    public Quaternion GetBoxRotation(BoxCollider boxCollider)
    {
        return transform.rotation * boxCollider.transform.rotation;
    }

    public Vector3 GetPosition(Vector3 boxSize, Quaternion rotation)
    {
        return floorPosition + (((rotation * boxSize).y + floorPadding) * Vector3.up);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (currentBoxColliderCheck != null)
        {
            Matrix4x4 originalMatrix = Gizmos.matrix;
            try
            {
                Vector3 boxSize = GetBoxSize(currentBoxColliderCheck);
                Quaternion boxRotation = GetBoxRotation(currentBoxColliderCheck);
                Gizmos.matrix = Matrix4x4.TRS(GetPosition(boxSize, boxRotation), boxRotation, transform.lossyScale);
                Gizmos.DrawWireCube(Vector3.zero, boxSize * 2);
            }
            finally
            {
                Gizmos.matrix = originalMatrix;
            }
        }
    }
}
