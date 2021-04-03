using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BoidBehaviourStrategyBase : BehaviourStrategy
{
    public Vector3 Position { get; protected set; } // Is this necessary or can i just use transform.position? Have uied transform.position for now
    public Vector3 Velocity { get; protected set; }
    public Vector3 Target { get; protected set; }
    public FieldOfView FieldOfView { get; protected set; }
    public GroupCollection GroupCollection { get; protected set; }
    public List<BoidBehaviourStrategyBase> Neighbours { get; protected set; }
    public GroupPedestrian GroupPedestrian { get; protected set; }
    public NavMeshAgent NavMeshAgent { get; protected set; }
    public BoidManager BoidManager { get; protected set; }
    public abstract float CohesionWeight { get; }
    public abstract float AlignmentWeight { get; }
    public abstract float SeparationWeight { get; }
    public abstract float SeekingWeight { get; }

    protected void Start()
    {
        GroupPedestrian = GetComponentInParent<GroupPedestrian>();
        FieldOfView = GroupPedestrian.fieldOfView;
        GroupCollection = GroupPedestrian.GroupCollection;
        NavMeshAgent = GroupPedestrian.navMeshAgent;
        Position = transform.position;
        Velocity = Vector3.zero;
    }

    // THIS IS VERY BAD
    protected void UpdateNeighbours()
    {
        List<Pedestrian> pedestriansInFieldOfView = FieldOfView.visiblePedestrians;

        foreach(Pedestrian pedestrian in pedestriansInFieldOfView)
        {
            Transform rootPedestrianTransform = pedestrian.transform.root;

            foreach(GroupPedestrian groupPedestrian in GroupCollection.GetFollowers())
            {
                if (groupPedestrian.transform.root == transform.root)
                    continue;

                if(GroupCollection.transform.root == rootPedestrianTransform)
                {
                    Neighbours.Add(groupPedestrian.GetComponentInChildren<BoidBehaviourStrategyBase>());
                }
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
}
