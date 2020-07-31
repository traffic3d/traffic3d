using UnityEngine;

public class JunctionState : MonoBehaviour
{
    public int stateNumber;
    public TrafficLightState[] trafficLightStates;
    public PedestrianCrossingState[] pedestrianCrossingStates;

    public int GetStateNumber()
    {
        return stateNumber;
    }

    public TrafficLightState[] GetTrafficLightStates()
    {
        return trafficLightStates;
    }

    public PedestrianCrossingState[] GetPedestrianCrossingStates()
    {
        return pedestrianCrossingStates;
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

    [System.Serializable]
    public class PedestrianCrossingState
    {
        public string pedestrianCrossingId;
        public bool allowCrossing;

        public PedestrianCrossingState(string pedestrianCrossingId, bool allowCrossing)
        {
            this.pedestrianCrossingId = pedestrianCrossingId;
            this.allowCrossing = allowCrossing;
        }

        public string GetPedestrianCrossingId()
        {
            return pedestrianCrossingId;
        }

        public bool AllowCrossing()
        {
            return allowCrossing;
        }
    }
}
