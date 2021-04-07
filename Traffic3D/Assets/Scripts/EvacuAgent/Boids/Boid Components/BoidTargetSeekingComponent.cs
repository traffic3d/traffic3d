using UnityEngine;

public class BoidTargetSeekingComponent : BoidComponentBase
{
    public override Vector3 CalculateComponentVelocity(BoidBehaviourStrategyBase followerBoidBehaviour)
    {
        if (followerBoidBehaviour.SeekingWeight < 0.0001f) return Vector3.zero;

        Vector3 distance = followerBoidBehaviour.Target - transform.position;
        Vector3 desiredVelocity = distance.normalized * followerBoidBehaviour.SeekingWeight;

        return desiredVelocity;
    }
}
