using UnityEngine;

public class UpdateFollowerDestinationsBehaviour : BehaviourStrategy
{
    private FollowerCollection followerCollection;
    private GenericMoveToNextDestinationBehaviour genericMoveToNextDestinationBehaviour;
    private Vector3 currentDestination;
    private bool shouldUpdateFollowerDestinations;

    private void Start()
    {
        followerCollection = GetComponentInParent<FollowerCollection>();
        genericMoveToNextDestinationBehaviour = GetComponent<GenericMoveToNextDestinationBehaviour>();
        currentDestination = genericMoveToNextDestinationBehaviour.CurrentDestination;
        shouldUpdateFollowerDestinations = false;
    }

    public override bool ShouldTriggerBehaviour()
    {
        if (shouldUpdateFollowerDestinations ||
            followerCollection.HasNewFollowerBeenAdded() ||
            HasDestinationChanged())
            return true;

        return false;
    }

    public override void PerformBehaviour()
    {
        foreach(EvacuAgentPedestrianBase follower in followerCollection.GetFollowers())
        {
            follower.GetComponentInChildren<FollowerDestinationUpdateBehaviour>().UpdateFollowerDestination(currentDestination);
        }

        shouldUpdateFollowerDestinations = false;
    }

    private bool HasDestinationChanged()
    {
        if (!genericMoveToNextDestinationBehaviour.CurrentDestination.Equals(currentDestination))
        {
            currentDestination = genericMoveToNextDestinationBehaviour.CurrentDestination;
            return true;
        }

        return false;
    }

    // As the collection of followers will be needed for this script and the one about waiting until all followers are there i thjink it is best to add a behaviour collection script to the
    // leader pedestrian, this could change a bool if followers are added which could be used to trigget this script?
    // How would this bool get reset?
}
