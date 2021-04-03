using UnityEngine;
using UnityEngine.AI;

public class UpdateFollowerDestinationsBehaviour : BehaviourStrategy
{
    private GroupCollection followerCollection;
    private GenericMoveToNextDestinationBehaviour genericMoveToNextDestinationBehaviour;
    private Vector3 currentDestination;

    private void Start()
    {
        followerCollection = GetComponentInParent<GroupCollection>();
        genericMoveToNextDestinationBehaviour = GetComponent<GenericMoveToNextDestinationBehaviour>();
        currentDestination = genericMoveToNextDestinationBehaviour.CurrentDestination;
    }

    public override bool ShouldTriggerBehaviour()
    {
        CreateNumberOfFollowerPointsAroundLeader();

        if (followerCollection.HasNewFollowerBeenAdded() ||
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

        followerCollection.ResetHasNewFollowerBeenAdded();
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


    private void CreateNumberOfFollowerPointsAroundLeader()
    {
        ChooseLocationOnNavmesh chooseLocationOnNavmesh = GetComponentInParent<ChooseLocationOnNavmesh>();

        foreach (GroupFollowerPedestrian groupFollowerPedestrian in followerCollection.GetFollowers())
        {
            Vector3 position = chooseLocationOnNavmesh.GetRandomPointOnNavMesh(transform.position, 5f);
            GameObject gameObject = Instantiate(new GameObject(), position, Quaternion.identity);
            gameObject.transform.SetParent(transform);
        }
    }
}
