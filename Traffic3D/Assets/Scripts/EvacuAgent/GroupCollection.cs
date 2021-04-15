using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroupCollection : MonoBehaviour
{
    public EvacuAgentPedestrianBase GroupLeaderPedestrian { get; set; }
    public Vector3 GroupDestination { get; set; }
    public int TotalGroupCount { get; set; }
    public bool ShouldUpdateGroupDestination { get; private set; }
    private bool hasNewMemberBeenAdded;
    private List<EvacuAgentPedestrianBase> group;
    private List<Vector3> path;

    // Current destination and it's visited status. Gets set to true when the first group member hits the relevant collider
    private KeyValuePair<Vector3, bool> visitedStateOfCurrentDestination;

    private void Awake()
    {
        group = new List<EvacuAgentPedestrianBase>();
        hasNewMemberBeenAdded = false;
        path = new List<Vector3>();
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

    public void CanUpdateGroupDestination(bool updateDestination)
    {
        ShouldUpdateGroupDestination = updateDestination;
    }

    public void UpdatePath(List<Vector3> newPath)
    {
        path.Clear();
        path.AddRange(newPath);
        GroupDestination = path.First();
        path.Remove(GroupDestination);
        visitedStateOfCurrentDestination = new KeyValuePair<Vector3, bool>(GroupDestination, false);
    }

    public void UpdateGroupDestination()
    {
        GroupDestination = path.First();
        path.Remove(GroupDestination);
        visitedStateOfCurrentDestination = new KeyValuePair<Vector3, bool>(GroupDestination, false);
    }

    public void MarkCurrentDestinationAsVisited()
    {
        visitedStateOfCurrentDestination = new KeyValuePair<Vector3, bool>(GroupDestination, true);
    }

    public bool HasGroupVisitedCurrentPathNode()
    {
        if(visitedStateOfCurrentDestination.Value == true)
        {
            return true;
        }

        return false;
    }

    public int GetNumberOfPathNodes() => path.Count;
}
