using UnityEngine;

public class BoidSeparationComponent : BoidComponentBase
{
    public override Vector3 CalculateComponentVelocity(BoidBehaviourStrategyBase followerBoidBehaviour)
    {
        Vector3 velocity = Vector3.zero;

        foreach(BoidBehaviourStrategyBase neighbour in followerBoidBehaviour.Neighbours)
        {
            var distance = Vector3.Distance(transform.root.position, neighbour.transform.root.position);
            velocity += Vector3.Normalize(transform.root.position - neighbour.transform.root.position) / Mathf.Pow(distance, 2);
        }

        if (DoesVectorContainNaN(velocity * followerBoidBehaviour.SeparationWeight))
        {
            Debug.Log("SEPARATION HAS NaN");
        }

        return velocity * followerBoidBehaviour.SeparationWeight;
    }
}
