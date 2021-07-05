using UnityEngine;

public class BoidTargetSeekingComponent : BoidComponentBase
{
    Vector3 currentPathTargetCache;

    public override Vector3 CalculateComponentVelocity(BoidBehaviourStrategyBase followerBoidBehaviour)
    {
        Vector3 velocity = Vector3.zero;

        Vector3 currentPathTarget = followerBoidBehaviour.NavMeshAgent.steeringTarget;
        currentPathTargetCache = currentPathTarget;
        Vector3 directionToTarget = (currentPathTarget - followerBoidBehaviour.transform.position).normalized;
        velocity += directionToTarget * followerBoidBehaviour.TargetSeekingWeight;

        return velocity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, currentPathTargetCache);
        Gizmos.color = Color.white;
    }
}
