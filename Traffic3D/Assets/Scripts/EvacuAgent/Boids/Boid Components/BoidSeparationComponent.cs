using UnityEngine;

public class BoidSeparationComponent : BoidComponentBase
{
    protected override bool IsDebuggingOn => false;

    public override Vector3 CalculateComponentVelocity(BoidBehaviourStrategyBase followerBoidBehaviour)
    {
        Vector3 velocity = Vector3.zero;

        if (followerBoidBehaviour.Neighbours.Count == 0)
            return velocity;

        foreach (BoidBehaviourStrategyBase neighbour in followerBoidBehaviour.Neighbours)
        {
            float distance = Vector3.Distance(followerBoidBehaviour.transform.position, neighbour.transform.position);
            velocity += (neighbour.transform.position - followerBoidBehaviour.transform.position).normalized / Mathf.Pow(distance, 2);
        }

        velocity /= followerBoidBehaviour.Neighbours.Count;
        velocity *= -1;

        return velocity * followerBoidBehaviour.SeparationWeight;
    }
}
