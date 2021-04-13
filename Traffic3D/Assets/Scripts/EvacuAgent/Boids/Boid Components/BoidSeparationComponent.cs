﻿using UnityEngine;

public class BoidSeparationComponent : BoidComponentBase
{
    public override Vector3 CalculateComponentVelocity(BoidBehaviourStrategyBase followerBoidBehaviour)
    {
        Vector3 velocity = Vector3.zero;

        if (followerBoidBehaviour.Neighbours.Count == 0)
            return velocity;

        foreach (BoidBehaviourStrategyBase neighbour in followerBoidBehaviour.Neighbours)
        {
            float distance = Vector3.Distance(transform.position, neighbour.transform.position);
            velocity += (neighbour.transform.position - transform.position).normalized / Mathf.Pow(distance, 2);
        }

        velocity /= followerBoidBehaviour.Neighbours.Count; // Do i need this?
        velocity *= -1;

        return velocity * followerBoidBehaviour.SeparationWeight;
    }
}
