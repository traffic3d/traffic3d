using UnityEngine;

public class BoidCohesionComponent : BoidComponentBase
{
    public override Vector3 CalculateComponentVelocity(BoidBehaviourStrategyBase followerBoidBehaviour)
    {
        Vector3 velocity = Vector3.zero;

        if (followerBoidBehaviour.Neighbours.Count == 0)
            return velocity;

        // Calculate center point of all visible neighbours
        velocity /= followerBoidBehaviour.Neighbours.Count;

        if (DoesVectorContainNaN(velocity * followerBoidBehaviour.CohesionWeight))
        {
            Debug.Log("COHESION HAS NaN");
        }

        return velocity * followerBoidBehaviour.CohesionWeight;
    }
}
