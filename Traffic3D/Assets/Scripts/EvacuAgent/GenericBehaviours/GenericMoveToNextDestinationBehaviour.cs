using UnityEngine.AI;

public class GenericMoveToNextDestinationBehaviour : BehaviourStrategy
{
    public PedestrianPoint CurrentPedestrianPointDestination { get; private set; }
    private GenericPathCreationBehaviour genericPathCreationBehaviour;
    private NavMeshAgent navMeshAgent;
    private int currentPathIndex;
    private readonly float proximityToDestination = 1f;

    private void Start()
    {
        genericPathCreationBehaviour = GetComponent<GenericPathCreationBehaviour>();
        navMeshAgent = GetComponentInParent<NavMeshAgent>();
        currentPathIndex = 1;
    }

    public override bool ShouldTriggerBehaviour()
    {
        if (navMeshAgent.remainingDistance < proximityToDestination)
        {
            return true;
        }

        return false;
    }

    public override void PerformBehaviour()
    {
        if (currentPathIndex < genericPathCreationBehaviour.PathOfPedestrianPoints.Count)
        {
            CurrentPedestrianPointDestination = genericPathCreationBehaviour.PathOfPedestrianPoints[currentPathIndex];
            navMeshAgent.SetDestination(CurrentPedestrianPointDestination.GetPointLocation());
            currentPathIndex++;
        }
    }
}
