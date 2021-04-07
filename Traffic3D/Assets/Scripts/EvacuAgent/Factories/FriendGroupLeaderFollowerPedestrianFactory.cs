public class FriendGroupLeaderFollowerPedestrianFactory : AbstractEvacuAgentPedestrianFactory
{
    private void Awake()
    {
        numPedestriansToSpawn = EvacuAgentSceneParamaters.NUMBER_OF_FRIEND_GROUPS;
        numberOfFollowersLeftToSpawn = 0;
    }

    public override EvacuAgentPedestrianBase CreateEvacuAgentPedestrian(Pedestrian pedestrian)
    {
        if (numberOfFollowersLeftToSpawn == 0)
        {
            EvacuAgentPedestrianBase groupLeaderPedestrian = CreatePedestrianType(pedestrian, EvacuAgentSceneParamaters.IS_FRIEND_GROUP_FOLLOWER_HIGHTLIGHT_VISUAL_ENABLED, leaderPedestrianTypePrefab);
            currentLeaderPedestrian = groupLeaderPedestrian;
            numberOfFollowersLeftToSpawn = GetNumberOfFollowersForCurrentGroup(EvacuAgentSceneParamaters.FRIEND_GROUP_FOLLOWER_COUNT_MINIMUM, EvacuAgentSceneParamaters.FRIEND_GROUP_FOLLOWER_COUNT_MAXIMUM);
            return UpdateGroupCollection();
        }

        EvacuAgentPedestrianBase groupFollowerPedestrian = CreatePedestrianType(pedestrian, EvacuAgentSceneParamaters.IS_FRIEND_GROUP_FOLLOWER_HIGHTLIGHT_VISUAL_ENABLED, followerPedestrianTypePrefab);
        AddGroupCollectionToFollower(groupFollowerPedestrian);
        return AssignToFollowerCollection(groupFollowerPedestrian);
    }
}
