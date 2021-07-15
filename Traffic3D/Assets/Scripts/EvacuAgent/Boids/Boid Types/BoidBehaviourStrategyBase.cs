using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BoidBehaviourStrategyBase : BehaviourStrategy
{
    public Vector3 Velocity { get; set; }
    public Vector3 Target { get; protected set; }
    public Vector3 NeighbourCenter { get; set; }
    public FieldOfView FieldOfView { get; protected set; }
    public GroupCollection GroupCollection { get; set; }
    public List<BoidBehaviourStrategyBase> Neighbours { get; set; }
    public List<BoidBehaviourStrategyBase> NonGroupNeighbours { get; set; }
    public EvacuAgentPedestrianBase EvacuAgentPedestrianBase { get; protected set; }
    public NavMeshAgent NavMeshAgent { get; protected set; }
    public BoidManager BoidManager { get; protected set; }
    public abstract float CohesionWeight { get; }
    public abstract float SeparationWeight { get; }
    public abstract float TargetSeekingWeight { get; }
    public abstract float InterGroupSeparationWeight { get; }
    protected abstract bool IsDebuggingOn { get; }
    protected bool shouldUpdateBoid;
    public float maxSpeedMetresSecond = 2.4f;

    //Debugging
    protected bool isDebuggingOn;
    protected Vector3 newVelocityCache;
    protected Vector3 navMeshVelocityCahce;

    protected void Start()
    {
        EvacuAgentPedestrianBase = GetComponentInParent<EvacuAgentPedestrianBase>();
        FieldOfView = EvacuAgentPedestrianBase.fieldOfView;
        GroupCollection = EvacuAgentPedestrianBase.GroupCollection;
        NavMeshAgent = EvacuAgentPedestrianBase.navMeshAgent;
        Neighbours = new List<BoidBehaviourStrategyBase>();
        NonGroupNeighbours = new List<BoidBehaviourStrategyBase>();
        BoidManager = EvacuAgentPedestrianBase.boidManager;
        Velocity = Vector3.zero;
        shouldUpdateBoid = true;
    }

    public void ShouldBoidLogicBeActive(bool shouldBoidLogicBeOn)
    {
        if (!shouldUpdateBoid)
            Velocity = Vector3.zero;

        shouldUpdateBoid = shouldBoidLogicBeOn;
    }

    public void UpdateNeighbours()
    {
        List<EvacuAgentPedestrianBase> visibleGroupMemebers = EvacuAgentPedestrianBase.GetVisibleGroupMembers();
        List<EvacuAgentPedestrianBase> visibleNonGroupMemebers = EvacuAgentPedestrianBase.GetVisibleNonGroupMembers();

        Neighbours.Clear();
        NonGroupNeighbours.Clear();

        foreach(EvacuAgentPedestrianBase visibleGroupMember in visibleGroupMemebers)
        {
            Neighbours.Add(visibleGroupMember.GetComponentInChildren<BoidBehaviourStrategyBase>());
        }

        foreach (EvacuAgentPedestrianBase visibleNonGroupMember in visibleNonGroupMemebers)
        {
            NonGroupNeighbours.Add(visibleNonGroupMember.GetComponentInChildren<BoidBehaviourStrategyBase>());
        }
    }

    protected bool IsDestinationValid(Vector3 proposedDestination)
    {
        NavMeshPath navMeshPath = new NavMeshPath();
        return NavMeshAgent.CalculatePath(proposedDestination, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete;
    }

    public Vector3 LimitVelocity(Vector3 newVelocity)
    {
        if (newVelocity.magnitude > maxSpeedMetresSecond)
        {
            newVelocity = newVelocity / newVelocity.magnitude * maxSpeedMetresSecond;
        }
        return newVelocity;
    }

    public void CalculateNeighbourPoint()
    {
        Vector3 groupCenter = Vector3.zero;

        foreach (BoidBehaviourStrategyBase boidBehaviourStrategyBase in Neighbours)
        {
            groupCenter += boidBehaviourStrategyBase.transform.position;
        }

        // Include myself in the center point average
        groupCenter += transform.position;
        groupCenter /= Neighbours.Count + 1;
        NeighbourCenter = groupCenter;
    }

    protected void CalculateNewVelocity()
    {
        UpdateNeighbours();
        CalculateNeighbourPoint();
        Vector3 newVelocity = BoidManager.CalculateNewVelocity();
        newVelocity = LimitVelocity(NavMeshAgent.velocity += newVelocity.normalized);
        NavMeshAgent.velocity = newVelocity;

        newVelocityCache = newVelocity;
        navMeshVelocityCahce = NavMeshAgent.velocity;
    }

    private void OnDrawGizmos()
    {
        if (!IsDebuggingOn)
            return;

        Gizmos.color = Color.red; // Red is to the group destination
        Gizmos.DrawLine(transform.position, GroupCollection.GroupDestination);
        Gizmos.color = Color.blue; // Blue is to the calculate new velocity
        Gizmos.DrawRay(transform.position, newVelocityCache);
        Gizmos.color = Color.black; // Black is to the navMeshAgentVelocity
        Gizmos.DrawRay(transform.position, navMeshVelocityCahce);
        Gizmos.color = Color.white;
    }
}
