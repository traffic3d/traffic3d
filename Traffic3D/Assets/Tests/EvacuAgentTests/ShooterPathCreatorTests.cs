using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Tests;
using UnityEngine;
using UnityEngine.TestTools;

public class ShooterPathCreator_GetAllPedestrianPointsInRadius_ReturnsCorrectCollectionOfPedestrianPoints : ArrangeActAssertStrategy
{
    private List<PedestrianPoint> expectedPedestrianPoints;
    private ShooterPedestrianPointPathCreator shooterPathCreator;
    private List<PedestrianPoint> actualPedestrianPoints;
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
        //DisableLoops();
        expectedPedestrianPoints = ShooterPathCreatorTestsHelper.GetExpectedPedestrianPoints(3);
        shooterPathCreator = ShooterPathCreatorTestsHelper.SetUpshooterPathCreator();
        radius = 35;
        expectedNumberOfPedestrianPoints = 3;
    }

    public override void Act()
    {
        actualPedestrianPoints = shooterPathCreator.GetAllPedestrianPointsInRadius(shooterPathCreator.transform, radius);
    }

    public override void Assertion()
    {
        Assert.AreEqual(expectedNumberOfPedestrianPoints, actualPedestrianPoints.Count);
        Assert.That(actualPedestrianPoints, Is.EquivalentTo(expectedPedestrianPoints));
    }
}

public class ShooterPathCreator_CreatePathDecisionMatrix_ReturnsCorrectCollectionOfPathDecisionOptions_AndCorrectDistanceFootfallValues : ArrangeActAssertStrategy
{
    private List<PedestrianPoint> pedestrianPoints;
    private ShooterPedestrianPointPathCreator shooterPathCreator;
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
        pedestrianPoints = ShooterPathCreatorTestsHelper.GetExpectedPedestrianPoints(3);
        shooterPathCreator = ShooterPathCreatorTestsHelper.SetUpshooterPathCreator();
        shooterPathCreator.CriteriaMinMaxValues.Clear();

        expectedNumberOfDecisionOptions = 3;
        footfallWeighting = 0.7f;
        distanceWeighting = 0.3f;
        expectedCurrentMaximumFootfall = 22f;
        expectedCurrentMinimumDistance = 28.619f;
    }

    public override void Act()
    {
        actualDecisionOptions = shooterPathCreator.CreatePathDecisionMatrix(pedestrianPoints, shooterPathCreator.transform, footfallWeighting, distanceWeighting);
    }

    public override void Assertion()
    {
        Assert.AreEqual(actualDecisionOptions.Count, expectedNumberOfDecisionOptions);
        Assert.That(shooterPathCreator.CriteriaMinMaxValues[0], Is.EqualTo(expectedCurrentMaximumFootfall).Within(floatingPointTolerance));
        Assert.That(shooterPathCreator.CriteriaMinMaxValues[1], Is.EqualTo(expectedCurrentMinimumDistance).Within(floatingPointTolerance));
    }
}

public class ShooterPathCreator_CalculateRankedShooterAgentPath_ReturnsCorrectPath : ArrangeActAssertStrategy
{
    private List<PedestrianPoint> expectedPedestrianPoints;
    private ShooterPedestrianPointPathCreator shooterPathCreator;
    private List<Vector3> actualPedestrianPoints;
    private PedestrianPoint ShoppingBuilding2;
    private PedestrianPoint ShoppingBuilding1;
    private PedestrianPoint RecreationBuilding1;
    private List<Vector3> expectedLocations;
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
        expectedPedestrianPoints = ShooterPathCreatorTestsHelper.GetExpectedPedestrianPoints(6);
        shooterPathCreator = ShooterPathCreatorTestsHelper.SetUpshooterPathCreator();
        sizeOfPath = 3;

        ShoppingBuilding1 = expectedPedestrianPoints[0];
        ShoppingBuilding2 = expectedPedestrianPoints[1];
        RecreationBuilding1 = expectedPedestrianPoints[2];

