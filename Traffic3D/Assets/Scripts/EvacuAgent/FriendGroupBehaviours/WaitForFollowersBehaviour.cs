using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WaitForFollowersBehaviour : BehaviourStrategy
{
    private GroupCollection followerCollection;
    private NavMeshAgent navMeshAgent;
    private GenericMoveToNextDestinationBehaviour genericMoveToNextDestinationBehaviour;
    private float acceptableProximity;
    private int waitTimerInSeconds;
    private bool isWaitTimerActive;

    private void Start()
    {
        followerCollection = GetComponentInParent<GroupCollection>();
        navMeshAgent = GetComponentInParent<NavMeshAgent>();
        genericMoveToNextDestinationBehaviour = GetComponent<GenericMoveToNextDestinationBehaviour>();
        acceptableProximity = 5f;
        waitTimerInSeconds = 20;
        isWaitTimerActive = false;
    }

    public override bool ShouldTriggerBehaviour()
    {
        Vector3 myPosition = transform.position;
        Vector3 myDestination = navMeshAgent.destination;

        if (IsLeaderAtDestination() &&
            IsWaitingForAtLeastOneFollower())
            return true;

        return false;
    }

    public override void PerformBehaviour()
    {
        StartCoroutine(StartWaitCoolDown());
    }

    public bool IsLeaderAtDestination()
    {
        if (Vector3.Distance(genericMoveToNextDestinationBehaviour.CurrentDestination, transform.position) < 1f)
        {
            return true;
        }

        return false;
    }

    public bool IsWaitingForAtLeastOneFollower()
    {
        if(followerCollection.GetFollowers().Count < followerCollection.TotalFollowerCount)
        {
            return true;
        }

        foreach(GroupFollowerPedestrian follower in followerCollection.GetFollowers())
        {
            if (Vector3.Distance(follower.transform.position, transform.position) > acceptableProximity)
            {
                NavMeshAgent navMeshAgentFollower = follower.GetComponentInParent<NavMeshAgent>();
                return true;
            }

            follower.ChangeSpeedToMatchLeader(navMeshAgent.speed);
            SetVelocityToSameValueAsLeaderWalkingVelocity();
        }

        return false;
    }

    public bool ShouldWaitForFollowersToSpawn()
    {
        if (followerCollection.GetFollowers().Count == 0)
            return true;

        return false;
    }

    IEnumerator StartWaitCoolDown()
    {
        navMeshAgent.isStopped = true;
        yield return new WaitForSeconds(waitTimerInSeconds);
        navMeshAgent.isStopped = false;
    }

    private void SetVelocityToSameValueAsLeaderWalkingVelocity()
    {
        // Set velocity in navmesh and animator
    }
}
