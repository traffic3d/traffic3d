using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GenericEnterLeaveBuildingBehaviour : BehaviourStrategy
{
    private Vector3 inBuildingScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
    private Vector3 originalScale;
    private CapsuleCollider capsuleCollider;
    private Pedestrian pedestrian;
    private NavMeshAgent navMeshAgent;
    private bool isAbleToEnterBuilding;
    private int secondsToWait;
    private bool isEnterBuildingCoolDownActive;
    private int enterBuildingCoolDown;


    private void Start()
    {
        navMeshAgent = GetComponentInParent<NavMeshAgent>();
        pedestrian = GetComponentInParent<Pedestrian>();
        originalScale = pedestrian.transform.localScale;
        capsuleCollider = GetComponentInParent<CapsuleCollider>();
        isAbleToEnterBuilding = false;
        isEnterBuildingCoolDownActive = false;
        enterBuildingCoolDown = 5;
    }

    public override bool ShouldTriggerBehaviour()
    {
        if (isAbleToEnterBuilding)
            return true;

        return false;
    }

    public override void PerformBehaviour()
    {
        StartCoroutine(StartAgentWaitAtCoffeeShop());
    }

    public void SetBuildingStayParamaters(int secondsToWait, bool isAbleToEnterBuilding)
    {
        if(isEnterBuildingCoolDownActive == false)
        {
            this.isAbleToEnterBuilding = isAbleToEnterBuilding;
            this.secondsToWait = secondsToWait;
        }
    }

    IEnumerator StartAgentWaitAtCoffeeShop()
    {
        isAbleToEnterBuilding = false;
        pedestrian.transform.localScale = inBuildingScale;
        navMeshAgent.isStopped = true;
        capsuleCollider.enabled = false;
        yield return new WaitForSeconds(secondsToWait);
        navMeshAgent.isStopped = false;
        pedestrian.transform.localScale = originalScale;
        capsuleCollider.enabled = true;
        StartCoroutine(StartEnterBuildingCooldown());
    }

    // Ensures the pedestrian cannot continually enter the same building
    IEnumerator StartEnterBuildingCooldown()
    {
        isEnterBuildingCoolDownActive = true;
        yield return new WaitForSeconds(enterBuildingCoolDown);
        isEnterBuildingCoolDownActive = false;
    }
}
