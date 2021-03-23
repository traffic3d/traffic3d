using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GenericPathCreationBehaviour : BehaviourStrategy
{
    public List<Vector3> Path { get; set; }
    public NonShooterPedestrianPointPathCreator PedestrianPointPathCreator { get; private set; }
    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        navMeshAgent = GetComponentInParent<NavMeshAgent>();
        PedestrianPointPathCreator = GetComponentInParent<NonShooterPedestrianPointPathCreator>();
    }

    public override bool ShouldTriggerBehaviour()
    {
        if(Path == null || Path.Count == 0)
            return true;

        return false;
    }

    public override void PerformBehaviour()
    {
        Path = PedestrianPointPathCreator.CreatePath();
        navMeshAgent.SetDestination(Path[0]);
    }
}
