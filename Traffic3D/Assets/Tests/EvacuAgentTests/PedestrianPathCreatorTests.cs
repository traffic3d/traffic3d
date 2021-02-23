using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Tests;
using UnityEngine;
using UnityEngine.TestTools;

public class PedestrianPathCreator_GetAllPedestrianPointsInRadius_ReturnsCorrectCollectionOfPedestrianPoints : ArrangeActAssertStrategy
{
    private PedestrianPoint[] expectedPedestrianPoints;
    private PedestrianPathCreator pedestrianPathCreator;
    private PedestrianPoint[] actualPedestrianPoints;
    private float radius;
    private int expectedNumberOfPedestrianPoints;

    [UnityTest]
    public override IEnumerator PerformTest()
    {
        yield return null;
        Arrange();
        Act();
        Assertion();
    }

    public override void Arrange()
    {
        DisableLoops();
        expectedPedestrianPoints = PedestrianPathCreatorTestsHelper.GetExpectedPedestrianPoints(5).ToArray();
        pedestrianPathCreator = PedestrianPathCreatorTestsHelper.SetUpPedestrianPathCreator();
        radius = 45f;
        expectedNumberOfPedestrianPoints = 5;
    }

    public override void Act()
    {
        actualPedestrianPoints = pedestrianPathCreator.GetAllPedestrianPointsInRadius(pedestrianPathCreator.transform, radius);
    }

    public override IEnumerator Assertion()
    {
        yield return null;
        Assert.AreEqual(actualPedestrianPoints.Length, expectedNumberOfPedestrianPoints);
        Assert.That(actualPedestrianPoints, Is.EquivalentTo(expectedPedestrianPoints));
    }
}

public class PedestrianPathCreator_CreatePathDecisionMatrix_ReturnsCorrectCollectionOfPathDecisionOptions_AndCorrectDistanceFootfallValues : ArrangeActAssertStrategy
{
    private List<PedestrianPoint> pedestrianPoints;
    private PedestrianPathCreator pedestrianPathCreator;
    private List<PathDecisionOption> actualDecisionOptions;
    private float footfallWeighting;
    private float distanceWeighting;
    private int expectedNumberOfDecisionOptions;
    private float expectedCurrentMaximumFootfall;
    private float expectedCurrentMinimumDistance;

    [UnityTest]
    public override IEnumerator PerformTest()
    {
        yield return null;
        Arrange();
        Act();
        Assertion();
    }

    public override void Arrange()
    {
        DisableLoops();
        pedestrianPoints = PedestrianPathCreatorTestsHelper.GetExpectedPedestrianPoints(3);
        pedestrianPathCreator = PedestrianPathCreatorTestsHelper.SetUpPedestrianPathCreator();
        pedestrianPathCreator.CriteriaMinMaxValues.Clear();

        expectedNumberOfDecisionOptions = 3;
        footfallWeighting = 0.7f;
        distanceWeighting = 0.3f;
        expectedCurrentMaximumFootfall = 22f;
        expectedCurrentMinimumDistance = 21.946f;
    }

    public override void Act()
    {
        actualDecisionOptions = pedestrianPathCreator.CreatePathDecisionMatrix(pedestrianPoints.ToArray(), pedestrianPathCreator.transform, footfallWeighting, distanceWeighting);
    }

    public override IEnumerator Assertion()
    {
        yield return null;
        Assert.AreEqual(actualDecisionOptions.Count, expectedNumberOfDecisionOptions);
        Assert.That(pedestrianPathCreator.CriteriaMinMaxValues[0], Is.EqualTo(expectedCurrentMaximumFootfall).Within(floatingPointTolerance));
        Assert.That(pedestrianPathCreator.CriteriaMinMaxValues[1], Is.EqualTo(expectedCurrentMinimumDistance).Within(floatingPointTolerance));
    }
}

public class PedestrianPathCreator_CalculateRankedShooterAgentPath_ReturnsCorrectPath : ArrangeActAssertStrategy
{
    private List<PedestrianPoint> expectedPedestrianPoints;
    private PedestrianPathCreator pedestrianPathCreator;
    private PedestrianPoint[] actualPedestrianPoints;
    private PedestrianPoint building5;
    private PedestrianPoint building3;
    private PedestrianPoint building2;
    private float radius;
    private float footfallWeighting;
    private float distanceWeighting;
    private int sizeOfPath;

    [UnityTest]
    public override IEnumerator PerformTest()
    {
        yield return null;
        Arrange();
        Act();
        Assertion();
    }

