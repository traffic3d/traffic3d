using UnityEngine;

public class BoidCohesionComponent : BoidComponentBase
{
    public override Vector3 CalculateComponentVelocity(BoidBehaviourStrategyBase followerBoidBehaviour)
    {
        Vector3 velocity = Vector3.zero;

        if (followerBoidBehaviour.Neighbours.Count == 0)
            return velocity;

        velocity += followerBoidBehaviour.NeighbourCenter;
        velocity -= transform.position;

        return velocity * followerBoidBehaviour.CohesionWeight;
    }
}
