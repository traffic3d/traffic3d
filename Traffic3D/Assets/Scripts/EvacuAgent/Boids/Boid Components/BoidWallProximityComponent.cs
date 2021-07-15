using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidWallProximityComponent : BoidComponentBase
{
    protected override bool IsDebuggingOn => true;

    private List<RaycastHit> raycastHitsDebug;

    private void Start()
    {
        raycastHitsDebug = new List<RaycastHit>();
    }

    public override Vector3 CalculateComponentVelocity(BoidBehaviourStrategyBase followerBoidBehaviour)
    {
        Vector3 velocity = Vector3.zero;
        raycastHitsDebug.Clear();

        if (followerBoidBehaviour.NonGroupNeighbours.Count == 0)
            return velocity;

        List<RaycastHit> raycastHits = followerBoidBehaviour.FieldOfView.GetPeripheralObjects();

        if (raycastHits.Count == 0)
            return velocity;

        raycastHitsDebug.AddRange(raycastHits);

        foreach(RaycastHit raycastHit in raycastHits)
        {
            foreach (BoidBehaviourStrategyBase neighbour in followerBoidBehaviour.NonGroupNeighbours)
            {
                // If this pedestrian is further from the peripheral obstacle than the non group memeber then apply small force away from the obstacle
                if (!IsThisPositionCloserToObject(transform.position, neighbour.transform.position, raycastHit.point))
                {
                    velocity += GetVelocityComponent(raycastHit.point);
                }
            }
        }

        return velocity;
    }

    private bool IsThisPositionCloserToObject(Vector3 thisPedestrianPosition, Vector3 otherPedestrianPosition, Vector3 obstaclePoint)
    {
        float distanceFromThisPedToPoint = Vector3.Distance(thisPedestrianPosition, obstaclePoint);
        float distanceFromOtherPedToPoint = Vector3.Distance(otherPedestrianPosition, obstaclePoint);

        if (distanceFromThisPedToPoint < distanceFromOtherPedToPoint)
            return true;

        return false;
    }

    private Vector3 GetVelocityComponent(Vector3 rayCastHitPoint)
    {
        Vector3 velocity = Vector3.zero;

        float distance = Vector3.Distance(transform.position, rayCastHitPoint);
        velocity += (transform.position - rayCastHitPoint).normalized / Mathf.Pow(distance, 2);

        float weight = 0.0001f;
        velocity *= weight;

        return velocity.normalized;
    }

    private void OnDrawGizmos()
    {
        if (!IsDebuggingOn)
            return;

        Gizmos.color = Color.green;
        raycastHitsDebug.ForEach(x => Gizmos.DrawLine(transform.position, x.point));
        raycastHitsDebug.ForEach(x => Gizmos.DrawSphere(x.point, 0.2f));
        Gizmos.color = Color.white;
    }
}
