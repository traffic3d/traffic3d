using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupCollection : MonoBehaviour
{
    public EvacuAgentPedestrianBase GroupLeaderPedestrian { get; set; }
    public Vector3 GroupDestination { get; set; }
    public int TotalGroupCount { get; set; }
    public bool shouldUpdatePath { get; set; }
    public bool ShouldUpdateGroupDestination { get; private set; }
    public bool isGroupWaiting { get; private set; }
    private bool hasNewMemberBeenAdded;
    private List<EvacuAgentPedestrianBase> group;
    private List<Vector3> path;
    private int currentPathIndex;
    private bool isDebuggingOn = false;

    // Current destination and it's visited status. Gets set to true when the first group member hits the relevant collider
    private KeyValuePair<Vector3, bool> visitedStateOfCurrentDestination;

    private void Awake()
    {
        group = new List<EvacuAgentPedestrianBase>();
        hasNewMemberBeenAdded = false;
        shouldUpdatePath = true;
        path = new List<Vector3>();
        currentPathIndex = 0;
    }

    public void AddFollowerToCollection(EvacuAgentPedestrianBase evacuAgentPedestrianBase)
    {
        hasNewMemberBeenAdded = true;
        group.Add(evacuAgentPedestrianBase);
    }

    public List<EvacuAgentPedestrianBase> GetGroupMembers() => group;

    public bool HasNewFollowerBeenAdded() => hasNewMemberBeenAdded;

    public void ResetHasNewFollowerBeenAdded()
    {
        hasNewMemberBeenAdded = false;
    }

    public void UpdatePath(List<Vector3> newPath)
    {
        shouldUpdatePath = false;
        currentPathIndex = 0;
        path.Clear();
        path.AddRange(newPath);
        GroupDestination = path[currentPathIndex];
        visitedStateOfCurrentDestination = new KeyValuePair<Vector3, bool>(GroupDestination, false);
    }

    public void UpdateGroupDestination()
    {
        if (currentPathIndex + 1 >= path.Count)
        {
            shouldUpdatePath = true;
            return;
        }

        currentPathIndex++;
        GroupDestination = path[currentPathIndex];
        visitedStateOfCurrentDestination = new KeyValuePair<Vector3, bool>(GroupDestination, false);
    }

    public void MarkCurrentDestinationAsVisited()
    {
        visitedStateOfCurrentDestination = new KeyValuePair<Vector3, bool>(GroupDestination, true);
    }

    public bool HasGroupVisitedCurrentPathNode()
    {
        return visitedStateOfCurrentDestination.Value == true;
    }

    public int GetNumberOfPathNodes() => path.Count;

    public IEnumerator SetGroupWaitingForSeconds(float seconds)
    {
        isGroupWaiting = true;
        yield return new WaitForSeconds(seconds);
        isGroupWaiting = false;
    }

    private void OnDrawGizmos()
    {
        if (!isDebuggingOn)
            return;

        // Draws lines from leader to all group members
        Gizmos.color = Color.blue;
        group.ForEach(x => Gizmos.DrawLine(GroupLeaderPedestrian.transform.position, x.transform.position));
        Gizmos.color = Color.white;
    }
}
