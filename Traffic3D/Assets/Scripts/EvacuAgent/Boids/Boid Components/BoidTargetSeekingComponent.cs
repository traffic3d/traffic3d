using UnityEngine;

public class BoidTargetSeekingComponent : BoidComponentBase
{
    public override Vector3 CalculateComponentVelocity(BoidBehaviourStrategyBase followerBoidBehaviour)
    {
        if (followerBoidBehaviour.SeekingWeight < 0.0001f) return Vector3.zero;

        var desiredVelocity = (followerBoidBehaviour.Target - followerBoidBehaviour.transform.position) * followerBoidBehaviour.SeekingWeight;

        if(DoesVectorContainNaN(desiredVelocity - followerBoidBehaviour.Velocity))
        {
            Debug.Log("SEEKING HAS NaN");
        }

        return desiredVelocity - followerBoidBehaviour.Velocity;
    }
}
