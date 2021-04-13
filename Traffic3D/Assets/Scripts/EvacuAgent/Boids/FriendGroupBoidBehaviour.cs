using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FriendGroupBoidBehaviour : BoidBehaviourStrategyBase
{
    public override float CohesionWeight { get => EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_COHESION_WEIGHT; }
    public override float SeparationWeight { get => EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_SEPARATION_WEIGHT; }

    void Start()
    {
        base.Start();
        Neighbours = new List<BoidBehaviourStrategyBase>();
        BoidManager = GetComponentInParent<BoidManager>();
        shouldUpdateBoid = true;
    }

    public override void PerformBehaviour()
    {
        UpdateNeighbours();
        CalculateNeighbourPoint();
        Vector3 newVelocity = BoidManager.CalculateNewVelocity();
        newVelocity = LimitVelocity(NavMeshAgent.velocity += newVelocity.normalized);
        NavMeshAgent.velocity = newVelocity;
    }

    public override bool ShouldTriggerBehaviour()
    {
        return shouldUpdateBoid;
    }
}
