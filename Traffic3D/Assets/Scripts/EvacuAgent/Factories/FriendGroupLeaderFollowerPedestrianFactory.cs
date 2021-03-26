public class FriendGroupLeaderFollowerPedestrianFactory : LeaderFollowerPedestrianFactory
{
    private void Awake()
    {
        numPedestriansToSpawn = EvacuAgentSceneParamaters.NUMBER_OF_FRIEND_GROUPS;
        numberOfFollowersLeftToSpawn = 0;
    }

    public override EvacuAgentPedestrianBase CreateEvacuAgentPedestrian(Pedestrian pedestrian)
    {
        EvacuAgentPedestrianBase groupLeaderFollowerPedestrian = CreatePedestrianType(pedestrian, EvacuAgentSceneParamaters.IS_FRIEND_GROUP_FOLLOWER_HIGHTLIGHT_VISUAL_ENABLED, pedestrianTypePrefab);

        if (numberOfFollowersLeftToSpawn == 0)
        {
            numberOfFollowersLeftToSpawn = GetNumberOfFollowersForCurrentGroup(EvacuAgentSceneParamaters.FRIEND_GROUP_FOLLOWER_COUNT_MINIMUM, EvacuAgentSceneParamaters.FRIEND_GROUP_FOLLOWER_COUNT_MAXIMUM);
            currentLeaderPedestrian = groupLeaderFollowerPedestrian;
            return AddFollowerCollection();
        }

        return AssignToFollowerCollection(groupLeaderFollowerPedestrian);
    }
}
