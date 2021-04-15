using System.Collections.Generic;
using UnityEngine;

public abstract class BuildingPedestrianPointTypeBase : PedestrianPoint
{
    protected abstract int timeToWaitLowerBounds { get; }
    protected abstract int timeToWaitUpperBounds { get; }

    protected void OnTriggerEnter(Collider collider)
    {
        GenericEnterLeaveBuildingBehaviour genericEnterLeaveBuildingBehaviour = collider.transform.GetComponentInChildren<GenericEnterLeaveBuildingBehaviour>();

        // Check if the pedestrian can enter buildings
        if (genericEnterLeaveBuildingBehaviour == null)
            return;

        EvacuAgentPedestrianBase evacuAgentPedestrianBase = collider.GetComponentInChildren<EvacuAgentPedestrianBase>();
        GroupCollection groupCollection = evacuAgentPedestrianBase.GroupCollection;

        // Check if this pedestrian point if the group members destination
        if (Vector3.Distance(groupCollection.GroupDestination, transform.position) > 3f)
            return;

        // If the node has already been visited by the group do not allow others to update group location
        if (groupCollection.HasGroupVisitedCurrentPathNode())
            return;

        groupCollection.MarkCurrentDestinationAsVisited();

        int secondsToWait = Random.Range(timeToWaitLowerBounds, timeToWaitUpperBounds);

        genericEnterLeaveBuildingBehaviour.SetBuildingStayParamaters(secondsToWait, true);
    }
}
