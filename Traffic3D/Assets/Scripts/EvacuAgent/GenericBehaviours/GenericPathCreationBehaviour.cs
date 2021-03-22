using System.Collections.Generic;
using UnityEngine.AI;

public class GenericPathCreationBehaviour : BehaviourStrategy
{
    public List<PedestrianPoint> PathOfPedestrianPoints { get; set; }
    public NonShooterPedestrianPointPathCreator PedestrianPointPathCreator { get; private set; }
    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        navMeshAgent = GetComponentInParent<NavMeshAgent>();
    }

    public void InitialiseScript(NonShooterPedestrianPointPathCreator pedestrianPointPathCreator) // This doesn't seem like a good way of doing this
    {
        PedestrianPointPathCreator = pedestrianPointPathCreator;
    }

    public override bool ShouldTriggerBehaviour()
    {
        if(PathOfPedestrianPoints == null || PathOfPedestrianPoints.Count == 0)
            return true;

        return false;
    }

    public override void PerformBehaviour()
    {
        PathOfPedestrianPoints = PedestrianPointPathCreator.CreatePath();
        navMeshAgent.SetDestination(PathOfPedestrianPoints[0].GetPointLocation());
    }
}
