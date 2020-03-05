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
        char value = GetCharacterFromLightColour(trafficLight.GetCurrentLightColour());
        foreach(int stateIndex in stateIndexes)
        {
            charArray.SetValue(value, stateIndex);
        }
        return new string(charArray);
    }

    public TrafficLight.LightColour GetLightColourFromStateString(string state)
    {
        char[] charArray = state.ToCharArray();
        char mostCommonChar = stateIndexes.Select(index => charArray[index]).GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key;
        return GetLightColourFromCharacter(mostCommonChar);
    }

    public HashSet<int> GetIndexStates()
    {
        return stateIndexes;
    }

    public void AddIndexState(int i)
    {
        stateIndexes.Add(i);
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

    public char GetCharacterFromLightColour(TrafficLight.LightColour lightColour)
    {
        if (lightColour == TrafficLight.LightColour.RED)
        {
            return 'r';
        }
        else if (lightColour == TrafficLight.LightColour.AMBER)
        {
            return 'y';
        }
        else if (lightColour == TrafficLight.LightColour.GREEN)
        {
            return 'g';
        }
        else
        {
            return 'r';
        }
    }

}
