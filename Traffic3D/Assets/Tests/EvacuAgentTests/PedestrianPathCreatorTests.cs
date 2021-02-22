using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Tests;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Tests")]
public class PedestrianPathCreatorTests : EvacuAgentCommonSceneTest
{
    // Path paramaters
    private const float radius = 45f;
    private const float footfallWeighting = 0.7f;
    private const float distanceWeighting = 0.3f;
    private const int sizeOfPath = 3;

    // Expected values
    private const float expectedCurrentMaximumFootfall = 22f;
    private const float expectedCurrentMinimumDistance = 21.946f;
    private const int expectedNumberOfPedestrianPoints = 5;
    private const string pedestrianPointPrefix = "PedestrianFactory/PedestrianPoint";

    // Decision matrix
    private const float pedPointFrontLeftFootfallValue = 13;
    private const float pedPointBuilding3FootfallValue = 20;
    private const float pedPointBuilding5FootfallValue = 22;
    private const float pedPointFrontLeftDistanceValue = 14.81941f;
    private const float pedPointBuilding3DistanceValue = 6.301397f;
    private const float pedPointBuilding5DistanceValue = 3.352163f;

    private const bool isBeneficialFirstNode = true;
    private const bool isBeneficialSecondNode = false;
    private const int minMaxValueFirstNode = 0;
    private const int minMaxValueSecondNode = 1;
    private const float weightingFirstNode = 0.7f;
    private const float weightingSecondNode = 0.3f;

    // Weighted sums
    private const float nodeOneExpectedWeightSum = 0.8578f;
    private const float nodeTwoExpectedWeightSum = 1.6800f;
    private const float nodeThreeExpectedWeightSum = 2.6640f;

