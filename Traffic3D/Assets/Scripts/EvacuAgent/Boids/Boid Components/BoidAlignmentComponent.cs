using UnityEngine;

public class BoidAlignmentComponent : BoidComponentBase
{
    public override Vector3 CalculateComponentVelocity(BoidBehaviourStrategyBase followerBoidBehaviour)
    {
        Vector3 velocity = Vector3.zero;

        if (followerBoidBehaviour.Neighbours.Count == 0)
            return velocity;

        foreach(BoidBehaviourStrategyBase neighbour in followerBoidBehaviour.Neighbours)
        {
            velocity += neighbour.Velocity;
        }

        // Average the velocities
        if (followerBoidBehaviour.Neighbours.Count > 1)
        {
            velocity /= followerBoidBehaviour.Neighbours.Count;
        }

        if (DoesVectorContainNaN((velocity - followerBoidBehaviour.Velocity) * followerBoidBehaviour.AlignmentWeight))
        {
            Debug.Log("ALIGNMENT HAS NaN");
        }

        // Return the offset vector multiplied by weight
        return velocity.normalized * followerBoidBehaviour.AlignmentWeight; //(velocity - followerBoidBehaviour.Velocity) * followerBoidBehaviour.AlignmentWeight;
    }
}
