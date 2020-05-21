using UnityEngine;
using UnityEngine.AI;

public class PedestrianPoint : MonoBehaviour
{
    public Vector3 GetPointLocation()
    {
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(transform.position, out hit, float.MaxValue, 1 << NavMesh.GetAreaFromName(PedestrianManager.WALKABLE_AREA)))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }
}
