using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FriendGroupBoidBehaviour : BoidBehaviourStrategyBase
{
    public override float CohesionWeight { get => EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_COHESION_WEIGHT; }
    public override float SeparationWeight { get => EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_SEPARATION_WEIGHT; }

    //Debugging
    private Vector3 newVelocityCache;
    //Debugging
    private Vector3 navMeshVelocityCahce;

    void Start()
    {
        base.Start();
        Neighbours = new List<BoidBehaviourStrategyBase>();
        BoidManager = EvacuAgentPedestrianBase.boidManager;
        shouldUpdateBoid = true;
    }

    public override void PerformBehaviour()
    {
        UpdateNeighbours();
        CalculateNeighbourPoint();
        Vector3 newVelocity = BoidManager.CalculateNewVelocity();
        newVelocity = LimitVelocity(NavMeshAgent.velocity += newVelocity.normalized);
        newVelocityCache = newVelocity;
        NavMeshAgent.velocity = newVelocity;
        navMeshVelocityCahce = NavMeshAgent.velocity;
    }

    public override bool ShouldTriggerBehaviour()
    {
        return shouldUpdateBoid;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // Red is to the group destination
        Gizmos.DrawLine(transform.position, GroupCollection.GroupDestination);
        Gizmos.color = Color.blue; // Blue is to the calculate new velocity
        Gizmos.DrawRay(transform.position, newVelocityCache);
        Gizmos.color = Color.black; // Black is to the navMeshAgentVelocity
        Gizmos.DrawRay(transform.position, navMeshVelocityCahce);
        Gizmos.color = Color.white;
    }
}
