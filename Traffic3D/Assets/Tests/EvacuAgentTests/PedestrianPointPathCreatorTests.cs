using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


public class PedestrianPointPathCreator_ReturnsCorrectPath_WithHospitalityPoint_WhenPedestrianTypeIsWorker : ArrangeActAssertStrategy
{
    private PedestrianPointPathCreator pedestrianPointPathCreator;
    private PedestrianType pedestrianType;
    private List<PedestrianPoint> actualPedestrianPoints;
    private float hopsitalityChanceOriginalValue;
    private int expectedNumberOfElements;

    [UnityTest]
    public override IEnumerator PerformTest()
    {
        Arrange();
        yield return null;
        Act();
        Assertion();
    }

    public override void Arrange()
    {
        pedestrianPointPathCreator = PedestrianPointPathCreatorTestsHelper.SetUpPedestrianPointPathCreator();
        pedestrianType = PedestrianType.Worker;
        hopsitalityChanceOriginalValue = EvacuAgentSceneParamaters.WORKER_CHANCE_TO_VISIT_HOSPITALITY_POINT_IN_ROUTE;
        EvacuAgentSceneParamaters.WORKER_CHANCE_TO_VISIT_HOSPITALITY_POINT_IN_ROUTE = 1f;
        expectedNumberOfElements = 2;
    }

    public override void Act()
    {
        actualPedestrianPoints = pedestrianPointPathCreator.GeneratePathBasedOnPedestrianType(pedestrianType);
    }

    public override void Assertion()
    {
        Assert.AreEqual(expectedNumberOfElements, actualPedestrianPoints.Count);
        Assert.AreEqual(PedestrianPointType.Hospitality, actualPedestrianPoints[0].PedestrianPointType);
        Assert.AreEqual(PedestrianPointType.Work, actualPedestrianPoints[1].PedestrianPointType);
    }

    [TearDown]
    public void TearDown()
    {
        EvacuAgentSceneParamaters.WORKER_CHANCE_TO_VISIT_HOSPITALITY_POINT_IN_ROUTE = hopsitalityChanceOriginalValue;
    }
}

public class PedestrianPointPathCreator_ReturnsCorrectPath_WithoutHospitalityPoint_WhenPedestrianTypeIsWorker : ArrangeActAssertStrategy
{
    private PedestrianPointPathCreator pedestrianPointPathCreator;
    private PedestrianType pedestrianType;
    private List<PedestrianPoint> actualPedestrianPoints;
    private float hopsitalityChanceOriginalValue;
    private int expectedNumberOfElements;

    [UnityTest]
    public override IEnumerator PerformTest()
    {
        Arrange();
        yield return null;
        Act();
        Assertion();
    }

    public override void Arrange()
    {
        pedestrianPointPathCreator = PedestrianPointPathCreatorTestsHelper.SetUpPedestrianPointPathCreator();
        pedestrianType = PedestrianType.Worker;
        hopsitalityChanceOriginalValue = EvacuAgentSceneParamaters.WORKER_CHANCE_TO_VISIT_HOSPITALITY_POINT_IN_ROUTE;
        EvacuAgentSceneParamaters.WORKER_CHANCE_TO_VISIT_HOSPITALITY_POINT_IN_ROUTE = 0f;
        expectedNumberOfElements = 1;
    }

    public override void Act()
    {
        actualPedestrianPoints = pedestrianPointPathCreator.GeneratePathBasedOnPedestrianType(pedestrianType);
    }

    public override void Assertion()
    {
        Assert.AreEqual(expectedNumberOfElements, actualPedestrianPoints.Count);
        Assert.AreEqual(PedestrianPointType.Work, actualPedestrianPoints[0].PedestrianPointType);
    }

    [TearDown]
    public void TearDown()
    {
        EvacuAgentSceneParamaters.WORKER_CHANCE_TO_VISIT_HOSPITALITY_POINT_IN_ROUTE = hopsitalityChanceOriginalValue;
    }
}

public class PedestrianPointPathCreator_ReturnsCorrectPath_WhenPedestrianTypeIsShooter : ArrangeActAssertStrategy
{
    private PedestrianPointPathCreator pedestrianPointPathCreator;
    private PedestrianType pedestrianType;
    private List<PedestrianPoint> actualPedestrianPoints;
    private float hopsitalityChanceOriginalValue;
    private int expectedNumberOfElements;

    [UnityTest]
    public override IEnumerator PerformTest()
    {
        Arrange();
        yield return null;
        Act();
        Assertion();
    }

    public override void Arrange()
    {
        pedestrianPointPathCreator = PedestrianPointPathCreatorTestsHelper.SetUpPedestrianPointPathCreator();
        pedestrianType = PedestrianType.Worker;
        hopsitalityChanceOriginalValue = EvacuAgentSceneParamaters.WORKER_CHANCE_TO_VISIT_HOSPITALITY_POINT_IN_ROUTE;
        EvacuAgentSceneParamaters.WORKER_CHANCE_TO_VISIT_HOSPITALITY_POINT_IN_ROUTE = 0f;
        expectedNumberOfElements = 1;
    }

    public override void Act()
    {
        actualPedestrianPoints = pedestrianPointPathCreator.GeneratePathBasedOnPedestrianType(pedestrianType);
    }

    public override void Assertion()
    {
        Assert.AreEqual(expectedNumberOfElements, actualPedestrianPoints.Count);
        Assert.AreEqual(PedestrianPointType.Work, actualPedestrianPoints[0].PedestrianPointType);
    }

    [TearDown]
    public void TearDown()
    {
        EvacuAgentSceneParamaters.WORKER_CHANCE_TO_VISIT_HOSPITALITY_POINT_IN_ROUTE = hopsitalityChanceOriginalValue;
    }
}

public static class PedestrianPointPathCreatorTestsHelper
{
    public static PedestrianPointPathCreator SetUpPedestrianPointPathCreator()
    {
        GameObject gameObject = GameObject.Instantiate(new GameObject());
        return gameObject.AddComponent<PedestrianPointPathCreator>();
    }
}
