using UnityEngine;

public class WaitAtDestinationBehaviour : BehaviourStrategy
{
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private GroupCollection groupCollection;
    private BoidBehaviourStrategyBase boidBehaviourStrategyBase;
    private float radiusToDestination;
    private float maxRadius;
    private float minRadius;
    private float normalSpeed;

    private void Start()
    {
        evacuAgentPedestrianBase = GetComponentInParent<EvacuAgentPedestrianBase>();
        groupCollection = evacuAgentPedestrianBase.GroupCollection;
        boidBehaviourStrategyBase = GetComponent<BoidBehaviourStrategyBase>();
        maxRadius = 5f;
        minRadius = 1f;
        radiusToDestination = Random.Range(minRadius, maxRadius);
        normalSpeed = GetComponentInParent<Pedestrian>().GetPedestrianNormalSpeed();
    }

    public override bool ShouldTriggerBehaviour()
    {
        float distanceToDestination = Vector3.Distance(transform.position, groupCollection.GroupDestination);

        if (distanceToDestination <= radiusToDestination)
            return true;

        return false;
    }

    public override void PerformBehaviour()
    {
        evacuAgentPedestrianBase.ChangeSpeedToMatchLeader(normalSpeed);
        boidBehaviourStrategyBase.ShouldBoidLogicBeActive(false);
    }
}
