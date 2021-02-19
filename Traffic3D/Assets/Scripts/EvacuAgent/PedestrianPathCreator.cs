using System;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianPathCreator : MonoBehaviour
{
    private const string footfall = "footfall";
    private const string distance = "distance";
    private const int footfallMinMaxIndex = 0;
    private const int distanceMinMaxIndex = 1;
    private const string pedestrianPointLayerMaskName = "PedestrianPoint";

    private LayerMask pedestrianPointLayer;
    private float currentMaximumFootfall = 0f;
    private float currentMinimumDistance = float.MaxValue;
    private Dictionary<int, float> criteriaMinMaxValues;

    private void Awake()
    {
        pedestrianPointLayer = LayerMask.GetMask(pedestrianPointLayerMaskName);
        criteriaMinMaxValues = new Dictionary<int, float>();
    }

    public PedestrianPoint[] CalculateRankedShooterAgentPath(float radius, Transform transform, int sizeOfPath, float footfallWeighting, float distanceWeighting)
    {
        currentMaximumFootfall = 0f;
        currentMinimumDistance = float.MaxValue;
        criteriaMinMaxValues.Clear();

        PedestrianPoint[] pedestrianPoints = GetAllPedestrianPointsInRadius(transform, radius);
        List<PathDecisionOption> pathDecisionOptions = CreatePathDecisionMatrix(pedestrianPoints, transform, footfallWeighting, distanceWeighting);
        CalculateWeightedSumOfPathOption(pathDecisionOptions);
        pathDecisionOptions.Sort((x, y) => x.WeightedSumOfPathNodes.CompareTo(y.WeightedSumOfPathNodes));

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

        for (int outerIndex = 0; outerIndex < pedestrianPoints.Length - 1; outerIndex++)
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

            if (footfallNode.DecisionNodeValue > currentMaximumFootfall)
                currentMaximumFootfall = footfallNode.DecisionNodeValue;

            if (distanceNode.DecisionNodeValue < currentMinimumDistance)
                currentMinimumDistance = distanceNode.DecisionNodeValue;

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

        criteriaMinMaxValues.Add(footfallMinMaxIndex, currentMaximumFootfall);
        criteriaMinMaxValues.Add(distanceMinMaxIndex, currentMinimumDistance);

        return pathDecisionOptions;
    }

    public void CalculateWeightedSumOfPathOption(List<PathDecisionOption> pathDecisionOptions)
    {
        foreach(PathDecisionOption pathDecisionOption in pathDecisionOptions)
        {
            float sumOfDecisionNodeValues = 0f;

            foreach(PathDecisionNode pathDecisionNode in pathDecisionOption.PathDecisionNodes)
            {
                pathDecisionNode.DecisionNodeValue = NormaliseValue(pathDecisionNode.DecisionNodeValue, pathDecisionNode.IsDecisionNodeBeneficial, criteriaMinMaxValues[pathDecisionNode.MinMaxValueIndex]);
                sumOfDecisionNodeValues += GetWeightedValueOfNode(pathDecisionNode);
            }

            pathDecisionOption.WeightedSumOfPathNodes = sumOfDecisionNodeValues;
        }
    }

    public float GetWeightedValueOfNode(PathDecisionNode pathDecisionNode)
    {
        return pathDecisionNode.DecisionNodeValue * pathDecisionNode.NodeWeighting;
    }

    public PedestrianPoint[] GetRankedPedestrianPoints(List<PathDecisionOption> pathDecisionOptions, int sizeOfPath)
    {
        pathDecisionOptions.Sort((x, y) => x.WeightedSumOfPathNodes.CompareTo(y.WeightedSumOfPathNodes));
        PedestrianPoint[] pedestrianPoints = new PedestrianPoint[sizeOfPath];

        for(int index = 0; index < sizeOfPath; index++)
        {
            pedestrianPoints[index] = pathDecisionOptions[index].PedestrianPoint;
        }

        return pedestrianPoints;
    }

    public float NormaliseValue(float valueToNormalise, bool isbeneficial, float valueToAdjustBy)
    {
        if (isbeneficial)
        {
            return valueToNormalise / valueToAdjustBy;
        }

        return valueToAdjustBy / valueToNormalise;
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
