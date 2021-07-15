using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidNonGroupMemberSeparationComponent : BoidComponentBase
{
    protected override bool IsDebuggingOn => false;

    public override Vector3 CalculateComponentVelocity(BoidBehaviourStrategyBase followerBoidBehaviour)
    {
        Vector3 velocity = Vector3.zero;

        if (followerBoidBehaviour.NonGroupNeighbours.Count == 0)
            return velocity;

        foreach (BoidBehaviourStrategyBase neighbour in followerBoidBehaviour.NonGroupNeighbours)
        {
            float distance = Vector3.Distance(transform.position, neighbour.transform.position);
            velocity += (neighbour.transform.position - transform.position).normalized / Mathf.Pow(distance, 2);
        }

        velocity /= followerBoidBehaviour.NonGroupNeighbours.Count; // Do i need this?
        velocity *= -1;

        float weight = 0.00005f;
        return velocity * weight;
    }

}
