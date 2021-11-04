using UnityEngine;
using UnityEngine.Events;

public class VehicleNavigation : MonoBehaviour
{
    private VehicleSettings vehicleSettings;
    public VehiclePath path;
    public Transform currentNode;
    public int currentNodeNumber;
    public RoadNodeUnityEvent nextNodeEvent = new RoadNodeUnityEvent();

    private const float targetSteerAngle = 0;
    private const float checkNextNodeDistance = 3f;
    private const float debugSphereSize = 0.25f;

    public void Awake()
    {
        vehicleSettings = gameObject.GetComponent<VehicleSettings>();
    }

    /// <summary>
    /// Method to check if vehicle is at the destination
    /// </summary>
    /// <returns>True if it at destination</returns>
    public bool IsAtDestination()
    {
        return currentNodeNumber == path.nodes.Count - 1;
    }

    /// <summary>
    /// Checks for the next node and assigns the next node if present
    /// </summary>
    /// <returns>True if assigned the next node</returns>
    public bool CheckNextNode()
    {
        if (Vector3.Distance(gameObject.transform.TransformPoint(0, 0, vehicleSettings.nodeReadingOffset), currentNode.position) < checkNextNodeDistance)
        {
            NextNode();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Applies the steer of the vehicle for this current period of time.
    /// </summary>
    public void ApplySteer()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(currentNode.position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * vehicleSettings.maxSteerAngle;
        vehicleSettings.wheelColliderFrontLeft.steerAngle = Mathf.Lerp(newSteer, targetSteerAngle, Time.deltaTime * vehicleSettings.turnSpeed);
        vehicleSettings.wheelColliderFrontRight.steerAngle = Mathf.Lerp(newSteer, targetSteerAngle, Time.deltaTime * vehicleSettings.turnSpeed);
    }

    /// <summary>
    /// Sets the path for the vehicle to use.
    /// </summary>
    /// <param name="path">The path for the vehicle to use.</param>
    public void GenerateVehiclePath(RoadNode startRoadNode)
    {
        SetVehiclePath(RoadNetworkManager.GetInstance().GetValidVehiclePath(startRoadNode));
    }

    public void SetVehiclePath(VehiclePath vehiclePath)
    {
        this.path = vehiclePath;
        currentNodeNumber = 1;
        currentNode = path.nodes[currentNodeNumber];
    }

    /// <summary>
    /// Sets the vehicle to the next node in the path.
    /// </summary>
    private void NextNode()
    {
        if (currentNodeNumber == path.nodes.Count - 1)
        {
            currentNodeNumber = 0;
        }
        else
        {
            currentNodeNumber++;
        }
        currentNode = path.nodes[currentNodeNumber];
        nextNodeEvent.Invoke(currentNode);
    }

    [System.Serializable]
    public class RoadNodeUnityEvent : UnityEvent<Transform>
    {
    }

    void OnDrawGizmosSelected()
    {
        if (path == null)
        {
            return;
        }
        Gizmos.color = Color.green;
        for (int i = 0; i < path.nodes.Count; i++)
        {
            Vector3 currentNode = path.nodes[i].transform.position;
            Vector3 previousNode = Vector3.zero;
            Vector3 lastNode = Vector3.zero;
            if (i > 0)
            {
                previousNode = path.nodes[i - 1].transform.position;
            }
            else if (i == 0 && path.nodes.Count > 1)
            {
                currentNode = lastNode;
            }
            Gizmos.DrawLine(previousNode, currentNode);
            Gizmos.DrawWireSphere(currentNode, debugSphereSize);
        }
    }
}
