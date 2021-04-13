using System.Linq;
using UnityEngine;

public class GenericMoveToNextDestinationBehaviour : BehaviourStrategy
{
    public Vector3 CurrentDestination { get; private set; }
    private GenericPathCreationBehaviour genericPathCreationBehaviour;
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private readonly float proximityToDestination = 3f;
    private GroupCollection groupCollection;

    private void Start()
    {
        genericPathCreationBehaviour = GetComponent<GenericPathCreationBehaviour>();
        evacuAgentPedestrianBase = GetComponentInParent<EvacuAgentPedestrianBase>();
        groupCollection = evacuAgentPedestrianBase.GroupCollection;
    }

    public override bool ShouldTriggerBehaviour()
    {
        float distanceToTarget = Vector3.Distance(evacuAgentPedestrianBase.GroupCollection.GroupDestination, transform.position);
        if (distanceToTarget < proximityToDestination)
        {
            return true;
        }

        return false;
    }

    public override void PerformBehaviour()
    {
        return;
        CurrentDestination = genericPathCreationBehaviour.Path.First();
        genericPathCreationBehaviour.Path.Remove(CurrentDestination);
        evacuAgentPedestrianBase.GroupCollection.GroupDestination = CurrentDestination;
    }
}
