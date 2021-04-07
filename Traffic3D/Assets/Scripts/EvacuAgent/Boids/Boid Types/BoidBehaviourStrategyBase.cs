using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BoidBehaviourStrategyBase : BehaviourStrategy
{
    public Vector3 Position { get; protected set; } // Is this necessary or can i just use transform.position? Have used transform.position for now
    public Vector3 Velocity { get; protected set; }
    public Vector3 Target { get; protected set; }
    public FieldOfView FieldOfView { get; protected set; }
    public GroupCollection GroupCollection { get; protected set; }
    public List<BoidBehaviourStrategyBase> Neighbours { get; protected set; }
    public EvacuAgentPedestrianBase EvacuAgentPedestrianBase { get; protected set; }
    public NavMeshAgent NavMeshAgent { get; protected set; }
    public BoidManager BoidManager { get; protected set; }
    public abstract float CohesionWeight { get; }
    public abstract float AlignmentWeight { get; }
    public abstract float SeparationWeight { get; }
    public abstract float SeekingWeight { get; }
    public abstract float AvoidanceWeight { get; }
    protected abstract float Speed { get; }
    protected bool shouldUpdateBoid;
    private float maxSpeed = 3f;
    private float maxRotationAngle = 2f;

    protected void Start()
    {
        EvacuAgentPedestrianBase = GetComponentInParent<EvacuAgentPedestrianBase>();
        FieldOfView = EvacuAgentPedestrianBase.fieldOfView;
        GroupCollection = EvacuAgentPedestrianBase.GroupCollection;
        NavMeshAgent = EvacuAgentPedestrianBase.navMeshAgent;
        Position = transform.position;
        Velocity = Vector3.zero;
        shouldUpdateBoid = true;
    }

    public void ShouldBoidLogicBeActive(bool shouldBoidLogicBeOn)
    {
        shouldUpdateBoid = shouldBoidLogicBeOn;
    }

    // THIS IS VERY BAD
    protected void UpdateNeighbours()
    {
        Neighbours.Clear();

        foreach(Pedestrian pedestrian in FieldOfView.visiblePedestrians)
        {
            Transform rootPedestrianTransform = pedestrian.transform.root;

            foreach (EvacuAgentPedestrianBase groupPedestrian in GroupCollection.GetGroupMembers())
            {
                if (groupPedestrian.transform.root == transform.root)
                    continue;

                Neighbours.Add(groupPedestrian.GetComponentInChildren<BoidBehaviourStrategyBase>());
            }
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

    protected Vector3 LimitRotation(Vector3 newVelocity)
    {
        return Vector3.RotateTowards(Velocity, newVelocity, maxRotationAngle * Mathf.Deg2Rad, maxSpeed);
    }
}
