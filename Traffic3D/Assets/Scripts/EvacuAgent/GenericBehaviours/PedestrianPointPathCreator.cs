using System;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianPointPathCreator : MonoBehaviour
{
    private PedestrianPoint[] allPedestrianPoints;
    private Dictionary<PedestrianPointType, List<PedestrianPoint>> pedestrianPointDictionary;

    private void Start()
    {
        allPedestrianPoints = FindObjectsOfType<PedestrianPoint>();
        pedestrianPointDictionary = new Dictionary<PedestrianPointType, List<PedestrianPoint>>();
        SetUpPedestrianPointDictionary();
    }

    public List<PedestrianPoint> GetPedestrianPointsOfType(PedestrianPointType pedestrianPointType)
    {
        List<PedestrianPoint> pedestrianPointsOfType = new List<PedestrianPoint>();

        foreach (PedestrianPoint pedestrianPoint in allPedestrianPoints)
        {
            if (pedestrianPoint.PedestrianPointType.Equals(pedestrianPointType))
                pedestrianPointsOfType.Add(pedestrianPoint);
        }

        return pedestrianPointsOfType;
    }

    public List<PedestrianPoint> GeneratePathBasedOnPedestrianType(PedestrianType pedestrianType)
    {
        List<PedestrianPoint> path = new List<PedestrianPoint>();

        switch (pedestrianType)
        {
            case PedestrianType.Worker:
                path = GetWorkerPedestrianPath();
                break;
            default:
                path = GetWorkerPedestrianPath();
                break;
        }

        return path;
    }

    private void SetUpPedestrianPointDictionary()
    {
        foreach(PedestrianPointType pedestrianPointType in Enum.GetValues(typeof(PedestrianPointType)))
        {
            pedestrianPointDictionary.Add(pedestrianPointType, GetPedestrianPointsOfType(pedestrianPointType));
        }
    }

    private PedestrianPoint GetRandomPedestrianPointOfType(PedestrianPointType pedestrianPointType)
    {
        List<PedestrianPoint> pedestrianPoints = pedestrianPointDictionary[pedestrianPointType];
        PedestrianPoint pedestrianPointDestination = pedestrianPoints[UnityEngine.Random.Range(0, pedestrianPoints.Count)];
        return pedestrianPointDestination;
    }

    private List<PedestrianPoint> GetWorkerPedestrianPath()
    {
        List<PedestrianPoint> path = new List<PedestrianPoint>();

        if (UnityEngine.Random.value < EvacuAgentSceneParamaters.WORKER_CHANCE_TO_VISIT_HOSPITALITY_POINT_IN_ROUTE)
        {
            path.Add(GetRandomPedestrianPointOfType(PedestrianPointType.Hospitality));
        }

        path.Add(GetRandomPedestrianPointOfType(PedestrianPointType.Work));

        return path;
    }
}
