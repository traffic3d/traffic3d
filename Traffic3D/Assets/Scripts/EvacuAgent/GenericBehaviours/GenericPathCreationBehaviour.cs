using System.Collections.Generic;
using UnityEngine.AI;

public class GenericPathCreationBehaviour : BehaviourStrategy
{
    public List<PedestrianPoint> pathOfPedestrianPoints { get; private set; }
    private PedestrianPointPathCreator pedestrianPointPathCreator;
    private PedestrianType pedestrianType;
    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        pedestrianPointPathCreator = gameObject.AddComponent<PedestrianPointPathCreator>();
        pedestrianType = GetComponentInParent<Pedestrian>().pedestrianType;
        navMeshAgent = GetComponentInParent<NavMeshAgent>();
    }

    public override bool ShouldTriggerBehaviour()
    {
        if(pathOfPedestrianPoints == null || pathOfPedestrianPoints.Count == 0)
            return true;

        return false;
    }

    public override void PerformBehaviour()
    {
        pathOfPedestrianPoints = pedestrianPointPathCreator.GeneratePathBasedOnPedestrianType(pedestrianType);
        navMeshAgent.SetDestination(pathOfPedestrianPoints[0].GetPointLocation());
    }
}
