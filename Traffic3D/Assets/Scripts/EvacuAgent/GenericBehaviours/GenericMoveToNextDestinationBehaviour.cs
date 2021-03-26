using UnityEngine;
using UnityEngine.AI;

public class GenericMoveToNextDestinationBehaviour : BehaviourStrategy
{
    public Vector3 CurrentDestination { get; private set; }
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
        if (currentPathIndex < genericPathCreationBehaviour.Path.Count)
        {
            CurrentDestination = genericPathCreationBehaviour.Path[currentPathIndex];
            genericPathCreationBehaviour.Path.Remove(CurrentDestination);
            navMeshAgent.SetDestination(CurrentDestination);
            currentPathIndex++;
        }
    }
}
