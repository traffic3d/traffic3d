using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroupCollection : MonoBehaviour
{
    public int TotalGroupCount { get; set; }
    public EvacuAgentPedestrianBase GroupLeaderPedestrian { get; set; }
    public Vector3 GroupDestination { get; set; }
    public bool ShouldUpdateGroupDestination { get; private set; }
    private List<EvacuAgentPedestrianBase> group;
    private bool hasNewMemberBeenAdded;
    private List<Vector3> path;

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
    }

    public void UpdateGroupDestination()
    {
        GroupDestination = path.First();
        path.Remove(GroupDestination);
    }

    public int GetNumberOfPathNodes() => path.Count;
}
