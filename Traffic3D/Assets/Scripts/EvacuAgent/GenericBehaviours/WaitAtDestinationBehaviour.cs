using UnityEngine;
using UnityEngine.AI;

public class WaitAtDestinationBehaviour : BehaviourStrategy
{
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private GroupCollection groupCollection;
    private BoidBehaviourStrategyBase boidBehaviourStrategyBase;
    private NavMeshAgent navMeshAgent;
    private float radiusToDestination;
    private float maxRadius;
    private float minRadius;
    private float normalSpeed;

    private void Start()
    {
        evacuAgentPedestrianBase = GetComponentInParent<EvacuAgentPedestrianBase>();
        groupCollection = evacuAgentPedestrianBase.GroupCollection;
        boidBehaviourStrategyBase = GetComponent<BoidBehaviourStrategyBase>();
        maxRadius = 3f;
        minRadius = 0f;
        radiusToDestination = Random.Range(minRadius, maxRadius);
        normalSpeed = GetComponentInParent<Pedestrian>().GetPedestrianNormalSpeed();
        navMeshAgent = evacuAgentPedestrianBase.navMeshAgent;
    }

    public override bool ShouldTriggerBehaviour()
    {
        float distanceToDestination = Vector3.Distance(transform.position, groupCollection.GroupDestination);
        return distanceToDestination <= radiusToDestination;
    }

    public override void PerformBehaviour()
    {
        evacuAgentPedestrianBase.ChangeSpeedToMatchLeader(normalSpeed);
        evacuAgentPedestrianBase.IsPedestrianMovementStopped(false);
    }
}
