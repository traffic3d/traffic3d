using UnityEngine;

public class BoidSeparationComponent : BoidComponentBase
{
    public override Vector3 CalculateComponentVelocity(BoidBehaviourStrategyBase followerBoidBehaviour)
    {
        Vector3 velocity = Vector3.zero;

        foreach(BoidBehaviourStrategyBase neighbour in followerBoidBehaviour.Neighbours)
        {
            float distance = Vector3.Distance(transform.position, neighbour.transform.position);
            velocity += (transform.position - neighbour.transform.position) / Mathf.Pow(distance, 2);
        }

        return velocity.normalized * followerBoidBehaviour.SeparationWeight;
    }
}
