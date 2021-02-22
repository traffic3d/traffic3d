using System;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianPathCreator : MonoBehaviour
{
    public float CurrentMaximumFootfall { get; set; }
    public float CurrentMinimumDistance { get; set; }
    public Dictionary<int, float> CriteriaMinMaxValues { get; set; }

    private const string footfall = "footfall";
    private const string distance = "distance";
    private const int footfallMinMaxIndex = 0;
    private const int distanceMinMaxIndex = 1;
    private const string pedestrianPointLayerMaskName = "PedestrianPoint";

    private LayerMask pedestrianPointLayer;

    private void Awake()
    {
        pedestrianPointLayer = LayerMask.GetMask(pedestrianPointLayerMaskName);
        CriteriaMinMaxValues = new Dictionary<int, float>();
    }

    public PedestrianPoint[] CalculateRankedShooterAgentPath(float radius, Transform transform, int sizeOfPath, float footfallWeighting, float distanceWeighting)
    {
        CurrentMaximumFootfall = 0f;
        CurrentMinimumDistance = float.MaxValue;
        CriteriaMinMaxValues.Clear();

        PedestrianPoint[] pedestrianPoints = GetAllPedestrianPointsInRadius(transform, radius);
        List<PathDecisionOption> pathDecisionOptions = CreatePathDecisionMatrix(pedestrianPoints, transform, footfallWeighting, distanceWeighting);
        CalculateWeightedSumOfNormalisedPathOptions(pathDecisionOptions);
        pathDecisionOptions.Sort((x, y) => y.WeightedSumOfPathNodes.CompareTo(x.WeightedSumOfPathNodes));

        return GetRankedPedestrianPoints(pathDecisionOptions, sizeOfPath);
    }

    public PedestrianPoint[] GetAllPedestrianPointsInRadius(Transform transform, float radius)
    {
        Collider[] collidersInRadius = Physics.OverlapSphere(transform.position, radius, pedestrianPointLayer);
        List<PedestrianPoint> pedestrianPoints = new List<PedestrianPoint>();

        foreach(Collider collider in collidersInRadius)
        {
            pedestrianPoints.Add(collider.GetComponent<PedestrianPoint>());
        }

        return pedestrianPoints.ToArray();
    }

    public List<PathDecisionOption> CreatePathDecisionMatrix(PedestrianPoint[] pedestrianPoints, Transform transform, float footfallWeighting, float distanceWeighting)
    {
        List<PathDecisionOption> pathDecisionOptions = new List<PathDecisionOption>();

        for (int outerIndex = 0; outerIndex < pedestrianPoints.Length; outerIndex++)
        {
            PathDecisionNode footfallNode = new PathDecisionNode()
            {
                DecisionNodeValue = pedestrianPoints[outerIndex].footfall,
                IsDecisionNodeBeneficial = CriteriaValues.GetCriteriaValueFromName(footfall),
                MinMaxValueIndex = footfallMinMaxIndex,
                NodeWeighting = footfallWeighting
            };

            PathDecisionNode distanceNode = new PathDecisionNode()
            {
                DecisionNodeValue = Vector3.Distance(transform.position, pedestrianPoints[outerIndex].transform.position),
                IsDecisionNodeBeneficial = CriteriaValues.GetCriteriaValueFromName(distance),
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

    public float NormaliseValue(float valueToNormalise, bool isbeneficial, float valueToAdjustBy)
    {
        if (isbeneficial)
        {
            return valueToNormalise / valueToAdjustBy;
        }

        return valueToAdjustBy / valueToNormalise;
    }

    public PedestrianPoint[] GetRankedPedestrianPoints(List<PathDecisionOption> pathDecisionOptions, int sizeOfPath)
    {
        pathDecisionOptions.Sort((x, y) => y.WeightedSumOfPathNodes.CompareTo(x.WeightedSumOfPathNodes));
        PedestrianPoint[] pedestrianPoints = new PedestrianPoint[sizeOfPath];

        for(int index = 0; index < sizeOfPath; index++)
        {
            pedestrianPoints[index] = pathDecisionOptions[index].PedestrianPoint;
        }

        return pedestrianPoints;
    }
}

public class PathDecisionOption
{
    public PedestrianPoint PedestrianPoint { get; set; }
    public int PathDecisionRank { get; set; }
    public float WeightedSumOfPathNodes { get; set; }
    public List<PathDecisionNode> PathDecisionNodes { get; set; }

    public PathDecisionOption()
    {
        PathDecisionNodes = new List<PathDecisionNode>();
    }
}

public class PathDecisionNode
{
    public float DecisionNodeValue { get; set; }
    public bool IsDecisionNodeBeneficial { get; set; }
    public int MinMaxValueIndex { get; set; }
    public float NodeWeighting { get; set; }
}

public static class CriteriaValues
{
    private static Dictionary<string, bool> criteriaDictionary = new Dictionary<string, bool>()
    {
        { "footfall", true },
        { "distance", false }
    };

    public static bool GetCriteriaValueFromName(string name)
    {
        if (criteriaDictionary.ContainsKey(name))
        {
            return criteriaDictionary[name];
        }

        Debug.Log($"The name \"{name}\" is not in the CriteriaValueDictionary");
        throw new ArgumentException();
    }
}
