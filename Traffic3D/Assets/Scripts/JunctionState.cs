using UnityEngine;

public class JunctionState : MonoBehaviour
{
    public int stateNumber;
    public TrafficLightState[] states;

    public int GetStateNumber()
    {
        return stateNumber;
    }

    public TrafficLightState[] GetStates()
    {
        return states;
    }

    [System.Serializable]
    public class TrafficLightState
    {
        public string trafficLightId;
        public TrafficLight.LightColour lightColour;

        public TrafficLightState(string trafficLightId, TrafficLight.LightColour lightColour)
        {
            this.trafficLightId = trafficLightId;
            this.lightColour = lightColour;
        }

        public string GetTrafficLightId()
        {
            return trafficLightId;
        }

        public TrafficLight.LightColour GetLightColour()
        {
            return lightColour;
        }
    }
}
