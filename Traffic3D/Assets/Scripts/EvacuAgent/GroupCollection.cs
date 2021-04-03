using System.Collections.Generic;
using UnityEngine;

public class GroupCollection : MonoBehaviour
{
    public int TotalFollowerCount { get; set; }
    public EvacuAgentPedestrianBase GroupLeaderPedestrian { get; set; }
    public Vector3 GroupDestination { get; set; }
    private List<GroupPedestrian> followers;
    private bool hasNewFollowerBeenAdded;

    private void Awake()
    {
        followers = new List<GroupPedestrian>();
        hasNewFollowerBeenAdded = false;
    }

    public void AddFollowerToCollection(GroupPedestrian evacuAgentPedestrianBase)
    {
        hasNewFollowerBeenAdded = true;
        followers.Add(evacuAgentPedestrianBase);
    }

    public List<GroupPedestrian> GetFollowers()
    {
        return followers;
    }

    public bool HasNewFollowerBeenAdded()
    {
        return hasNewFollowerBeenAdded;
    }

    public void ResetHasNewFollowerBeenAdded()
    {
        hasNewFollowerBeenAdded = false;
    }
}
