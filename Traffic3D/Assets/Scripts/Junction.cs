using System.Linq;
using UnityEngine;

public class Junction : MonoBehaviour
{
    public string junctionId;
    public int currentState;
    public JunctionState[] junctionStates;

    private void Awake()
    {
        junctionStates = GetComponentsInChildren<JunctionState>();
    }

    public string GetJunctionId()
    {
        return junctionId;
    }

    public int GetCurrentState()
    {
        return currentState;
    }

    public JunctionState[] GetJunctionStates()
    {
        return junctionStates;
    }

    public void SetJunctionState(JunctionState junctionState)
    {
        this.currentState = junctionState.GetStateNumber();
        foreach (JunctionState.TrafficLightState trafficLightState in junctionState.GetStates())
        {
            TrafficLightManager.GetInstance().GetTrafficLight(trafficLightState.GetTrafficLightId()).SetColour(trafficLightState.GetLightColour());
        }
    }

    public void SetNextJunctionState()
    {
        if (junctionStates.Length == 0)
        {
            return;
        }
        JunctionState junctionState = GetJunctionState(currentState + 1);
        if (junctionState != null)
        {
            SetJunctionState(junctionState);
        }
        else
        {
            SetJunctionState(junctionStates[0]);
        }
    }

    public JunctionState GetJunctionState(int stateNumber)
    {
        return junctionStates.ToList().Find(junctionState => junctionState.GetStateNumber() == stateNumber);
    }
}
