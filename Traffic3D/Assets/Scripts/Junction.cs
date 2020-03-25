﻿using System.Linq;
using UnityEngine;

public class Junction : MonoBehaviour
{
    public string junctionId;
    private int currentState;
    private JunctionState[] junctionStates;
    public Camera junctionCamera;

    private void Start()
    {
        RefreshJunctionStates();
    }

    public void RefreshJunctionStates()
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

    public Camera GetJunctionCamera()
    {
        return junctionCamera;
    }

    public void SetJunctionState(JunctionState junctionState)
    {
        this.currentState = junctionState.GetStateNumber();
        foreach (JunctionState.TrafficLightState trafficLightState in junctionState.GetStates())
        {
            TrafficLightManager.GetInstance().GetTrafficLight(trafficLightState.GetTrafficLightId()).SetColour(trafficLightState.GetLightColour());
        }
    }

    public void SetJunctionState(int junctionStateNumber)
    {
        JunctionState junctionState = GetJunctionState(junctionStateNumber);
        if (junctionState == null)
        {
            return;
        }
        SetJunctionState(junctionState);
    }

    public void SetNextJunctionState()
    {
        if (junctionStates.Length == 0)
        {
            RefreshJunctionStates();
            if (junctionStates.Length == 0)
            {
                return;
            }
        }
        JunctionState junctionState = GetJunctionState(currentState + 1);
        if (junctionState != null)
        {
            SetJunctionState(junctionState);
        }
        else
        {
            SetJunctionState(GetFirstJunctionState());
        }
    }

    public JunctionState GetJunctionState(int stateNumber)
    {
        return junctionStates.ToList().Find(junctionState => junctionState.GetStateNumber() == stateNumber);
    }

    public JunctionState GetFirstJunctionState()
    {
        return GetJunctionState(junctionStates.ToList().Min(junctionState => junctionState.GetStateNumber()));
    }
}
