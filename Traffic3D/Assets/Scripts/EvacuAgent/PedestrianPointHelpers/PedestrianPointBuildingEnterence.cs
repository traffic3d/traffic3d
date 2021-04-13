using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PedestrianPointBuildingEnterence : MonoBehaviour
{
    private PedestrianPoint pedestrianPoint;
    private int timeToWaitLowerBounds;
    private int timeToWaitUpperBounds;
    //private Dictionary<GroupCollection, int> groupSecondsToWait;

    // In the int array the value at 0 is the time each group member will wait
    // The value at 1 is the number of members of the group left to recieve the time
    // After all have been given the time the group collection is removed from the dictionary to allow re-visits with different times
    private Dictionary<GroupCollection, int[]> groupSecondsToWait;

    private void Start()
    {
        pedestrianPoint = GetComponent<PedestrianPoint>();
        SetSecondsToWaitBounds();
        groupSecondsToWait = new Dictionary<GroupCollection, int[]>();
    }

    void OnTriggerEnter(Collider collider)
    {
        GenericEnterLeaveBuildingBehaviour genericEnterLeaveBuildingBehaviour = collider.transform.GetComponentInChildren<GenericEnterLeaveBuildingBehaviour>();

        // Check if the pedestrian can enter buildings
        if (genericEnterLeaveBuildingBehaviour == null)
            return;

        EvacuAgentPedestrianBase evacuAgentPedestrianBase = collider.GetComponentInChildren<EvacuAgentPedestrianBase>();
        GroupCollection groupCollection = evacuAgentPedestrianBase.GroupCollection;

        // Check if this pedestrian point if the group members destination
        if (!groupCollection.GroupDestination.Equals(transform.position))
            return;

        // Check to see if a time has already been allocated to this group
        if(!groupSecondsToWait.ContainsKey(groupCollection))
        {
            int secondsToWait = Random.Range(timeToWaitLowerBounds, timeToWaitUpperBounds);
            int[] secondsToWaitAndMembersLeft = new int[] { secondsToWait, groupCollection.GetGroupMembers().Count };
            groupSecondsToWait.Add(groupCollection, secondsToWaitAndMembersLeft);
        }

        int waitTime = groupSecondsToWait[groupCollection][0];
        groupSecondsToWait[groupCollection][1]--;

        genericEnterLeaveBuildingBehaviour.SetBuildingStayParamaters(waitTime, true);

        // Removes the group collection from the dictionary as per the comment at the top of this script
        if (groupSecondsToWait[groupCollection][1] == 0)
            groupSecondsToWait.Remove(groupCollection);
    }

    private void SetSecondsToWaitBounds()
    {
        if(pedestrianPoint.PedestrianPointType == PedestrianPointType.Hospitality)
        {
            timeToWaitLowerBounds = EvacuAgentSceneParamaters.HOSPITALITY_WAIT_TIME_LOWER_BOUND;
            timeToWaitUpperBounds = EvacuAgentSceneParamaters.HOSPITALITY_WAIT_TIME_UPPER_BOUND;
        }
        else if(pedestrianPoint.PedestrianPointType == PedestrianPointType.Work)
        {
            timeToWaitLowerBounds = EvacuAgentSceneParamaters.WORK_WAIT_TIME_LOWER_BOUND;
            timeToWaitUpperBounds = EvacuAgentSceneParamaters.WORK_WAIT_TIME_UPPER_BOUND;
        }
    }
}
