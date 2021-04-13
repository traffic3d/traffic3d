using UnityEngine;

public class WaitForFollowersBehaviour : BehaviourStrategy
{
    private GroupCollection groupCollection;
    private float acceptableProximity;
    private float groupWalkingSpeedLowerBound;
    private float groupWalkingSpeedUpperBound;
    private float groupWalkingSpeed;

    private void Start()
    {
        groupCollection = GetComponentInParent<GroupCollection>();
        acceptableProximity = 5f;
        groupWalkingSpeedLowerBound = 1.5f;
        groupWalkingSpeedUpperBound = 3f;
        GenerateGroupWalkingSpeed();
    }

    public override bool ShouldTriggerBehaviour()
    {
        if (IsLeaderAtDestination() &&
            AreAllFolowersAtDestination())
            return true;

        return false;
    }

    public override void PerformBehaviour()
    {
        groupCollection.UpdateGroupDestination();

        foreach (EvacuAgentPedestrianBase evacuAgentPedestrianBase in groupCollection.GetGroupMembers())
        {
            BoidBehaviourStrategyBase boidBehaviourStrategyBase = evacuAgentPedestrianBase.GetComponentInChildren<BoidBehaviourStrategyBase>();
            boidBehaviourStrategyBase.ShouldBoidLogicBeActive(true);
            boidBehaviourStrategyBase.EvacuAgentPedestrianBase.ChangeSpeedToMatchLeader(groupWalkingSpeed);
            evacuAgentPedestrianBase.navMeshAgent.isStopped = false;
            evacuAgentPedestrianBase.navMeshAgent.SetDestination(groupCollection.GroupDestination);
        }
    }

    public bool IsLeaderAtDestination()
    {
        if (Vector3.Distance(groupCollection.GroupDestination, transform.position) < 3f)
        {
            return true;
        }

        return false;
    }

    public bool AreAllFolowersAtDestination()
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
        if (groupCollection.GetGroupMembers().Count == 0)
            return true;

        return false;
    }

    public void GenerateGroupWalkingSpeed()
    {
        groupWalkingSpeed = Random.Range(groupWalkingSpeedLowerBound, groupWalkingSpeedUpperBound);
    }
}
