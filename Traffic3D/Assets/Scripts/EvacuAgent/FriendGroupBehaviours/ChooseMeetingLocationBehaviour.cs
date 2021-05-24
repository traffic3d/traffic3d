using UnityEngine;
using UnityEngine.AI;

public class ChooseMeetingLocationBehaviour : BehaviourStrategy
{
    public bool isMeetingLocationChosen { get; set; }
    private ChooseLocationOnNavmesh chooseLocationOnNavmesh;
    private float radiusOfPointToConsider;
    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        isMeetingLocationChosen = false;
        radiusOfPointToConsider = 200f;
        chooseLocationOnNavmesh = gameObject.AddComponent<ChooseLocationOnNavmesh>();
        navMeshAgent = GetComponentInParent<NavMeshAgent>();
    }

    public override bool ShouldTriggerBehaviour()
    {
        if (!isMeetingLocationChosen)
            return true;

        return false;
    }

    public override void PerformBehaviour()
    {
        PedestrianPoint pedestrianPoint = FindObjectOfType<PedestrianPoint>();
        Vector3 meetingLocation = chooseLocationOnNavmesh.GetRandomPointOnNavMesh(pedestrianPoint.GetPointLocation(), radiusOfPointToConsider);
        navMeshAgent.SetDestination(meetingLocation);
    }
}
