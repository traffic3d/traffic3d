using UnityEngine;
using UnityEngine.AI;

public class FollowerDestinationUpdateBehaviour : BehaviourStrategy
{
    private NavMeshAgent navMeshAgent;
    private bool shouldUpdateDestination;
    private Vector3 newDestination;

    private void Start()
    {
        navMeshAgent = gameObject.GetComponentInParent<NavMeshAgent>();
    }

    public override bool ShouldTriggerBehaviour()
    {
        if (shouldUpdateDestination)
            return true;

        return false;
    }

    public override void PerformBehaviour()
    {
        navMeshAgent.SetDestination(newDestination);
        shouldUpdateDestination = false;
    }

    public void UpdateFollowerDestination(Vector3 newDestination)
    {
        shouldUpdateDestination = true;
        this.newDestination = newDestination; // Could reuse chooseRandomPointOnNavmesh with small radius here so pedestrians do not all stand on the same spot
    }
}
