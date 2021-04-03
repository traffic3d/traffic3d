using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FriendGroupBoidBehaviour : BoidBehaviourStrategyBase
{
    public override float CohesionWeight { get => EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_COHESION_WEIGHT; }
    public override float AlignmentWeight { get => EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_ALIGNMENT_WEIGHT; }
    public override float SeparationWeight { get => EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_SEPARATION_WEIGHT; }
    public override float SeekingWeight { get => EvacuAgentSceneParamaters.FRIEND_GROUP_BOID_SEEKING_WEIGHT; }

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        Neighbours = new List<BoidBehaviourStrategyBase>();
        BoidManager = GetComponentInParent<BoidManager>();
    }

    public override void PerformBehaviour()
    {
        UpdateNeighbours();
        Target = GroupCollection.GroupDestination;
        Vector3 newVelocity = BoidManager.CalculateNewVelocity();
        Vector3 newPosition = transform.root.position + newVelocity;

        if (IsDestinationValid(newPosition))
        {
            NavMeshAgent.SetDestination(newPosition);
            Velocity = newVelocity;
        }
        else
        {
            // Could do sample nav mesh here?
        }
    }

    public override bool ShouldTriggerBehaviour()
    {
        return true;
    }

    public void SetPositionAndVelocity(Vector3 position, Vector3 velocity)
    {
        Position = position;
        Velocity = velocity;
    }
}
