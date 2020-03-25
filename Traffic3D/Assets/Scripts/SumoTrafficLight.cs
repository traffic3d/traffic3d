using System;
using System.Collections.Generic;
using System.Linq;

public class SumoTrafficLight
{
    public TrafficLight trafficLight;
    public string junctionId;
    public HashSet<int> stateIndexes;

    public SumoTrafficLight(TrafficLight trafficLight, string junctionId, HashSet<int> stateIndexes)
    {
        this.trafficLight = trafficLight;
        this.junctionId = junctionId;
        this.stateIndexes = stateIndexes;
    }

    public void UpdateTrafficLight(string state)
    {
        trafficLight.SetColour(GetLightColourFromStateString(state));
    }

    public string GetStateFromTrafficLightColour(string currentState)
    {
        char[] charArray = currentState.ToCharArray();
        char value = SumoTrafficLightCharacterState.GetCharacterFromLightColour(trafficLight.GetCurrentLightColour());
        foreach (int stateIndex in stateIndexes)
        {
            charArray.SetValue(value, stateIndex);
        }
        return new string(charArray);
    }

    public TrafficLight.LightColour GetLightColourFromStateString(string state)
    {
        char[] charArray = state.ToCharArray();
        char mostCommonChar = stateIndexes.Select(index => charArray[index]).GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key;
        return SumoTrafficLightCharacterState.GetLightColourFromCharacter(mostCommonChar);
    }

    public HashSet<int> GetIndexStates()
    {
        return stateIndexes;
    }

    public void AddIndexState(int i)
    {
        stateIndexes.Add(i);
    }

}
