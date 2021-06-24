using System;
using System.Collections.Generic;

public abstract class NonShooterPedestrianPointPathCreator : PedestrianPointPathCreator
{
    private PedestrianPoint[] allPedestrianPoints;
    private Dictionary<PedestrianPointType, List<PedestrianPoint>> pedestrianPointDictionary;

    private void Start()
    {
        allPedestrianPoints = FindObjectsOfType<PedestrianPoint>();
        pedestrianPointDictionary = new Dictionary<PedestrianPointType, List<PedestrianPoint>>();
        SetUpPedestrianPointDictionary();
    }

    protected List<PedestrianPoint> GetPedestrianPointsOfType(PedestrianPointType pedestrianPointType)
    {
        List<PedestrianPoint> pedestrianPointsOfType = new List<PedestrianPoint>();

        foreach (PedestrianPoint pedestrianPoint in allPedestrianPoints)
        {
            if (pedestrianPoint.pedestrianPointType.Equals(pedestrianPointType))
                pedestrianPointsOfType.Add(pedestrianPoint);
        }

        return pedestrianPointsOfType;
    }

    protected PedestrianPoint GetRandomPedestrianPointOfType(PedestrianPointType pedestrianPointType)
    {
        List<PedestrianPoint> pedestrianPoints = pedestrianPointDictionary[pedestrianPointType];
        PedestrianPoint pedestrianPointDestination = pedestrianPoints[UnityEngine.Random.Range(0, pedestrianPoints.Count)];
        return pedestrianPointDestination;
    }

    private void SetUpPedestrianPointDictionary()
    {
        foreach(PedestrianPointType pedestrianPointType in Enum.GetValues(typeof(PedestrianPointType)))
        {
            pedestrianPointDictionary.Add(pedestrianPointType, GetPedestrianPointsOfType(pedestrianPointType));
        }
    }
}
