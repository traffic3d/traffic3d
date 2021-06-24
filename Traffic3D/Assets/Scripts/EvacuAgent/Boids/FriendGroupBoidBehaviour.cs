using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FriendGroupBoidBehaviour : BoidBehaviourStrategyBase
{
    public override float CohesionWeight => EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_COHESION_WEIGHT;
    public override float SeparationWeight => EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_SEPARATION_WEIGHT;
    public override float TargetSeekingWeight => EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_TARGET_SEEKING_WEIGHT;

    private bool isDebuggingOn;
    private Vector3 newVelocityCache;
    private Vector3 navMeshVelocityCahce;

    void Start()
    {
        base.Start();
        Neighbours = new List<BoidBehaviourStrategyBase>();
        BoidManager = EvacuAgentPedestrianBase.boidManager;
        shouldUpdateBoid = true;
        isDebuggingOn = false;
    }

    public override void PerformBehaviour()
    {
        UpdateNeighbours();
        CalculateNeighbourPoint();
        Vector3 newVelocity = BoidManager.CalculateNewVelocity();
        newVelocity = LimitVelocity(NavMeshAgent.velocity += newVelocity.normalized);
        NavMeshAgent.velocity = newVelocity;

        newVelocityCache = newVelocity;
        navMeshVelocityCahce = NavMeshAgent.velocity;
    }

    public override bool ShouldTriggerBehaviour()
    {
        return shouldUpdateBoid;
    }

    private void OnDrawGizmos()
    {
        if (!isDebuggingOn)
            return;

        Gizmos.color = Color.red; // Red is to the group destination
        Gizmos.DrawLine(transform.position, GroupCollection.GroupDestination);
        Gizmos.color = Color.blue; // Blue is to the calculate new velocity
        Gizmos.DrawRay(transform.position, newVelocityCache);
        Gizmos.color = Color.black; // Black is to the navMeshAgentVelocity
        Gizmos.DrawRay(transform.position, navMeshVelocityCahce);
        Gizmos.color = Color.white;
    }
}
