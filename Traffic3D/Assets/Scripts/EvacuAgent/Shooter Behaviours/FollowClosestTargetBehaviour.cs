using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class FollowClosestTargetBehaviour : BehaviourStrategy
{
    private FieldOfView fieldOfView;
    private NavMeshAgent navMeshAgent;
    private bool isAbleToTargetNewPedestrian;
    private int targettingCoolDown;

    public void Start()
    {
        fieldOfView = GetComponentInParent<Pedestrian>().GetComponentInChildren<FieldOfView>();
        navMeshAgent = GetComponentInParent<NavMeshAgent>();
        isAbleToTargetNewPedestrian = true;
        targettingCoolDown = 4;
    }

    public override bool ShouldTriggerBehaviour()
    {
        if(fieldOfView.allVisiblePedestrians.Count >= 1 && isAbleToTargetNewPedestrian)
        {
            StartCoroutine(FollowNewPedestrianCoolDown());
            return true;
        }

        return false;
    }

    public override void PerformBehaviour()
    {
        Transform cloststTransform = fieldOfView.allVisiblePedestrians.First().transform;
        float currentSmallestDistance = Vector3.Distance(transform.position, cloststTransform.position);

        foreach (Pedestrian pedestrian in fieldOfView.allVisiblePedestrians)
        {
            float distanceToAgent = Vector3.Distance(transform.position, pedestrian.transform.position);

            if (distanceToAgent > currentSmallestDistance)
            {
                cloststTransform = pedestrian.transform;
                currentSmallestDistance = distanceToAgent;
            }
        }

        navMeshAgent.ResetPath();
        navMeshAgent.SetDestination(cloststTransform.position);
    }

    private IEnumerator FollowNewPedestrianCoolDown()
    {
        isAbleToTargetNewPedestrian = false;
        yield return new WaitForSeconds(targettingCoolDown);
        isAbleToTargetNewPedestrian = true;
    }
}
