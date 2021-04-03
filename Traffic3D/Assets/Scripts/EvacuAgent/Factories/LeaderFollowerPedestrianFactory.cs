using UnityEngine;
using UnityEngine.AI;

public abstract class LeaderFollowerPedestrianFactory : AbstractEvacuAgentPedestrianFactory
{
    [SerializeField]
    protected GameObject followerPedestrianTypePrefab;

    protected EvacuAgentPedestrianBase currentLeaderPedestrian;
    protected GroupCollection currentLeaderFollowerCollection;
    protected int numberOfFollowersLeftToSpawn;

    public override bool HasSpawnedMaxPedestrians()
    {
        if (numPedestriansToSpawn == 0 && numberOfFollowersLeftToSpawn == 0)
            return true;

        return false;
    }

    public int GetNumberOfFollowers()
    {
        return numberOfFollowersLeftToSpawn;
    }

    protected int GetNumberOfFollowersForCurrentGroup(int lowerBound, int upperBound)
    {
        return Random.Range(lowerBound, upperBound);
    }

    protected EvacuAgentPedestrianBase UpdateGroupCollection()
    {
        numPedestriansToSpawn--;
        currentLeaderFollowerCollection = currentLeaderPedestrian.GetComponent<GroupCollection>();
        currentLeaderFollowerCollection.TotalFollowerCount = numberOfFollowersLeftToSpawn;
        currentLeaderFollowerCollection.GroupLeaderPedestrian = currentLeaderPedestrian;
        currentLeaderFollowerCollection.AddFollowerToCollection((GroupPedestrian)currentLeaderPedestrian);
        return currentLeaderPedestrian;
    }

    protected void AddGroupCollectionToFollower(GroupPedestrian groupPedestrian)
    {
        groupPedestrian.AddGroupCollection(currentLeaderFollowerCollection);
    }

    protected EvacuAgentPedestrianBase AssignToFollowerCollection(EvacuAgentPedestrianBase follower)
    {
        numberOfFollowersLeftToSpawn--;
        currentLeaderFollowerCollection.AddFollowerToCollection((GroupFollowerPedestrian)follower);
        follower.GetComponentInParent<NavMeshAgent>().stoppingDistance = 1f;
        return follower;
    }
}
