using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SumoTrafficLight
{
    public TrafficLight trafficLight;
    public string junctionId;
    public int stateIndex;

    public SumoTrafficLight(TrafficLight trafficLight, string junctionId, int stateIndex)
    {
        this.trafficLight = trafficLight;
        this.junctionId = junctionId;
        this.stateIndex = stateIndex;
    }

    public void UpdateTrafficLight(string state)
    {
        trafficLight.SetColour(GetLightColourFromStateString(state));
    }

    public TrafficLight.LightColour GetLightColourFromStateString(string state)
    {
        return GetLightColourFromCharacter(state.ToCharArray()[stateIndex]);
    }

    public TrafficLight.LightColour GetLightColourFromCharacter(char character)
    {
        if (character == 'r' || character == 'R')
        {
            return TrafficLight.LightColour.RED;
        }
        else if (character == 'y' || character == 'Y')
        {
            return TrafficLight.LightColour.AMBER;
        }
        else if (character == 'g' || character == 'G')
        {
            return TrafficLight.LightColour.GREEN;
        }
        else
        {
            return TrafficLight.LightColour.RED;
        }
    }

}
