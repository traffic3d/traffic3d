using UnityEngine;

public class WaitForFollowersBehaviour : BehaviourStrategy
{
    private GroupCollection groupCollection;
    private float acceptableProximity;

    private void Start()
    {
        groupCollection = GetComponentInParent<GroupCollection>();
        acceptableProximity = 3f;
    }

    public override bool ShouldTriggerBehaviour()
    {
        if (IsLeaderAtDestination() &&
            IsWaitingForAtLeastOneFollower())
            return true;

        return false;
    }

    public override void PerformBehaviour()
    {
        foreach(EvacuAgentPedestrianBase evacuAgentPedestrianBase in groupCollection.GetGroupMembers())
        {
            BoidBehaviourStrategyBase boidBehaviourStrategyBase = evacuAgentPedestrianBase.GetComponentInChildren<BoidBehaviourStrategyBase>();
            boidBehaviourStrategyBase.ShouldBoidLogicBeActive(true);
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

    public bool IsWaitingForAtLeastOneFollower()
    {
        if(groupCollection.GetGroupMembers().Count < groupCollection.TotalGroupCount)
        {
            return true;
        }

        foreach(EvacuAgentPedestrianBase groupMember in groupCollection.GetGroupMembers())
        {
            if (groupMember == groupCollection.GroupLeaderPedestrian)
                continue;

            float distanceToMember = Vector3.Distance(groupMember.transform.position, transform.position);
            if (distanceToMember > acceptableProximity)
            {
                return true;
            }
        }

        return false;
    }

    public bool ShouldWaitForFollowersToSpawn()
    {
        if (groupCollection.GetGroupMembers().Count == 0)
            return true;

        return false;
    }
}
