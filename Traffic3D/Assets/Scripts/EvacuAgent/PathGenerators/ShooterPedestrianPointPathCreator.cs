using System;
using System.Collections.Generic;
using UnityEngine;

public class ShooterPedestrianPointPathCreator : PedestrianPointPathCreator
{
    public Dictionary<int, float> CriteriaMinMaxValues { get; set; }

    private InvalidCriteriaValuePopup invalidCriteriaValuePopup;
    private const string footfall = "footfall";
    private const string distance = "distance";
    private const int footfallMinMaxIndex = 0;
    private const int distanceMinMaxIndex = 1;
    private const string pedestrianPointLayerMaskName = "PedestrianPoint";

    private float CurrentMaximumFootfall;
    private float CurrentMinimumDistance;
    private LayerMask pedestrianPointLayer;
    private const bool invalidCriteriaValueBool = true;
    private float radius = 100f;
    private int sizeOfPath = 3;

    // Note that these weighting values must sum to 1 as they represent probability of choosing a PedestrianPoint
    private float footfallWeighting = 0.7f;
    private float distanceWeighting = 0.3f;


    private void Awake()
    {
        invalidCriteriaValuePopup = GameObject.FindObjectOfType<InvalidCriteriaValuePopup>();
        pedestrianPointLayer = LayerMask.GetMask(pedestrianPointLayerMaskName);
        CriteriaMinMaxValues = new Dictionary<int, float>();
    }

    public override List<Vector3> CreatePath()
    {
        List<PedestrianPoint> pedestrianPoints = GetAllPedestrianPointsInRadius(transform, radius);
        List<PathDecisionOption> pathDecisionOptions = CreatePathDecisionMatrix(pedestrianPoints, transform, footfallWeighting, distanceWeighting);
        CalculateWeightedSumOfNormalisedPathOptions(pathDecisionOptions);
        pathDecisionOptions.Sort((x, y) => y.WeightedSumOfPathNodes.CompareTo(x.WeightedSumOfPathNodes));
        return GetRankedPedestrianPoints(pathDecisionOptions, sizeOfPath);
    }

    public List<PedestrianPoint> GetAllPedestrianPointsInRadius(Transform transform, float radius)
    {
        Collider[] collidersInRadius = Physics.OverlapSphere(transform.position, radius, pedestrianPointLayer);
        List<PedestrianPoint> pedestrianPoints = new List<PedestrianPoint>();

        foreach (Collider collider in collidersInRadius)
        {
            pedestrianPoints.Add(collider.GetComponentInChildren<PedestrianPoint>());
        }

        return pedestrianPoints;
    }

    public List<PathDecisionOption> CreatePathDecisionMatrix(List<PedestrianPoint> pedestrianPoints, Transform transform, float footfallWeighting, float distanceWeighting)
    {
        List<PathDecisionOption> pathDecisionOptions = new List<PathDecisionOption>();

        CurrentMaximumFootfall = 0f;
        CurrentMinimumDistance = float.MaxValue;
        CriteriaMinMaxValues.Clear();

        for (int outerIndex = 0; outerIndex < pedestrianPoints.Count; outerIndex++)
        {
            PathDecisionNode footfallNode = new PathDecisionNode()
            {
                DecisionNodeValue = pedestrianPoints[outerIndex].footfall,
                IsDecisionNodeBeneficial = TryGetCriteriaValueFromName(footfall, transform),
                MinMaxValueIndex = footfallMinMaxIndex,
                NodeWeighting = footfallWeighting
            };

            PathDecisionNode distanceNode = new PathDecisionNode()
            {
                DecisionNodeValue = Vector3.Distance(transform.position, pedestrianPoints[outerIndex].transform.position),
                IsDecisionNodeBeneficial = TryGetCriteriaValueFromName(distance, transform),
                MinMaxValueIndex = distanceMinMaxIndex,
                NodeWeighting = distanceWeighting
            };

            if (footfallNode.DecisionNodeValue > CurrentMaximumFootfall)
                CurrentMaximumFootfall = footfallNode.DecisionNodeValue;

            if (distanceNode.DecisionNodeValue < CurrentMinimumDistance)
                CurrentMinimumDistance = distanceNode.DecisionNodeValue;

            pathDecisionOptions.Add(
                new PathDecisionOption()
                {
                    PedestrianPoint = pedestrianPoints[outerIndex],
                    PathDecisionNodes =
                    {   footfallNode,
                        distanceNode
                    }
                });
        }

        CriteriaMinMaxValues.Add(footfallMinMaxIndex, CurrentMaximumFootfall);
        CriteriaMinMaxValues.Add(distanceMinMaxIndex, CurrentMinimumDistance);

        return pathDecisionOptions;
    }

    public void CalculateWeightedSumOfNormalisedPathOptions(List<PathDecisionOption> pathDecisionOptions)
    {
        foreach(PathDecisionOption pathDecisionOption in pathDecisionOptions)
        {
            float sumOfDecisionNodeValues = 0f;

            foreach(PathDecisionNode pathDecisionNode in pathDecisionOption.PathDecisionNodes)
            {
                pathDecisionNode.DecisionNodeValue = NormaliseValue(pathDecisionNode.DecisionNodeValue, pathDecisionNode.IsDecisionNodeBeneficial, CriteriaMinMaxValues[pathDecisionNode.MinMaxValueIndex]);
                sumOfDecisionNodeValues += GetWeightedValueOfNode(pathDecisionNode);
            }

            pathDecisionOption.WeightedSumOfPathNodes = sumOfDecisionNodeValues;
        }
    }

    public float GetWeightedValueOfNode(PathDecisionNode pathDecisionNode)
    {
        return pathDecisionNode.DecisionNodeValue * pathDecisionNode.NodeWeighting;
    }

    public float NormaliseValue(float valueToNormalise, bool isBeneficial, float valueToAdjustBy)
    {
        if (isBeneficial)
        {
            return valueToNormalise / valueToAdjustBy;
        }

        return valueToAdjustBy / valueToNormalise;
    }

    public List<Vector3> GetRankedPedestrianPoints(List<PathDecisionOption> pathDecisionOptions, int sizeOfPath)
    {
        pathDecisionOptions.Sort((x, y) => y.WeightedSumOfPathNodes.CompareTo(x.WeightedSumOfPathNodes));
        List<Vector3> pedestrianPoints = new List<Vector3>();

        for(int index = 0; index < sizeOfPath; index++)
        {
            pedestrianPoints.Add(pathDecisionOptions[index].PedestrianPoint.GetPointLocation());
        }

        return pedestrianPoints;
    }

    public bool TryGetCriteriaValueFromName(string name, Transform transform)
    {
        try
        {
            CriteriaValues.GetCriteriaValueFromName(name);
        }
        catch (ArgumentException e)
        {
            invalidCriteriaValuePopup.CreateCriteriaValuePopup(name, invalidCriteriaValueBool, transform);
            return invalidCriteriaValueBool;
        }

        return CriteriaValues.GetCriteriaValueFromName(name);
    }
}
