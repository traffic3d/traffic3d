using System.Collections.Generic;
using UnityEngine;

public class GenericPathCreationBehaviour : BehaviourStrategy
{
    public List<Vector3> Path { get; set; }
    public NonShooterPedestrianPointPathCreator PedestrianPointPathCreator { get; private set; }
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;

    private void Start()
    {
        Path = new List<Vector3>();
        PedestrianPointPathCreator = GetComponentInParent<NonShooterPedestrianPointPathCreator>();
        evacuAgentPedestrianBase = GetComponentInParent<EvacuAgentPedestrianBase>();
    }

    public override bool ShouldTriggerBehaviour()
    {
        if(Path.Count == 0)
            return true;

        return false;
    }

    public override void PerformBehaviour()
    {
        Path = PedestrianPointPathCreator.CreatePath();
        evacuAgentPedestrianBase.GroupCollection.GroupDestination = Path[0];
        Path.RemoveAt(0);
    }
}
