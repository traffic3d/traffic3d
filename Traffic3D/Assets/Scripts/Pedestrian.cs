using UnityEngine;
using UnityEngine.AI;

public class Pedestrian : MonoBehaviour
{
    private Rigidbody rigidbody;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private Vector3 location;

    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        GoToRandomLocation();
    }

    void Update()
    {
        animator.SetFloat("speed", navMeshAgent.velocity.magnitude);
        if(navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid || navMeshAgent.remainingDistance < 1)
        {
            Destroy(gameObject);
        }
    }

    public void GoToRandomLocation()
    {
        location = GetRandomLocation(200);
        navMeshAgent.SetDestination(location);
        // Chance of walking in road (1% chance)
        if(Random.value > 0.01)
        {
            // Walkable
            navMeshAgent.areaMask = 1;
        }
        else
        {
            // Walkable and Road
            navMeshAgent.areaMask = 9;
        }
    }

    public Vector3 GetRandomLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.GetAreaFromName("walkable")))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

}
