using UnityEngine;

public class WaitForFollowersBehaviour : BehaviourStrategy
{
    private GroupCollection groupCollection;
    private float acceptableProximity;
    private float leaderAcceptableProximity;
    private float groupWalkingSpeedLowerBound;
    private float groupWalkingSpeedUpperBound;
    private float groupWalkingSpeed;

    private void Start()
    {
        groupCollection = GetComponentInParent<GroupCollection>();
        acceptableProximity = 3f;
        leaderAcceptableProximity = 3f;
        groupWalkingSpeedLowerBound = 1.2f;
        groupWalkingSpeedUpperBound = 2.4f;
        GenerateGroupWalkingSpeed();

        if (groupCollection.TotalGroupCount == 0)
            leaderAcceptableProximity = 1f;
    }

    public override bool ShouldTriggerBehaviour()
    {
        return IsLeaderAtDestination() && AreAllFollowersAtDestination();
    }

    public override void PerformBehaviour()
    {
        groupCollection.UpdateGroupDestination();

        foreach (EvacuAgentPedestrianBase evacuAgentPedestrianBase in groupCollection.GetGroupMembers())
        {
            BoidBehaviourStrategyBase boidBehaviourStrategyBase = evacuAgentPedestrianBase.GetComponentInChildren<BoidBehaviourStrategyBase>();
            boidBehaviourStrategyBase.EvacuAgentPedestrianBase.ChangeSpeedToMatchLeader(groupWalkingSpeed);
            evacuAgentPedestrianBase.IsPedestrianMovementStopped(false);
            evacuAgentPedestrianBase.navMeshAgent.SetDestination(groupCollection.GroupDestination);
        }
    }

    public bool IsLeaderAtDestination()
    {
        float distance = Vector3.Distance(groupCollection.GroupDestination, transform.position);
        return distance <= leaderAcceptableProximity;
    }

    public bool AreAllFollowersAtDestination()
    {
        if(groupCollection.GetGroupMembers().Count < groupCollection.TotalGroupCount)
        {
            return false;
        }

        foreach(EvacuAgentPedestrianBase groupMember in groupCollection.GetGroupMembers())
        {
            if (groupMember == groupCollection.GroupLeaderPedestrian)
                continue;

            float memberToDestinationDistance = Vector3.Distance(groupMember.transform.position, groupCollection.GroupDestination);
            if (memberToDestinationDistance > acceptableProximity)
            {
                return false;
            }
        }

        return true;
    }

    public bool ShouldWaitForFollowersToSpawn()
    {
        return groupCollection.GetGroupMembers().Count == 0;
    }

    public void GenerateGroupWalkingSpeed()
    {
        groupWalkingSpeed = Random.Range(groupWalkingSpeedLowerBound, groupWalkingSpeedUpperBound);
    }
}