        expectedLocations = new List<Vector3>()
        {
            ShoppingBuilding1.GetPointLocation(),
            RecreationBuilding1.GetPointLocation(),
            ShoppingBuilding2.GetPointLocation()
        };
    }

    public override void Act()
    {
        actualPedestrianPoints = shooterPathCreator.CreatePath();
    }

    public override void Assertion()
    {
        Assert.AreEqual(actualPedestrianPoints.Count, sizeOfPath);
        Assert.That(actualPedestrianPoints[0], Is.EqualTo(expectedLocations[0]));
        Assert.That(actualPedestrianPoints[1], Is.EqualTo(expectedLocations[1]));
        Assert.That(actualPedestrianPoints[2], Is.EqualTo(expectedLocations[2]));
    }
}

public class ShooterPathCreator_NormaliseValue_ReturnsCorrectValue_WhenIsBeneficialIsTrue : ArrangeActAssertStrategy
{
    private ShooterPedestrianPointPathCreator shooterPathCreator;
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
        shooterPathCreator = ShooterPathCreatorTestsHelper.SetUpshooterPathCreator();
        isBeneficial = true;
        expectedNormalisedValue = 3.912f;
        valueToNormalise = 20.346f;
        valueToAdjustBy = 5.2f;
    }

    public override void Act()
    {
        actualNormalisedValue = shooterPathCreator.NormaliseValue(valueToNormalise, isBeneficial, valueToAdjustBy);
    }

    public override void Assertion()
    {
        Assert.That(actualNormalisedValue, Is.EqualTo(expectedNormalisedValue).Within(floatingPointTolerance));
    }
}

public class ShooterPathCreator_NormaliseValue_ReturnsCorrectValue_WhenIsBeneficialIsFalse : ArrangeActAssertStrategy
{
    private ShooterPedestrianPointPathCreator shooterPathCreator;
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
        shooterPathCreator = ShooterPathCreatorTestsHelper.SetUpshooterPathCreator();
        isBeneficial = false;
        expectedNormalisedValue = 0.255f;
        valueToNormalise = 20.346f;
        valueToAdjustBy = 5.2f;
    }

    public override void Act()
    {
        actualNormalisedValue = shooterPathCreator.NormaliseValue(valueToNormalise, isBeneficial, valueToAdjustBy);
    }

    public override void Assertion()
    {
        Assert.That(actualNormalisedValue, Is.EqualTo(expectedNormalisedValue).Within(floatingPointTolerance));
    }
}

public class ShooterPathCreator_GetWeightedValueOfNode_ReturnsCorrectValue : ArrangeActAssertStrategy
{
    private ShooterPedestrianPointPathCreator shooterPathCreator;
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
        shooterPathCreator = ShooterPathCreatorTestsHelper.SetUpshooterPathCreator();
        pathDecisionOption = ShooterPathCreatorTestsHelper.GetExpectedPathDecisionOptions()[0];
        pathDecisionNode = pathDecisionOption.PathDecisionNodes[0];
        expectedWeightedSumValue = 9.1f;
    }

    public override void Act()
    {
        actualEightedSumValue = shooterPathCreator.GetWeightedValueOfNode(pathDecisionNode);
    }

    public override void Assertion()
    {
        Assert.That(actualEightedSumValue, Is.EqualTo(expectedWeightedSumValue).Within(floatingPointTolerance));
    }
}

public class ShooterPathCreator_CalculateWeightedSumOfNormalisedPathOptions_ReturnsCorrectValue : ArrangeActAssertStrategy
{
    private ShooterPedestrianPointPathCreator shooterPathCreator;
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
        shooterPathCreator = ShooterPathCreatorTestsHelper.SetUpshooterPathCreator();
        pathDecisionOptions = ShooterPathCreatorTestsHelper.GetExpectedPathDecisionOptions();
        nodeOneExpectedWeightSum = 0.8578f;
        nodeTwoExpectedWeightSum = 1.6800f;
        nodeThreeExpectedWeightSum = 2.6640f;
    }

    public override void Act()
    {
        shooterPathCreator.CalculateWeightedSumOfNormalisedPathOptions(pathDecisionOptions);
    }

    public override void Assertion()
    {
        Assert.That(pathDecisionOptions[0].WeightedSumOfPathNodes, Is.EqualTo(nodeOneExpectedWeightSum).Within(floatingPointTolerance));
        Assert.That(pathDecisionOptions[1].WeightedSumOfPathNodes, Is.EqualTo(nodeTwoExpectedWeightSum).Within(floatingPointTolerance));
        Assert.That(pathDecisionOptions[2].WeightedSumOfPathNodes, Is.EqualTo(nodeThreeExpectedWeightSum).Within(floatingPointTolerance));
    }
}

