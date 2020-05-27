using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PedestrianCrossing : MonoBehaviour
{
    public string pedestrianCrossingId;
    public List<Pedestrian> pedestriansCurrentlyInCrossingArea = new List<Pedestrian>();
    public bool allowCrossing = false;

    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        if(gameObject.GetComponent<BoxCollider>() == null)
        {
            throw new System.Exception("Pedestrian Crossing " + pedestrianCrossingId + " has no box collider.");
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        Pedestrian pedestrian = collider.gameObject.GetComponent<Pedestrian>();
        if (pedestrian != null)
        {
            pedestriansCurrentlyInCrossingArea.Add(pedestrian);
            pedestrian.SetAllowCrossing(allowCrossing);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        Pedestrian pedestrian = collider.gameObject.GetComponent<Pedestrian>();
        if (pedestrian != null)
        {
            pedestriansCurrentlyInCrossingArea.Remove(pedestrian);
            if(FindObjectsOfType<PedestrianCrossing>().Count(p => p.pedestriansCurrentlyInCrossingArea.Contains(pedestrian)) == 0)
            {
                pedestrian.SetAllowCrossing(true);
            }
        }
    }

    public string GetPedestrianCrossingId()
    {
        return pedestrianCrossingId;
    }

    public void SetAllowCrossing(bool allowCrossing)
    {
        this.allowCrossing = allowCrossing;
        foreach(Pedestrian pedestrian in pedestriansCurrentlyInCrossingArea)
        {
            pedestrian.SetAllowCrossing(allowCrossing);
        }
    }
}
