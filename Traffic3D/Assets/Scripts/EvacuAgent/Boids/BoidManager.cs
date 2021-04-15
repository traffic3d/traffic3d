using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    public bool isBoidMovementStopped { get; set; }
    private List<BoidComponentBase> boidComponents;
    private BoidBehaviourStrategyBase followerBoidBehaviour;

    void Start()
    {
        boidComponents = new List<BoidComponentBase>();
        isBoidMovementStopped = false;

        foreach (BoidComponentBase boidComponentBase in GetComponentsInChildren<BoidComponentBase>())
        {
            boidComponents.Add(boidComponentBase);
        }

        followerBoidBehaviour = GetComponentInChildren<BoidBehaviourStrategyBase>();
    }

    public Vector3 CalculateNewVelocity()
    {
        Vector3 velocity = Vector3.zero;

        if (isBoidMovementStopped)
            return velocity;

        foreach(BoidComponentBase boidComponentBase in boidComponents)
        {
            velocity += boidComponentBase.CalculateComponentVelocity(followerBoidBehaviour);
        }

        return velocity;
    }
}
