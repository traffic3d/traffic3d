using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


public class WorkerPedestrianPointPathCreator_ReturnsCorrectPath_WithHospitalityPoint : ArrangeActAssertStrategy
{
    private WorkerPedestrianPointPathCreator pedestrianPointPathCreator;
    private List<Vector3> actualPedestrianPoints;
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
        pedestrianPointPathCreator = PedestrianPointPathCreatorTestsHelper.SetUpWorkerPedestrianPointPathCreator();
        hopsitalityChanceOriginalValue = EvacuAgentSceneParamaters.WORKER_CHANCE_TO_VISIT_HOSPITALITY_POINT_IN_ROUTE;
        EvacuAgentSceneParamaters.WORKER_CHANCE_TO_VISIT_HOSPITALITY_POINT_IN_ROUTE = 1f;
        expectedNumberOfElements = 2;
    }

    public override void Act()
    {
        actualPedestrianPoints = pedestrianPointPathCreator.CreatePath();
    }

    public override void Assertion()
    {
        Assert.AreEqual(expectedNumberOfElements, actualPedestrianPoints.Count);
        Assert.AreEqual(PedestrianPointType.Hospitality, GetPedestrianPointFromLocation(actualPedestrianPoints[0]).pedestrianPointType);
        Assert.AreEqual(PedestrianPointType.Work, GetPedestrianPointFromLocation(actualPedestrianPoints[1]).pedestrianPointType);
    }

    [TearDown]
    public void TearDown()
    {
        EvacuAgentSceneParamaters.WORKER_CHANCE_TO_VISIT_HOSPITALITY_POINT_IN_ROUTE = hopsitalityChanceOriginalValue;
    }
}


public class WorkerPedestrianPointPathCreator_ReturnsCorrectPath_WithoutHospitalityPoint : ArrangeActAssertStrategy
{
    private WorkerPedestrianPointPathCreator pedestrianPointPathCreator;
    private List<Vector3> actualPedestrianPoints;
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
        hopsitalityChanceOriginalValue = EvacuAgentSceneParamaters.WORKER_CHANCE_TO_VISIT_HOSPITALITY_POINT_IN_ROUTE;
        EvacuAgentSceneParamaters.WORKER_CHANCE_TO_VISIT_HOSPITALITY_POINT_IN_ROUTE = 0f;
        expectedNumberOfElements = 1;
        pedestrianPointPathCreator = PedestrianPointPathCreatorTestsHelper.SetUpWorkerPedestrianPointPathCreator();
    }

    public override void Act()
    {
        actualPedestrianPoints = pedestrianPointPathCreator.CreatePath();
    }

    public override void Assertion()
    {
        Assert.AreEqual(expectedNumberOfElements, actualPedestrianPoints.Count);
        Assert.AreEqual(PedestrianPointType.Work, GetPedestrianPointFromLocation(actualPedestrianPoints[0]).pedestrianPointType);
    }

    [TearDown]
    public void TearDown()
    {
        EvacuAgentSceneParamaters.WORKER_CHANCE_TO_VISIT_HOSPITALITY_POINT_IN_ROUTE = hopsitalityChanceOriginalValue;
    }
}

public static class PedestrianPointPathCreatorTestsHelper
{
    public static WorkerPedestrianPointPathCreator SetUpWorkerPedestrianPointPathCreator()
    {
        GameObject gameObject = GameObject.Instantiate(new GameObject());
        return gameObject.AddComponent<WorkerPedestrianPointPathCreator>();
    }
}
