using UnityEngine;

public class BoidCohesionComponent : BoidComponentBase
{
    protected override bool IsDebuggingOn => false;

    public override Vector3 CalculateComponentVelocity(BoidBehaviourStrategyBase followerBoidBehaviour)
    {
        Vector3 velocity = Vector3.zero;

        if (followerBoidBehaviour.Neighbours.Count == 0)
            return velocity;

        velocity += followerBoidBehaviour.NeighbourCenter;
        velocity -= followerBoidBehaviour.transform.position;

        return velocity * followerBoidBehaviour.CohesionWeight;
    }
}
