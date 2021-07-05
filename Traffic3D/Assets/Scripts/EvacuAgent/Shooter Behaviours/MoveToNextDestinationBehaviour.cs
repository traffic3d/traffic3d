using UnityEngine;
using UnityEngine.AI;

public class MoveToNextDestinationBehaviour : BehaviourStrategy
{
    private CreateWeightedPathOfPedestrianPointsBehaviour createWeightedPathOfPedestrianPointsBehaviour;
    private NavMeshAgent navMeshAgent;
    private readonly float proximityToDestination = 1f;

    private void Start()
    {
        createWeightedPathOfPedestrianPointsBehaviour = GetComponent<CreateWeightedPathOfPedestrianPointsBehaviour>();
        navMeshAgent = GetComponentInParent<NavMeshAgent>();
    }

    public override bool ShouldTriggerBehaviour()
    {
        if (Vector3.Distance(transform.position, navMeshAgent.destination) < proximityToDestination)
        {
            return true;
        }

        return false;
    }

    public override void PerformBehaviour()
    {
        createWeightedPathOfPedestrianPointsBehaviour.CurrentPathIndex += 1;
        Vector3 currentPathNode = createWeightedPathOfPedestrianPointsBehaviour.CurrentPath[createWeightedPathOfPedestrianPointsBehaviour.CurrentPathIndex];
        navMeshAgent.SetDestination(currentPathNode);
        return;
    }
}