    public override void Arrange()
    {
        DisableLoops();
        expectedPedestrianPoints = PedestrianPathCreatorTestsHelper.GetExpectedPedestrianPoints(5);
        pedestrianPathCreator = PedestrianPathCreatorTestsHelper.SetUpPedestrianPathCreator();

        radius = 45f;
        footfallWeighting = 0.7f;
        distanceWeighting = 0.3f;
        sizeOfPath = 3;

        building5 = expectedPedestrianPoints[2];
        building3 = expectedPedestrianPoints[1];
        building2 = expectedPedestrianPoints[3];
    }

    public override void Act()
    {
        actualPedestrianPoints = pedestrianPathCreator.CalculateRankedShooterAgentPath(radius, pedestrianPathCreator.transform, sizeOfPath, footfallWeighting, distanceWeighting);
    }

    public override IEnumerator Assertion()
    {
        yield return null;
        Assert.AreEqual(actualPedestrianPoints.Length, sizeOfPath);
        Assert.That(actualPedestrianPoints[0], Is.EqualTo(building5));
        Assert.That(actualPedestrianPoints[1], Is.EqualTo(building3));
        Assert.That(actualPedestrianPoints[2], Is.EqualTo(building2));
    }
}

public class PedestrianPathCreator_NormaliseValue_ReturnsCorrectValue_WhenIsBeneficialIsTrue : ArrangeActAssertStrategy
{
    private PedestrianPathCreator pedestrianPathCreator;
    private bool isBeneficial;
    private float expectedNormalisedValue;
    private float actualNormalisedValue;
    private float valueToNormalise;
    private float valueToAdjustBy;

    [UnityTest]
    public override IEnumerator PerformTest()
    {
        yield return null;
        Arrange();
        Act();
        Assertion();
    }

    public override void Arrange()
    {
        DisableLoops();
        pedestrianPathCreator = PedestrianPathCreatorTestsHelper.SetUpPedestrianPathCreator();
        isBeneficial = true;
        expectedNormalisedValue = 3.912f;
        valueToNormalise = 20.346f;
        valueToAdjustBy = 5.2f;
    }

    public override void Act()
    {
        actualNormalisedValue = pedestrianPathCreator.NormaliseValue(valueToNormalise, isBeneficial, valueToAdjustBy);
    }

    public override IEnumerator Assertion()
    {
        yield return null;
        Assert.That(actualNormalisedValue, Is.EqualTo(expectedNormalisedValue).Within(floatingPointTolerance));
    }
}

public class PedestrianPathCreator_NormaliseValue_ReturnsCorrectValue_WhenIsBeneficialIsFalse : ArrangeActAssertStrategy
{
    private PedestrianPathCreator pedestrianPathCreator;
    private bool isBeneficial;
    private float expectedNormalisedValue;
    private float actualNormalisedValue;
    private float valueToNormalise;
    private float valueToAdjustBy;

    [UnityTest]
    public override IEnumerator PerformTest()
    {
        yield return null;
        Arrange();
        Act();
        Assertion();
    }

    public override void Arrange()
    {
        DisableLoops();
        pedestrianPathCreator = PedestrianPathCreatorTestsHelper.SetUpPedestrianPathCreator();
        isBeneficial = false;
        expectedNormalisedValue = 0.255f;
        valueToNormalise = 20.346f;
        valueToAdjustBy = 5.2f;
    }

    public override void Act()
    {
        actualNormalisedValue = pedestrianPathCreator.NormaliseValue(valueToNormalise, isBeneficial, valueToAdjustBy);
    }

    public override IEnumerator Assertion()
    {
        yield return null;
        Assert.That(actualNormalisedValue, Is.EqualTo(expectedNormalisedValue).Within(floatingPointTolerance));
    }
}

public class PedestrianPathCreator_GetWeightedValueOfNode_ReturnsCorrectValue : ArrangeActAssertStrategy
{
    private PedestrianPathCreator pedestrianPathCreator;
    private PathDecisionOption pathDecisionOption;
    private PathDecisionNode pathDecisionNode;
    private float expectedWeightedSumValue;
    private float actualEightedSumValue;

    [UnityTest]
    public override IEnumerator PerformTest()
    {
        yield return null;
        Arrange();
        Act();
        Assertion();
    }

