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
        currentPathIndex = 0;
    }

    public override bool ShouldTriggerBehaviour()
    {
        if (genericPathCreationBehaviour.Path.Count > 0 &&
            currentPathIndex == 0 || Vector3.Distance(navMeshAgent.destination, transform.position) < proximityToDestination)
        {
            return true;
        }

        return false;
    }

    public override void PerformBehaviour()
    {
        if (currentPathIndex <= genericPathCreationBehaviour.Path.Count)
        {
            CurrentDestination = genericPathCreationBehaviour.Path[currentPathIndex];
            genericPathCreationBehaviour.Path.Remove(CurrentDestination);
            navMeshAgent.SetDestination(CurrentDestination);
            currentPathIndex++;
        }
    }
}
