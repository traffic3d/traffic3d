﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GenericEnterLeaveBuildingBehaviour : BehaviourStrategy
{
    private Vector3 inBuildingScale = new Vector3(0.0001f, 0.0001f, 0.0001f); // These values are aritrary, they are used to shrink Pedestrians to a size the user cannot see to simulate being inside a building
    private Vector3 originalScale;
    private Collider pedestrianCollider;
    private Pedestrian pedestrian;
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
    private NavMeshAgent navMeshAgent;
    private List<EvacuAgentPedestrianBase> visibleGroupMembers;
    private bool isAbleToEnterBuilding;
    private int secondsToWait;
    private bool isEnterBuildingCoolDownActive;
    private int enterBuildingCoolDownSeconds;

    private void Start()
    {
        navMeshAgent = GetComponentInParent<NavMeshAgent>();
        pedestrian = GetComponentInParent<Pedestrian>();
        evacuAgentPedestrianBase = GetComponentInParent<EvacuAgentPedestrianBase>();
        originalScale = pedestrian.transform.localScale;
        pedestrianCollider = GetComponentInParent<Collider>();
        isAbleToEnterBuilding = false;
        isEnterBuildingCoolDownActive = false;
        enterBuildingCoolDownSeconds = 6;
        visibleGroupMembers = new List<EvacuAgentPedestrianBase>();
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
        visibleGroupMembers = evacuAgentPedestrianBase.GetVisibleGroupMemebers();
        isAbleToEnterBuilding = false;
        pedestrian.transform.localScale = inBuildingScale;
        //navMeshAgent.isStopped = true;
        evacuAgentPedestrianBase.IsPedestrianMovementStopped(true);
        pedestrianCollider.enabled = false;
        ScaleAllVisibleGroupMembers(visibleGroupMembers, inBuildingScale, true, false);

        yield return new WaitForSeconds(secondsToWait);
        navMeshAgent.isStopped = false;
        pedestrian.transform.localScale = originalScale;
        pedestrianCollider.enabled = true;
        evacuAgentPedestrianBase.IsPedestrianMovementStopped(false);
        ScaleAllVisibleGroupMembers(visibleGroupMembers, originalScale, false, true);
        visibleGroupMembers.Clear();
        //evacuAgentPedestrianBase.GroupCollection.UpdateGroupDestination(); // This might be a problem as it can interfere with group leader wait for follower behaviour
        StartCoroutine(StartEnterBuildingCooldown()); // May not ne necessary with visited node system
    }

    private void ScaleAllVisibleGroupMembers(List<EvacuAgentPedestrianBase> groupMemebers, Vector3 scale, bool isNavMeshAgentStopped, bool isPedestrianColliderEnabled)
    {
        foreach(EvacuAgentPedestrianBase visibleGroupMember in groupMemebers)
        {
            Transform visibleGroupMemberTransform = visibleGroupMember.transform.root;
            visibleGroupMemberTransform.localScale = scale;
            //visibleGroupMemberTransform.GetComponent<NavMeshAgent>().isStopped = isNavMeshAgentStopped;
            visibleGroupMemberTransform.GetComponent<Collider>().enabled = isPedestrianColliderEnabled;
            visibleGroupMember.GetComponentInChildren<GenericEnterLeaveBuildingBehaviour>().StartCoroutine(StartEnterBuildingCooldown());
            visibleGroupMember.IsPedestrianMovementStopped(isNavMeshAgentStopped);
        }
    }
}
