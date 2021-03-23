using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderFollowerPedestrianFactory : AbstractEvacuAgentPedestrianFactory
{
    [SerializeField]
    private GameObject followerPedestrianTypePrefab;

    private EvacuAgentPedestrianBase currentLeaderPedestrian;
    private int numberOfFollowersSpawnedInCurrentGroup;

    private void Awake()
    {
        numPedestriansToSpawn = EvacuAgentSceneParamaters.NUMBER_OF_FRIEND_GROUP_AGENTS;
        numberOfFollowersSpawnedInCurrentGroup = 0;
    }

    public override EvacuAgentPedestrianBase CreateEvacuAgentPedestrian(Pedestrian pedestrian)
    {
        numPedestriansToSpawn--;
        numberOfFollowersSpawnedInCurrentGroup++;
        return CreatePedestrianType(pedestrian, EvacuAgentSceneParamaters.IS_FRIEND_GROUP_HIGHTLIGHT_VISUAL_ENABLED, pedestrianTypePrefab);
    }

    private int GetNumberOfFollowersForCurrentGroup()
    {
        return Random.Range(EvacuAgentSceneParamaters.FRIEND_GROUP_FOLLOWER_COUNT_MINIMUM, EvacuAgentSceneParamaters.FRIEND_GROUP_FOLLOWER_COUNT_MAXIMUM);
    }
}
