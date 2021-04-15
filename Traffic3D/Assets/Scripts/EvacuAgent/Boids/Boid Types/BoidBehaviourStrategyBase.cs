using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BoidBehaviourStrategyBase : BehaviourStrategy
{
    public Vector3 Velocity { get; protected set; }
    public Vector3 Target { get; protected set; }
    public Vector3 NeighbourCenter { get; private set; }
    public FieldOfView FieldOfView { get; protected set; }
    public GroupCollection GroupCollection { get; protected set; }
    public List<BoidBehaviourStrategyBase> Neighbours { get; protected set; }
    public EvacuAgentPedestrianBase EvacuAgentPedestrianBase { get; protected set; }
    public NavMeshAgent NavMeshAgent { get; protected set; }
    public BoidManager BoidManager { get; protected set; }
    public abstract float CohesionWeight { get; }
    public abstract float SeparationWeight { get; }
    protected bool shouldUpdateBoid;
    private float maxSpeed = 2.4f;

    protected void Start()
    {
        EvacuAgentPedestrianBase = GetComponentInParent<EvacuAgentPedestrianBase>();
        FieldOfView = EvacuAgentPedestrianBase.fieldOfView;
        GroupCollection = EvacuAgentPedestrianBase.GroupCollection;
        NavMeshAgent = EvacuAgentPedestrianBase.navMeshAgent;
        Velocity = Vector3.zero;
        shouldUpdateBoid = true;
    }

    public void ShouldBoidLogicBeActive(bool shouldBoidLogicBeOn)
    {
        if (!shouldUpdateBoid)
            Velocity = Vector3.zero;

        shouldUpdateBoid = shouldBoidLogicBeOn;
    }

    protected void UpdateNeighbours()
    {
        List<EvacuAgentPedestrianBase> visibleGroupMemebers =  EvacuAgentPedestrianBase.GetVisibleGroupMemebers();
        Neighbours.Clear();

        foreach(EvacuAgentPedestrianBase visibleGroupMember in visibleGroupMemebers)
        {
            Neighbours.Add(visibleGroupMember.GetComponentInChildren<BoidBehaviourStrategyBase>());
        }
    }

    protected bool IsDestinationValid(Vector3 proposedDestination)
    {
        NavMeshPath navMeshPath = new NavMeshPath();
        if (NavMeshAgent.CalculatePath(proposedDestination, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
        {
            return true;
        }

        return false;
    }

    protected Vector3 LimitVelocity(Vector3 newVelocity)
    {
        if (newVelocity.magnitude > maxSpeed)
        {
            newVelocity = newVelocity / newVelocity.magnitude * maxSpeed;
        }
        return newVelocity;
    }

    protected void CalculateNeighbourPoint()
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
}