    public override void Arrange()
    {
        pedestrianPathCreator = PedestrianPathCreatorTestsHelper.SetUpPedestrianPathCreator();
        pathDecisionOption = PedestrianPathCreatorTestsHelper.GetExpectedPathDecisionOptions()[0];
        pathDecisionNode = pathDecisionOption.PathDecisionNodes[0];
        expectedWeightedSumValue = 9.1f;
    }

    public override void Act()
    {
        actualEightedSumValue = pedestrianPathCreator.GetWeightedValueOfNode(pathDecisionNode);
    }

    public override IEnumerator Assertion()
    {
        yield return null;
        Assert.That(actualEightedSumValue, Is.EqualTo(expectedWeightedSumValue).Within(floatingPointTolerance));
    }
}

public class PedestrianPathCreator_CalculateWeightedSumOfNormalisedPathOptions_ReturnsCorrectValue : ArrangeActAssertStrategy
{
    private PedestrianPathCreator pedestrianPathCreator;
    private List<PathDecisionOption> pathDecisionOptions;
    private float nodeOneExpectedWeightSum;
    private float nodeTwoExpectedWeightSum;
    private float nodeThreeExpectedWeightSum;

    [UnityTest]
    public override IEnumerator PerformTest()
    {
        yield return null;
        Arrange();
        Act();
        Assertion();
    }

    public override void Arrange()
    {
        pedestrianPathCreator = PedestrianPathCreatorTestsHelper.SetUpPedestrianPathCreator();
        pathDecisionOptions = PedestrianPathCreatorTestsHelper.GetExpectedPathDecisionOptions();
        nodeOneExpectedWeightSum = 0.8578f;
        nodeTwoExpectedWeightSum = 1.6800f;
        nodeThreeExpectedWeightSum = 2.6640f;
    }

    public override void Act()
    {
        pedestrianPathCreator.CalculateWeightedSumOfNormalisedPathOptions(pathDecisionOptions);
    }

    public override IEnumerator Assertion()
    {
        yield return null;
        Assert.That(pathDecisionOptions[0].WeightedSumOfPathNodes, Is.EqualTo(nodeOneExpectedWeightSum).Within(floatingPointTolerance));
        Assert.That(pathDecisionOptions[1].WeightedSumOfPathNodes, Is.EqualTo(nodeTwoExpectedWeightSum).Within(floatingPointTolerance));
        Assert.That(pathDecisionOptions[2].WeightedSumOfPathNodes, Is.EqualTo(nodeThreeExpectedWeightSum).Within(floatingPointTolerance));
    }
}

public class PedestrianPathCreator_GetRankedPedestrianPoints_ReturnsCorrectOrderOfPedestrianPoints : ArrangeActAssertStrategy
{
    private PedestrianPathCreator pedestrianPathCreator;
    private List<PathDecisionOption> pathDecisionOptions;
    private List<PedestrianPoint> pedestrianPoints;
    private PedestrianPoint[] orderedPedestrianPoints;
    private int sizeOfPath;
    private float nodeOneExpectedWeightSum;
    private float nodeTwoExpectedWeightSum;
    private float nodeThreeExpectedWeightSum;

    [UnityTest]
    public override IEnumerator PerformTest()
    {
        yield return null;
        Arrange();
        Act();
        Assertion();
    }

    public override void Arrange()
    {
        pedestrianPathCreator = PedestrianPathCreatorTestsHelper.SetUpPedestrianPathCreator();
        pathDecisionOptions = PedestrianPathCreatorTestsHelper.GetExpectedPathDecisionOptions();

        nodeOneExpectedWeightSum = 0.8578f;
        nodeTwoExpectedWeightSum = 1.6800f;
        nodeThreeExpectedWeightSum = 2.6640f;

        pathDecisionOptions[0].WeightedSumOfPathNodes = nodeOneExpectedWeightSum;
        pathDecisionOptions[1].WeightedSumOfPathNodes = nodeTwoExpectedWeightSum;
        pathDecisionOptions[2].WeightedSumOfPathNodes = nodeThreeExpectedWeightSum;

        pedestrianPoints = PedestrianPathCreatorTestsHelper.GetExpectedPedestrianPoints(3);
        sizeOfPath = 3;
    }

    public override void Act()
    {
        orderedPedestrianPoints = pedestrianPathCreator.GetRankedPedestrianPoints(pathDecisionOptions, sizeOfPath);
    }

