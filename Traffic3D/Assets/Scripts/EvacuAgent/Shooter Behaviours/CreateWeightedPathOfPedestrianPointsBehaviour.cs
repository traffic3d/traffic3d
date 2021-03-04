using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreateWeightedPathOfPedestrianPointsBehaviour : BehaviourStrategy
{
    public int SizeOfPath { get; set; } = 4;
    public int CurrentPathIndex { get; set; } = 0;
    public PedestrianPoint[] CurrentPath { get; set; }

    private PedestrianPathCreator pedestrianPathCreator;
    private NavMeshAgent navMeshAgent;
    private float radiusToConsiderPedestrianPoints = 100f;
    private float footfallWeighting = 0.7f;
    private float distanceWeighting = 0.3f;

    private void Start()
    {
        pedestrianPathCreator = gameObject.AddComponent<PedestrianPathCreator>();
        CurrentPath = new PedestrianPoint[SizeOfPath];
        navMeshAgent = GetComponentInParent<NavMeshAgent>();
    }

    public override bool ShouldTriggerBehaviour()
    {
        if(CurrentPath[CurrentPathIndex] == null || CurrentPathIndex == SizeOfPath - 1)
        {
            return true;
        }

        return false;
    }

    public override void PerformBehaviour()
    {
        CurrentPathIndex = 0;
        CurrentPath = pedestrianPathCreator.CalculateRankedShooterAgentPath(radiusToConsiderPedestrianPoints, transform, SizeOfPath, footfallWeighting, distanceWeighting);
        navMeshAgent.SetDestination(CurrentPath[CurrentPathIndex].transform.position);
    }
}
