using UnityEngine;

public abstract class LeaderFollowerPedestrianFactory : AbstractEvacuAgentPedestrianFactory
{
    [SerializeField]
    protected GameObject followerPedestrianTypePrefab;

    protected EvacuAgentPedestrianBase currentLeaderPedestrian;
    protected FollowerCollection currentLeaderFollowerCollection;
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

    protected EvacuAgentPedestrianBase AddFollowerCollection()
    {
        numPedestriansToSpawn--;
        currentLeaderFollowerCollection = currentLeaderPedestrian.GetComponent<FollowerCollection>();
        return currentLeaderPedestrian;
    }

    protected EvacuAgentPedestrianBase AssignToFollowerCollection(EvacuAgentPedestrianBase follower)
    {
        numberOfFollowersLeftToSpawn--;
        currentLeaderFollowerCollection.AddFollowerToCollection(follower);
        return follower;
    }
}