    public override IEnumerator Assertion()
    {
        yield return null;
        Assert.AreEqual(orderedPedestrianPoints.Length, sizeOfPath);
        Assert.That(orderedPedestrianPoints[0], Is.EqualTo(pedestrianPoints[2]));
        Assert.That(orderedPedestrianPoints[1], Is.EqualTo(pedestrianPoints[1]));
        Assert.That(orderedPedestrianPoints[2], Is.EqualTo(pedestrianPoints[0]));
    }
}

public static class PedestrianPathCreatorTestsHelper
{
    // Expected values
    private static readonly float expectedCurrentMaximumFootfall = 22f;
    private static readonly float expectedCurrentMinimumDistance = 21.946f;

    // Pedestrian points
    private static readonly string pedestrianPointPrefix = "PedestrianFactory/PedestrianPoint";

    // Decision matrix
    private static readonly float pedPointFrontLeftFootfallValue = 13;
    private static readonly float pedPointBuilding3FootfallValue = 20;
    private static readonly float pedPointBuilding5FootfallValue = 22;
    private static readonly float pedPointFrontLeftDistanceValue = 14.81941f;
    private static readonly float pedPointBuilding3DistanceValue = 6.301397f;
    private static readonly float pedPointBuilding5DistanceValue = 3.352163f;

    private static readonly bool isBeneficialFootfall = true;
    private static readonly bool isBeneficialDistance = false;
    private static readonly int minMaxIndexFootfall = 0;
    private static readonly int minMaxIndexDistance = 1;
    private static readonly float weightingFootfall = 0.7f;
    private static readonly float weightingDistance = 0.3f;

    public static List<PedestrianPoint> GetExpectedPedestrianPoints(int numberOfPoints)
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

    public static PedestrianPoint GetSinglePedestrianPoint(string name)
    {
        return GameObject.Find($"{pedestrianPointPrefix} {name}").GetComponent<PedestrianPoint>();
    }

    public static List<PathDecisionOption> GetExpectedPathDecisionOptions()
    {
        List<PedestrianPoint> pedestrianPoints = GetExpectedPedestrianPoints(3);

        return new List<PathDecisionOption>
            {
                // Front left node
                CreatePathDecisionOption(

                    pedestrianPoints[0],

                    // Footfall node
                    CreatePathDecisionNode(pedPointFrontLeftFootfallValue, isBeneficialFootfall, minMaxIndexFootfall, weightingFootfall),

                    // Distance node
                    CreatePathDecisionNode(pedPointFrontLeftDistanceValue, isBeneficialDistance, minMaxIndexDistance, weightingDistance)),

                // Building 3 node
                CreatePathDecisionOption(

                    pedestrianPoints[1],

                    // Footfall Node
                    CreatePathDecisionNode(pedPointBuilding3FootfallValue, isBeneficialFootfall, minMaxIndexFootfall, weightingFootfall),

                    // Distance Node
                    CreatePathDecisionNode(pedPointBuilding3DistanceValue, isBeneficialDistance, minMaxIndexDistance, weightingDistance)),

                // Building 5 node
                CreatePathDecisionOption(

                    pedestrianPoints[2],

                    // Footfall Node
                    CreatePathDecisionNode(pedPointBuilding5FootfallValue, isBeneficialFootfall, minMaxIndexFootfall, weightingFootfall),

                    // Distance Node
                    CreatePathDecisionNode(pedPointBuilding5DistanceValue, isBeneficialDistance, minMaxIndexDistance, weightingDistance))
        };
    }

    public static PathDecisionOption CreatePathDecisionOption(PedestrianPoint pedestrianPoint, PathDecisionNode pathDecisionNodeOne, PathDecisionNode pathDecisionNodeTwo)
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

    public static PathDecisionNode CreatePathDecisionNode(float decisionValue, bool isDecisionNodeBeneficial, int minMaxIndex, float nodeWeighting)
    {
        return new PathDecisionNode()
        {
            DecisionNodeValue = decisionValue,
            IsDecisionNodeBeneficial = isDecisionNodeBeneficial,
            MinMaxValueIndex = minMaxIndex,
            NodeWeighting = nodeWeighting
        };
    }

    public static Pedestrian SetUpPedestrian()
    {
        Pedestrian pedestrian = EvacuAgentCommonSceneTest.SpawnPedestrians(1)[0];
        pedestrian.gameObject.AddComponent<PedestrianPathCreator>();
        PedestrianPathCreator pedestrianPathCreator = pedestrian.GetComponent<PedestrianPathCreator>();
        pedestrian.transform.position = new Vector3(0, 0, 0);

        return pedestrian;
    }

    public static PedestrianPathCreator SetUpPedestrianPathCreator()
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
