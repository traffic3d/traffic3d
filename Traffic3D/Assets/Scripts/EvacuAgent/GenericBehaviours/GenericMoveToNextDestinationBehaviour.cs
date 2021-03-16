using UnityEngine.AI;

public class GenericMoveToNextDestinationBehaviour : BehaviourStrategy
{
    public PedestrianPoint currentPedestrianPointDestination { get; private set; }
    private GenericPathCreationBehaviour genericPathCreationBehaviour;
    private NavMeshAgent navMeshAgent;
    private int currentPathIndex;
    private readonly float proximityToDestination = 1f;

    private void Start()
    {
        genericPathCreationBehaviour = GetComponent<GenericPathCreationBehaviour>();
        navMeshAgent = GetComponentInParent<NavMeshAgent>();
        currentPathIndex = 0;
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
        if (currentPathIndex + 1 <= genericPathCreationBehaviour.PathOfPedestrianPoints.Count)
        {
            currentPedestrianPointDestination = genericPathCreationBehaviour.PathOfPedestrianPoints[currentPathIndex];
            navMeshAgent.SetDestination(currentPedestrianPointDestination.GetPointLocation());
            currentPathIndex++;
        }
    }
}
