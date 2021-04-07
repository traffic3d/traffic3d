using UnityEngine;

public class BoidObstacleAvoidanceComponent : BoidComponentBase
{
    private Vector3 facingDirCache = Vector3.zero;
    private Vector3 velocityCache = Vector3.zero;

    public override Vector3 CalculateComponentVelocity(BoidBehaviourStrategyBase followerBoidBehaviour)
    {
        Vector3 velocity = Vector3.zero;

        velocity += CalculateObstacleDirectAheadAvoidance(followerBoidBehaviour);
        velocity += CalculatePeripheralObstacleAvoidance(followerBoidBehaviour);

        return velocity;
    }

    private Vector3 CalculateObstacleDirectAheadAvoidance(BoidBehaviourStrategyBase followerBoidBehaviour)
    {
        Vector3 velocity = Vector3.zero;
        velocity += CalculateVelocity(followerBoidBehaviour.AvoidanceWeight, followerBoidBehaviour.FieldOfView.nearestObstacleAhead);
        return velocity;
    }

    private Vector3 CalculatePeripheralObstacleAvoidance(BoidBehaviourStrategyBase followerBoidBehaviour)
    {
        Vector3 velocity = Vector3.zero;

        if (followerBoidBehaviour.FieldOfView.peripheralGameObjects.Count == 0)
            return velocity;


        float weight = followerBoidBehaviour.AvoidanceWeight;

        foreach(GameObject obstacleToAvoid in followerBoidBehaviour.FieldOfView.peripheralGameObjects)
        {
            velocity += CalculateVelocity(weight, obstacleToAvoid);
        }

        return velocity;
    }

    private Vector3 CalculateVelocity(float weight, GameObject objectToAvoid)
    {
        Vector3 velocity = Vector3.zero;

        if (objectToAvoid == null)
            return velocity;

        facingDirCache = Vector3.zero;
        velocityCache = Vector3.zero;

        Vector3 obstacleCentre = objectToAvoid.transform.position;
        Vector3 facingDir = transform.position + transform.forward;
        Vector3 force = (facingDir - obstacleCentre) * weight;
        velocity = new Vector3(force.x, 0.0f, force.z);

        facingDirCache = facingDir;
        velocityCache = velocity;

        if (velocity.Equals(Vector3.zero))
            return Vector3.zero;

        return velocity.normalized;
    }

    // For debugging, the magenta line shows the force direction applied from obstace avoidance
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + facingDirCache);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + velocityCache);
        Gizmos.color = Color.white;
    }
}
