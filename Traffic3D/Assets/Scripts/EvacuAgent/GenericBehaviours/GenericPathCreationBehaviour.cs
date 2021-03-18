using System.Collections.Generic;
using UnityEngine.AI;

public class GenericPathCreationBehaviour : BehaviourStrategy
{
    public List<PedestrianPoint> PathOfPedestrianPoints { get; set; }
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
        if(PathOfPedestrianPoints == null || PathOfPedestrianPoints.Count == 0)
            return true;

        return false;
    }

    public override void PerformBehaviour()
    {
        PathOfPedestrianPoints = pedestrianPointPathCreator.GeneratePathBasedOnPedestrianType(pedestrianType);
        navMeshAgent.SetDestination(PathOfPedestrianPoints[0].GetPointLocation());
    }
}
