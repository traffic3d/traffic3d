using System.Collections.Generic;
using UnityEngine;

public class GroupCollection : MonoBehaviour
{
    public int TotalGroupCount { get; set; }
    public EvacuAgentPedestrianBase GroupLeaderPedestrian { get; set; }
    public Vector3 GroupDestination { get; set; }
    private List<EvacuAgentPedestrianBase> group;
    private bool hasNewMemberBeenAdded;

    private void Awake()
    {
        group = new List<EvacuAgentPedestrianBase>();
        hasNewMemberBeenAdded = false;
    }

    public void AddFollowerToCollection(EvacuAgentPedestrianBase evacuAgentPedestrianBase)
    {
        hasNewMemberBeenAdded = true;
        group.Add(evacuAgentPedestrianBase);
    }

    public List<EvacuAgentPedestrianBase> GetGroupMembers()
    {
        return group;
    }

    public bool HasNewFollowerBeenAdded()
    {
        return hasNewMemberBeenAdded;
    }

    public void ResetHasNewFollowerBeenAdded()
    {
        hasNewMemberBeenAdded = false;
    }
}
