using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Pedestrian : MonoBehaviour
{
    private Rigidbody rigidbody;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private Vector3 location;
    public float walkingInRoadPercentage = 0.01f;
    private int walkingInRoadAreaCost = 5;

    void Start()
    {
        int roadArea = NavMesh.GetAreaFromName("Road");
        int walkableArea = NavMesh.GetAreaFromName("Walkable");
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        // Chance of walking in road
        if (UnityEngine.Random.value > walkingInRoadPercentage)
        {
            // Walkable
            navMeshAgent.areaMask = 1;
        }
        else
        {
            // Walkable and Road
            navMeshAgent.areaMask = (1 << walkableArea) | (1 << roadArea);
            navMeshAgent.SetAreaCost(roadArea, walkingInRoadAreaCost);
        }
        GoToRandomPossibleLocation();
    }

    void Update()
    {
        animator.SetFloat("speed", navMeshAgent.velocity.magnitude);
        if(navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid || navMeshAgent.remainingDistance < 1)
        {
            Destroy(gameObject);
        }
    }

    public void GoToRandomPossibleLocation()
    {
        location = FindRandomPossibleLocation();
        if (location.Equals(Vector3.negativeInfinity))
        {
            Destroy(gameObject);
            return;
        }
        navMeshAgent.SetDestination(location);
    }

    public Vector3 FindRandomPossibleLocation()
    {
        foreach (PedestrianPoint pedestrianPoint in FindObjectsOfType<PedestrianPoint>().OrderBy(a => Guid.NewGuid()).ToList())
        {
            Vector3 location = pedestrianPoint.GetPointLocation();
            if(Vector3.Distance(location, transform.position) < 1)
            {
                continue;
            }
            NavMeshPath path = new NavMeshPath();
            if(navMeshAgent.CalculatePath(location, path))
            {
                if(path.status == NavMeshPathStatus.PathComplete)
                {
                    return location;
                }
            }
        }
        return Vector3.negativeInfinity;
    }

}
