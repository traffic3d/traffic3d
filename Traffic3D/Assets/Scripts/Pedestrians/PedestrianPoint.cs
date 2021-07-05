using UnityEngine;
using UnityEngine.AI;

public class PedestrianPoint : MonoBehaviour
{
    public float footfall;
    private Vector3 location = Vector3.negativeInfinity;
    public PedestrianPointType pedestrianPointType { get; protected set; }

    public Vector3 GetPointLocation()
    {
        if (!location.Equals(Vector3.negativeInfinity))
        {
            return location;
        }
        else
        {
            NavMeshHit hit;
            Vector3 finalPosition = Vector3.zero;
            if (NavMesh.SamplePosition(transform.position, out hit, float.MaxValue, 1 << NavMesh.GetAreaFromName(PedestrianManager.WALKABLE_AREA)))
            {
                location = hit.position;
                return location;
            }
            else
            {
                throw new System.Exception("No location for Pedestrian Point.");
            }
        }
    }
}
