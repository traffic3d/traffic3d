using System.Collections.Generic;
using UnityEngine;

public class FollowerCollection : MonoBehaviour
{
    private List<EvacuAgentPedestrianBase> followers;
    private bool hasNewFollowerBeenAdded;

    private void Start()
    {
        followers = new List<EvacuAgentPedestrianBase>();
        hasNewFollowerBeenAdded = false;
    }

    public void AddFollowerToCollection(EvacuAgentPedestrianBase evacuAgentPedestrianBase)
    {
        hasNewFollowerBeenAdded = true;
        followers.Add(evacuAgentPedestrianBase);
    }

    public List<EvacuAgentPedestrianBase> GetFollowers()
    {
        return followers;
    }

    public bool HasNewFollowerBeenAdded()
    {
        return hasNewFollowerBeenAdded;
    }

    public void ResetHasNewFollowerBeenAdded()
    {
        hasNewFollowerBeenAdded = true;
    }
}