public class ShooterPathCreator_GetRankedPedestrianPoints_ReturnsCorrectOrderOfPedestrianPoints : ArrangeActAssertStrategy
{
    private ShooterPedestrianPointPathCreator shooterPathCreator;
    private List<PathDecisionOption> pathDecisionOptions;
    private List<PedestrianPoint> pedestrianPoints;
    private List<Vector3> orderedPedestrianPoints;
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
        shooterPathCreator = ShooterPathCreatorTestsHelper.SetUpshooterPathCreator();
        pathDecisionOptions = ShooterPathCreatorTestsHelper.GetExpectedPathDecisionOptions();

        nodeOneExpectedWeightSum = 0.8578f;
        nodeTwoExpectedWeightSum = 1.6800f;
        nodeThreeExpectedWeightSum = 2.6640f;

        pathDecisionOptions[0].WeightedSumOfPathNodes = nodeOneExpectedWeightSum;
        pathDecisionOptions[1].WeightedSumOfPathNodes = nodeTwoExpectedWeightSum;
        pathDecisionOptions[2].WeightedSumOfPathNodes = nodeThreeExpectedWeightSum;

        pedestrianPoints = ShooterPathCreatorTestsHelper.GetExpectedPedestrianPoints(3);
        sizeOfPath = 3;
    }

    public override void Act()
    {
        orderedPedestrianPoints = shooterPathCreator.GetRankedPedestrianPoints(pathDecisionOptions, sizeOfPath);
    }

    public override void Assertion()
    {
        Assert.AreEqual(orderedPedestrianPoints.Count, sizeOfPath);
        Assert.That(GetPedestrianPointFromLocation(orderedPedestrianPoints[0]), Is.EqualTo(pedestrianPoints[2]));
        Assert.That(GetPedestrianPointFromLocation(orderedPedestrianPoints[1]), Is.EqualTo(pedestrianPoints[1]));
        Assert.That(GetPedestrianPointFromLocation(orderedPedestrianPoints[2]), Is.EqualTo(pedestrianPoints[0]));
    }
}

public static class ShooterPathCreatorTestsHelper
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
            GetSinglePedestrianPoint("ShoppingBuilding1"),
            GetSinglePedestrianPoint("ShoppingBuilding2"),
            GetSinglePedestrianPoint("HospitalityBuilding2"),
            GetSinglePedestrianPoint("RecreationBuilding1"),
            GetSinglePedestrianPoint("LandMark1"),
            GetSinglePedestrianPoint("HospitalityBuilding1"),
            GetSinglePedestrianPoint("WorkBuilding1")
        };

        pedestrianPoints.RemoveRange(numberOfPoints, pedestrianPoints.Count - numberOfPoints);

        return pedestrianPoints;
    }

    public static PedestrianPoint GetSinglePedestrianPoint(string name)
    {
        return GameObject.Find($"{name}").GetComponentInChildren<PedestrianPoint>();
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
        pedestrian.gameObject.AddComponent<ShooterPedestrianPointPathCreator>();
        ShooterPedestrianPointPathCreator shooterPathCreator = pedestrian.GetComponent<ShooterPedestrianPointPathCreator>();
        pedestrian.transform.position = new Vector3(0, 0, 0);

        return pedestrian;
    }

    public static ShooterPedestrianPointPathCreator SetUpshooterPathCreator()
    {
        Pedestrian pedestrian = SetUpPedestrian();
        ShooterPedestrianPointPathCreator shooterPathCreator = pedestrian.GetComponent<ShooterPedestrianPointPathCreator>();
        shooterPathCreator.CriteriaMinMaxValues = new Dictionary<int, float>
        {
            { 0, expectedCurrentMaximumFootfall },
            { 1, expectedCurrentMinimumDistance }
        };

        return shooterPathCreator;
    }
}
