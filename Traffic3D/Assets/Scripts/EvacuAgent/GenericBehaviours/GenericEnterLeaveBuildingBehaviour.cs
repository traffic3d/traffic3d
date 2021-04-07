using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GenericEnterLeaveBuildingBehaviour : BehaviourStrategy
{
    private Vector3 inBuildingScale = new Vector3(0.0001f, 0.0001f, 0.0001f); // These values are aritrary, they are used to shrink Pedestrians to a size the user cannot see to simulate being inside a building
    private Vector3 originalScale;
    private Collider pedestrianCollider;
    private Pedestrian pedestrian;
    private NavMeshAgent navMeshAgent;
    private bool isAbleToEnterBuilding;
    private int secondsToWait;
    private bool isEnterBuildingCoolDownActive;
    private int enterBuildingCoolDownSeconds;

    private void Start()
    {
        navMeshAgent = GetComponentInParent<NavMeshAgent>();
        pedestrian = GetComponentInParent<Pedestrian>();
        originalScale = pedestrian.transform.localScale;
        pedestrianCollider = GetComponentInParent<Collider>();
        isAbleToEnterBuilding = false;
        isEnterBuildingCoolDownActive = false;
        enterBuildingCoolDownSeconds = 5;
    }

    public override bool ShouldTriggerBehaviour()
    {
        if (isAbleToEnterBuilding)
            return true;

        return false;
    }

    public override void PerformBehaviour()
    {
        StartCoroutine(StartAgentWaitAtBuilding());
    }

    public void SetBuildingStayParamaters(int secondsToWait, bool isAbleToEnterBuilding)
    {
        if(!isEnterBuildingCoolDownActive)
        {
            this.isAbleToEnterBuilding = isAbleToEnterBuilding;
            this.secondsToWait = secondsToWait;
        }
    }

    // Ensures the pedestrian cannot continually enter the same building
    public IEnumerator StartEnterBuildingCooldown()
    {
        isEnterBuildingCoolDownActive = true;
        yield return new WaitForSeconds(enterBuildingCoolDownSeconds);
        isEnterBuildingCoolDownActive = false;
    }

    IEnumerator StartAgentWaitAtBuilding()
    {
        isAbleToEnterBuilding = false;
        pedestrian.transform.localScale = inBuildingScale;
        navMeshAgent.isStopped = true;
        pedestrianCollider.enabled = false;
        yield return new WaitForSeconds(secondsToWait);
        navMeshAgent.isStopped = false;
        pedestrian.transform.localScale = originalScale;
        pedestrianCollider.enabled = true;
        StartCoroutine(StartEnterBuildingCooldown());
    }
}
