// Note that order matters. The closer to 0 in the enum the higher preference it has in the subsumption hierarchy
// Meaning it will be iterated over first and potentially chosen over other behaviours
public enum ShooterBehaviourTypes
{
    [BehaviourType("FollowClosestTargetBehaviour", 1f)]
    FollowClosestTarget,

    [BehaviourType("CreateWeightedPathOfPedestrianPointsBehaviour", 1f)]
    CreateWeightedPathOfPedestrianPoints,

    [BehaviourType("MoveToNextDestinationBehaviour", 1f)]
    MoveToNextDestination
}
