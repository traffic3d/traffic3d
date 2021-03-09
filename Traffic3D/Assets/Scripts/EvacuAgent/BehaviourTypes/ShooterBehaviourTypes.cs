/*Note that order matters. The closer to 0 in the enum the higher preference it has in the subsumption hierarchy
* Meaning it will be iterated over first and potentially chosen over other behaviours
* 1f here is the chance the behaviour has of being added to a BehaviourCollection (this is not used yet, 09/03/21)
*/
public enum ShooterBehaviourTypes
{
    [BehaviourType("FollowClosestTargetBehaviour", 1f)]
    FollowClosestTarget,

    [BehaviourType("CreateWeightedPathOfPedestrianPointsBehaviour", 1f)]
    CreateWeightedPathOfPedestrianPoints,

    [BehaviourType("MoveToNextDestinationBehaviour", 1f)]
    MoveToNextDestination
}
