using UnityEngine;

public class BoidArrivalComponent : BoidComponentBase
{
    private float distanceToStartSlowing = 20f;
    private float maxArrivalSpeed = 0.001f;

    public override Vector3 CalculateComponentVelocity(BoidBehaviourStrategyBase followerBoidBehaviour)
    {
        var desiredVelocity = Vector3.zero;
        if (distanceToStartSlowing < 1f) return desiredVelocity; // This is never true

        var targetOffset = followerBoidBehaviour.Target - followerBoidBehaviour.Position;
        var distance = Vector3.Distance(followerBoidBehaviour.Target, followerBoidBehaviour.Position);
        var rampedSpeed = maxArrivalSpeed * (distance / distanceToStartSlowing);
        var clippedSpeed = Mathf.Min(rampedSpeed, maxArrivalSpeed);
        if (distance > 0)
        {
            desiredVelocity = (clippedSpeed / distance) * targetOffset;
        }
        return desiredVelocity.normalized;// - followerBoidBehaviour.Velocity;
    }
}
