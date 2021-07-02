public class WorkerLeaderFollowerPedestrianFactory : AbstractEvacuAgentPedestrianFactory
{
    private void Awake()
    {
        numPedestriansToSpawn = EvacuAgentSceneParamaters.NUMBER_OF_WORKER_AGENTS;
        numberOfFollowersLeftToSpawn = 0;
    }

    public override EvacuAgentPedestrianBase CreateEvacuAgentPedestrian(Pedestrian pedestrian)
    {
        if (numberOfFollowersLeftToSpawn == 0)
        {
            EvacuAgentPedestrianBase groupLeaderPedestrian = CreatePedestrianType(pedestrian, EvacuAgentSceneParamaters.IS_WORKER_HIGHTLIGHT_VISUAL_ENABLED, leaderPedestrianTypePrefab);
            currentLeaderPedestrian = groupLeaderPedestrian;
            numberOfFollowersLeftToSpawn = GetNumberOfFollowersForCurrentGroup(EvacuAgentSceneParamaters.WORKER_GROUP_FOLLOWER_COUNT_MINIMUM, EvacuAgentSceneParamaters.WORKER_GROUP_FOLLOWER_COUNT_MAXIMUM);
            return UpdateGroupCollection();
        }

        EvacuAgentPedestrianBase groupFollowerPedestrian = CreatePedestrianType(pedestrian, EvacuAgentSceneParamaters.IS_WORKER_HIGHTLIGHT_VISUAL_ENABLED, followerPedestrianTypePrefab);
        AddGroupCollectionToFollower(groupFollowerPedestrian);
        return AssignToFollowerCollection(groupFollowerPedestrian);
    }
}
