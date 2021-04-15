using UnityEngine;
using UnityEngine.AI;

public class ChooseLocationOnNavmesh : MonoBehaviour
{
	private int walkableAreaMask;
	private float maxDistanceFromChosenPoint = 30f;

    private void Awake()
    {
		walkableAreaMask = 1 << NavMesh.GetAreaFromName(PedestrianManager.WALKABLE_AREA);
	}

	public Vector3 GetRandomPointOnNavMesh(Vector3 centerPointOfConsideration, float radiusToConsider)
	{
		bool validPositionFound = false;
		Vector3 result = centerPointOfConsideration;

		while(!validPositionFound)
		{
			Vector3 randomPoint = centerPointOfConsideration + Random.insideUnitSphere * radiusToConsider;

			NavMeshHit hit;
			if (NavMesh.SamplePosition(randomPoint, out hit, maxDistanceFromChosenPoint, walkableAreaMask))
			{
				result = hit.position;
				validPositionFound = true;
			}
		}

		return result;
	}
}