    // Normalisation
    private const float valueToNormalise = 20.346f;
    private const float valueToAdjustBy = 5.2f;

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        yield return null;
    }

    [UnityTest]
    public IEnumerator PedestrianPathCreator_GetAllPedestrianPointsInRadius_ReturnsCorrectCollectionOfPedestrianPoints()
    {
        // Arrange
        yield return null;
        DisableLoops();

        PedestrianPoint[] expectedPedestrianPoints = GetExpectedPedestrianPoints(5).ToArray();
        PedestrianPathCreator pedestrianPathCreator = SetUpPedestrianPathCreator();

        // Act
        PedestrianPoint[] actualPedestrianPoints = pedestrianPathCreator.GetAllPedestrianPointsInRadius(pedestrianPathCreator.transform, radius);

        // Assert
        Assert.AreEqual(actualPedestrianPoints.Length, expectedNumberOfPedestrianPoints);
        Assert.That(actualPedestrianPoints, Is.EquivalentTo(expectedPedestrianPoints));
    }

    [UnityTest]
    public IEnumerator PedestrianPathCreator_CreatePathDecisionMatrix_ReturnsCorrectCollectionOfPathDecisionOptions_AndCorrectDistanceFootfallValues()
    {
        // Arrange
        yield return null;
        DisableLoops();

        List<PedestrianPoint> pedestrianPoints = GetExpectedPedestrianPoints(3);
        PedestrianPathCreator pedestrianPathCreator = SetUpPedestrianPathCreator();
        pedestrianPathCreator.CriteriaMinMaxValues.Clear();

        int expectedNumberOfDecisionOptions = 3;

        // Act
        List<PathDecisionOption> actualDecisionOptions = pedestrianPathCreator.CreatePathDecisionMatrix(pedestrianPoints.ToArray(), pedestrianPathCreator.transform, footfallWeighting, distanceWeighting);

        // Assert
        Assert.AreEqual(actualDecisionOptions.Count, expectedNumberOfDecisionOptions);
        Assert.That(pedestrianPathCreator.CurrentMaximumFootfall, Is.EqualTo(expectedCurrentMaximumFootfall).Within(floatingPointTolerance));
        Assert.That(pedestrianPathCreator.CurrentMinimumDistance, Is.EqualTo(expectedCurrentMinimumDistance).Within(floatingPointTolerance));
    }

    [UnityTest]
    public IEnumerator PedestrianPathCreator_CalculateRankedShooterAgentPath_ReturnsCorrectPath()
    {
        // Arrange
        yield return null;
        DisableLoops();

        List<PedestrianPoint> expectedPedestrianPoints = GetExpectedPedestrianPoints(5);
        PedestrianPathCreator pedestrianPathCreator = SetUpPedestrianPathCreator();

        PedestrianPoint building5 = expectedPedestrianPoints[2];
        PedestrianPoint building3 = expectedPedestrianPoints[1];
        PedestrianPoint building2 = expectedPedestrianPoints[3];

        // Act
        PedestrianPoint[] actualPedestrianPoints = pedestrianPathCreator.CalculateRankedShooterAgentPath(radius, pedestrianPathCreator.transform, sizeOfPath, footfallWeighting, distanceWeighting);

        // Assert
        Assert.AreEqual(actualPedestrianPoints.Length, sizeOfPath);
        Assert.That(actualPedestrianPoints[0], Is.EqualTo(building5));
        Assert.That(actualPedestrianPoints[1], Is.EqualTo(building3));
        Assert.That(actualPedestrianPoints[2], Is.EqualTo(building2));
    }

    [UnityTest]
    public IEnumerator PedestrianPathCreator_NormaliseValue_ReturnsCorrectValue_WhenIsBeneficialIsTrue()
    {
        // Arrange
        yield return null;
        DisableLoops();

        PedestrianPathCreator pedestrianPathCreator = SetUpPedestrianPathCreator();
        bool isBeneficial = true;
        float expectedNormalisedValue = 3.912f;

        // Act
        float actualNormalisedValue = pedestrianPathCreator.NormaliseValue(valueToNormalise, isBeneficial, valueToAdjustBy);

        // Assert
        Assert.That(actualNormalisedValue, Is.EqualTo(expectedNormalisedValue).Within(floatingPointTolerance));
    }

    [UnityTest]
    public IEnumerator PedestrianPathCreator_NormaliseValue_ReturnsCorrectValue_WhenIsBeneficialIsFalse()
    {
        // Arrange
        yield return null;
        DisableLoops();

        PedestrianPathCreator pedestrianPathCreator = SetUpPedestrianPathCreator();
        bool isBeneficial = false;
        float expectedNormalisedValue = 0.255f;

        // Act
        float actualNormalisedValue = pedestrianPathCreator.NormaliseValue(valueToNormalise, isBeneficial, valueToAdjustBy);

        // Assert
        Assert.That(actualNormalisedValue, Is.EqualTo(expectedNormalisedValue).Within(floatingPointTolerance));
    }

    [UnityTest]
    public IEnumerator PedestrianPathCreator_GetWeightedValueOfNode_ReturnsCorrectValue()
    {
        // Arrange
        yield return null;
        DisableLoops();

        PedestrianPathCreator pedestrianPathCreator = SetUpPedestrianPathCreator();
        PathDecisionOption pathDecisionOption = GetExpectedPathDecisionOptions()[0];
        PathDecisionNode pathDecisionNode = pathDecisionOption.PathDecisionNodes[0];
        float expectedWeightedSumValue = 9.1f;

        // Act
        float actualEightedSumValue = pedestrianPathCreator.GetWeightedValueOfNode(pathDecisionNode);

        // Assert
        Assert.That(actualEightedSumValue, Is.EqualTo(expectedWeightedSumValue).Within(floatingPointTolerance));
    }

    [UnityTest]
    public IEnumerator PedestrianPathCreator_CalculateWeightedSumOfNormalisedPathOptions_ReturnsCorrectValue()
    {
        // Arrange
        yield return null;
        DisableLoops();

        PedestrianPathCreator pedestrianPathCreator = SetUpPedestrianPathCreator();
        List<PathDecisionOption> pathDecisionOptions = GetExpectedPathDecisionOptions();

        // Act
        pedestrianPathCreator.CalculateWeightedSumOfNormalisedPathOptions(pathDecisionOptions);

        // Assert
        Assert.That(pathDecisionOptions[0].WeightedSumOfPathNodes, Is.EqualTo(nodeOneExpectedWeightSum).Within(floatingPointTolerance));
        Assert.That(pathDecisionOptions[1].WeightedSumOfPathNodes, Is.EqualTo(nodeTwoExpectedWeightSum).Within(floatingPointTolerance));
        Assert.That(pathDecisionOptions[2].WeightedSumOfPathNodes, Is.EqualTo(nodeThreeExpectedWeightSum).Within(floatingPointTolerance));
    }

    [UnityTest]
    public IEnumerator PedestrianPathCreator_GetRankedPedestrianPoints_ReturnsCorrectOrderOfPedestrianPoints()
    {
        // Arrange
        yield return null;
        DisableLoops();

        PedestrianPathCreator pedestrianPathCreator = SetUpPedestrianPathCreator();
        List<PathDecisionOption> pathDecisionOptions = GetExpectedPathDecisionOptions();
        pathDecisionOptions[0].WeightedSumOfPathNodes = nodeOneExpectedWeightSum;
        pathDecisionOptions[1].WeightedSumOfPathNodes = nodeTwoExpectedWeightSum;
        pathDecisionOptions[2].WeightedSumOfPathNodes = nodeThreeExpectedWeightSum;

        List <PedestrianPoint> pedestrianPoints = GetExpectedPedestrianPoints(3);
        int sizeOfPath = 3;

        // Act
        PedestrianPoint[] orderedPedestrianPoints = pedestrianPathCreator.GetRankedPedestrianPoints(pathDecisionOptions, sizeOfPath);

        // Assert
        Assert.AreEqual(orderedPedestrianPoints.Length, sizeOfPath);
        Assert.That(orderedPedestrianPoints[0], Is.EqualTo(pedestrianPoints[2]));
        Assert.That(orderedPedestrianPoints[1], Is.EqualTo(pedestrianPoints[1]));
        Assert.That(orderedPedestrianPoints[2], Is.EqualTo(pedestrianPoints[0]));
    }

    private List<PedestrianPoint> GetExpectedPedestrianPoints(int numberOfPoints)
    {
        List<PedestrianPoint> pedestrianPoints = new List<PedestrianPoint>()
        {
            GetSinglePedestrianPoint("front left"),
            GetSinglePedestrianPoint("building 3"),
            GetSinglePedestrianPoint("building 5"),
            GetSinglePedestrianPoint("building 2"),
            GetSinglePedestrianPoint("front right")
        };

        pedestrianPoints.RemoveRange(numberOfPoints, pedestrianPoints.Count - numberOfPoints);

        return pedestrianPoints;
    }

    private PedestrianPoint GetSinglePedestrianPoint(string name)
    {
        return GameObject.Find($"{pedestrianPointPrefix} {name}").GetComponent<PedestrianPoint>();
    }

    private Pedestrian SetUpPedestrian()
    {
        Pedestrian pedestrian = base.SpawnPedestrians(1)[0];
        pedestrian.gameObject.AddComponent<PedestrianPathCreator>();
        PedestrianPathCreator pedestrianPathCreator = pedestrian.GetComponent<PedestrianPathCreator>();
        pedestrianPathCreator.CurrentMaximumFootfall = 0f;
        pedestrianPathCreator.CurrentMinimumDistance = float.MaxValue;
        pedestrian.transform.position = new Vector3(0, 0, 0);

        return pedestrian;
    }

    private List<PathDecisionOption> GetExpectedPathDecisionOptions()
    {
        List<PedestrianPoint> pedestrianPoints = GetExpectedPedestrianPoints(3);

        return new List<PathDecisionOption>
        {
            // Front left node
            CreatePathDecisionOption(

                pedestrianPoints[0],

                // Footfall node
                CreatePathDecisionNode(pedPointFrontLeftFootfallValue, isBeneficialFirstNode, minMaxValueFirstNode, weightingFirstNode),

                // Distance node
                CreatePathDecisionNode(pedPointFrontLeftDistanceValue, isBeneficialSecondNode, minMaxValueSecondNode, weightingSecondNode)),

            // Building 3 node
            CreatePathDecisionOption(

                pedestrianPoints[1],

                // Footfall Node
                CreatePathDecisionNode(pedPointBuilding3FootfallValue, isBeneficialFirstNode, minMaxValueFirstNode, weightingFirstNode),

                // Distance Node
                CreatePathDecisionNode(pedPointBuilding3DistanceValue, isBeneficialSecondNode, minMaxValueSecondNode, weightingSecondNode)),

            // Building 5 node
            CreatePathDecisionOption(

                pedestrianPoints[2],

                // Footfall Node
                CreatePathDecisionNode(pedPointBuilding5FootfallValue, isBeneficialFirstNode, minMaxValueFirstNode, weightingFirstNode),

                // Distance Node
                CreatePathDecisionNode(pedPointBuilding5DistanceValue, isBeneficialSecondNode, minMaxValueSecondNode, weightingSecondNode))
        };
    }

    private PathDecisionOption CreatePathDecisionOption(PedestrianPoint pedestrianPoint, PathDecisionNode pathDecisionNodeOne, PathDecisionNode pathDecisionNodeTwo)
    {
        return new PathDecisionOption()
        {
            PedestrianPoint = pedestrianPoint,
            PathDecisionNodes = new List<PathDecisionNode>()
            {
                pathDecisionNodeOne,
                pathDecisionNodeTwo
            }
        };
    }

    private PathDecisionNode CreatePathDecisionNode(float decisionValue, bool isDecisionNodeBeneficial, int minMaxIndex, float nodeWeighting)
    {
        return new PathDecisionNode()
        {
            DecisionNodeValue = decisionValue,
            IsDecisionNodeBeneficial = isDecisionNodeBeneficial,
            MinMaxValueIndex = minMaxIndex,
            NodeWeighting = nodeWeighting
        };
    }

    private PedestrianPathCreator SetUpPedestrianPathCreator()
    {
        Pedestrian pedestrian = SetUpPedestrian();
        PedestrianPathCreator pedestrianPathCreator = pedestrian.GetComponent<PedestrianPathCreator>();
        pedestrianPathCreator.CriteriaMinMaxValues = new Dictionary<int, float>
        {
            { 0, expectedCurrentMaximumFootfall },
            { 1, expectedCurrentMinimumDistance }
        };

        return pedestrianPathCreator;
    }
}
