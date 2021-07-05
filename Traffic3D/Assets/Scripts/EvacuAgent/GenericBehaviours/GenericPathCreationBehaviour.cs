using System.Collections.Generic;
using UnityEngine;

public class GenericPathCreationBehaviour : BehaviourStrategy
{
    public List<Vector3> Path { get; set; }
    public NonShooterPedestrianPointPathCreator PedestrianPointPathCreator { get; private set; }
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private GroupCollection groupCollection;
    private bool isDebuggingOn = false; // turn on for gizmo debugging

    private void Start()
    {
        Path = new List<Vector3>();
        PedestrianPointPathCreator = GetComponentInParent<NonShooterPedestrianPointPathCreator>();
        evacuAgentPedestrianBase = GetComponentInParent<EvacuAgentPedestrianBase>();
        groupCollection = evacuAgentPedestrianBase.GroupCollection;
    }

    public override bool ShouldTriggerBehaviour()
    {
        return groupCollection.shouldUpdatePath;
    }

    public override void PerformBehaviour()
    {
        Path = PedestrianPointPathCreator.CreatePath();
        groupCollection.UpdatePath(Path);
        evacuAgentPedestrianBase.navMeshAgent.SetDestination(groupCollection.GroupDestination);

    }

    private void OnDrawGizmos()
    {
        if (!isDebuggingOn)
            return;

        Gizmos.color = Color.red;

        if (evacuAgentPedestrianBase.navMeshAgent.path.corners.Length == 0)
            return;

        for (int index = 0; index < evacuAgentPedestrianBase.navMeshAgent.path.corners.Length; index++)
        {
            Gizmos.DrawLine(evacuAgentPedestrianBase.navMeshAgent.path.corners[index], evacuAgentPedestrianBase.navMeshAgent.path.corners[index + 1]);
        }

        Gizmos.color = Color.white;
    }
}
