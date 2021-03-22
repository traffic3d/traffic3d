using System.Collections.Generic;
using UnityEngine.AI;

public class CreateWeightedPathOfPedestrianPointsBehaviour : BehaviourStrategy
{
    public int SizeOfPath { get; set; } = 4;
    public int CurrentPathIndex { get; set; } = 0;
    public List<PedestrianPoint> CurrentPath { get; set; }

    private ShooterPedestrianPointPathCreator pedestrianPathCreator;
    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        pedestrianPathCreator = gameObject.AddComponent<ShooterPedestrianPointPathCreator>();
        CurrentPath = new List<PedestrianPoint>();
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
        CurrentPath = pedestrianPathCreator.CreatePath();
        navMeshAgent.SetDestination(CurrentPath[CurrentPathIndex].transform.position);
    }
}
