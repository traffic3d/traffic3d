using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreateWeightedPathOfPedestrianPointsBehaviour : BehaviourStrategy
{
    public int SizeOfPath { get; set; } = 4;
    public int CurrentPathIndex { get; set; } = 0;
    public List<Vector3> CurrentPath { get; set; }

    private ShooterPedestrianPointPathCreator pedestrianPathCreator;
    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        pedestrianPathCreator = gameObject.AddComponent<ShooterPedestrianPointPathCreator>();
        CurrentPath = new List<Vector3>();
        navMeshAgent = GetComponentInParent<NavMeshAgent>();
    }

    public override bool ShouldTriggerBehaviour()
    {
        return CurrentPath.Count == 0 || CurrentPathIndex == SizeOfPath - 1;
    }

    public override void PerformBehaviour()
    {
        CurrentPathIndex = 0;
        CurrentPath = pedestrianPathCreator.CreatePath();
        navMeshAgent.SetDestination(CurrentPath[CurrentPathIndex]);
    }
}
