using UnityEngine;
using UnityEngine.AI;

public class FollowerDestinationUpdateBehaviour : BehaviourStrategy
{
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private GroupCollection groupCollection;
    private NavMeshAgent navMeshAgent;
    private Vector3 currentDestination;

    private void Start()
    {
        evacuAgentPedestrianBase = GetComponentInParent<EvacuAgentPedestrianBase>();
        groupCollection = evacuAgentPedestrianBase.GroupCollection;
        navMeshAgent = evacuAgentPedestrianBase.navMeshAgent;
        currentDestination = groupCollection.GroupDestination;
        navMeshAgent.SetDestination(currentDestination);
    }

    public override bool ShouldTriggerBehaviour()
    {
        return !groupCollection.GroupDestination.Equals(currentDestination);
    }

    public override void PerformBehaviour()
    {
        currentDestination = groupCollection.GroupDestination;
        navMeshAgent.SetDestination(currentDestination);
    }
}
