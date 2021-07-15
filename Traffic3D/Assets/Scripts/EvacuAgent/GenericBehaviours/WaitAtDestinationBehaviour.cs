using UnityEngine;

public class WaitAtDestinationBehaviour : BehaviourStrategy
{
    public float RadiusToDestination { get; set; }
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private GroupCollection groupCollection;
    private float maxRadius;
    private float minRadius;
    private float normalSpeed;

    private void Start()
    {
        evacuAgentPedestrianBase = GetComponentInParent<EvacuAgentPedestrianBase>();
        groupCollection = evacuAgentPedestrianBase.GroupCollection;
        maxRadius = 2f;
        minRadius = 0f;
        RadiusToDestination = Random.Range(minRadius, maxRadius);
        normalSpeed = GetComponentInParent<Pedestrian>().GetPedestrianNormalSpeed();
    }

    public override bool ShouldTriggerBehaviour()
    {
        float distanceToDestination = Vector3.Distance(transform.position, groupCollection.GroupDestination);
        return distanceToDestination <= RadiusToDestination;
    }

    public override void PerformBehaviour()
    {
        evacuAgentPedestrianBase.ChangeSpeedToMatchLeader(normalSpeed);
        evacuAgentPedestrianBase.IsPedestrianMovementStopped(true);
    }
}
