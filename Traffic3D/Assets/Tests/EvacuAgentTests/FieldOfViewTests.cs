using NUnit.Framework;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

public class FieldOfView_CanDetectOtherPedestrian_WhenPedestrianIsWithinViewAngleAndViewRadius : ArrangeActAssertStrategy
{
    private Pedestrian[] pedestrians;
    private FieldOfView viewingPedestrianFov;
    private Pedestrian targetPedestrian;
    private Vector3 insideViewAngleAndInsideRadius;

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
        pedestrians = SpawnPedestrians(2);
        viewingPedestrianFov = FieldOfViewTestsHelper.SetUpViewingPedestrian(pedestrians[0]);

        targetPedestrian = pedestrians[1];
        insideViewAngleAndInsideRadius = new Vector3(5, 0, 10);
        targetPedestrian.transform.position = insideViewAngleAndInsideRadius;
    }

    public override void Act()
    {
        viewingPedestrianFov.GetAllAgentsInViewAngle();
    }

    public override IEnumerator Assertion()
    {
        yield return null;
        Assert.AreEqual(1, viewingPedestrianFov.visiblePedestrians.Count);
        Assert.AreSame(viewingPedestrianFov.visiblePedestrians.First(), targetPedestrian);
    }
}

public class FieldOfView_CannnotDetectOtherPedestrian_WhenPedestrianIsWithinViewAngle_AndOutsideOfViewRadius : ArrangeActAssertStrategy
{
    private Pedestrian[] pedestrians;
    private FieldOfView viewingPedestrianFov;
    private Pedestrian targetPedestrian;
    private Vector3 insideViewAngleAndOutsideRadius;

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
        pedestrians = SpawnPedestrians(2);
        viewingPedestrianFov = FieldOfViewTestsHelper.SetUpViewingPedestrian(pedestrians[0]);

        targetPedestrian = pedestrians[1];
        insideViewAngleAndOutsideRadius = new Vector3(0, 0, 150);
        targetPedestrian.transform.position = insideViewAngleAndOutsideRadius;
    }

    public override void Act()
    {
        viewingPedestrianFov.GetAllAgentsInViewAngle();
    }

    public override IEnumerator Assertion()
    {
        yield return null;
        Assert.AreEqual(0, viewingPedestrianFov.visiblePedestrians.Count);
    }
}

public class FieldOfView_CannnotDetectOtherPedestrian_WhenPedestrianIsOutsideOfViewAngle_AndInsideOfViewRadius : ArrangeActAssertStrategy
{
    private Pedestrian[] pedestrians;
    private FieldOfView viewingPedestrianFov;
    private Pedestrian targetPedestrian;
    private Vector3 outsideViewAngleAndInsideRadius;

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
        pedestrians = SpawnPedestrians(2);
        viewingPedestrianFov = FieldOfViewTestsHelper.SetUpViewingPedestrian(pedestrians[0]);

        targetPedestrian = pedestrians[1];
        outsideViewAngleAndInsideRadius = new Vector3(15, 0, -5);
        targetPedestrian.transform.position = outsideViewAngleAndInsideRadius;
    }

    public override void Act()
    {
        viewingPedestrianFov.GetAllAgentsInViewAngle();
    }

    public override IEnumerator Assertion()
    {
        yield return null;
        Assert.AreEqual(0, viewingPedestrianFov.visiblePedestrians.Count);
    }
}

public class FieldOfView_CannnotDetectOtherPedestrian_WhenPedestrianIsWithinViewAngleAndViewRadius_WhenObstacleInBetween : ArrangeActAssertStrategy
{
    private Pedestrian[] pedestrians;
    private FieldOfView viewingPedestrianFov;
    private Pedestrian targetPedestrian;
    private Vector3 insideViewAngleAndInsideRadius;
    private Vector3 obstructionScale;
    private Vector3 obstructionPosition;
    private string obstacleLayerMask;

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

        obstructionScale = new Vector3(20, 15, 2);
        obstructionPosition = new Vector3(-5, 0, 5);
        obstacleLayerMask = "Obstacle";
        FieldOfViewTestsHelper.SetUpNonPedestrianObject(obstructionScale, obstructionPosition, obstacleLayerMask);

        pedestrians = SpawnPedestrians(2);
        viewingPedestrianFov = FieldOfViewTestsHelper.SetUpViewingPedestrian(pedestrians[0]);

        targetPedestrian = pedestrians[1];
        insideViewAngleAndInsideRadius = new Vector3(5, 0, 10);
        targetPedestrian.transform.position = insideViewAngleAndInsideRadius;
    }

    public override void Act()
    {
        viewingPedestrianFov.GetAllAgentsInViewAngle();
    }

    public override IEnumerator Assertion()
    {
        yield return null;
        Assert.AreEqual(0, viewingPedestrianFov.visiblePedestrians.Count);
    }
}

public class FieldOfView_OnlyAddsPedestriansToVisiblePedestrainList : ArrangeActAssertStrategy
{
    private Pedestrian[] pedestrians;
    private FieldOfView viewingPedestrianFov;
    private Pedestrian targetPedestrian;
    private Vector3 insideViewAngleAndInsideRadius;
    private Vector3 nonObstructingObjectScale = new Vector3(2, 2, 2);
    private Vector3 nonObstructingObjectPosition = new Vector3(8, 0, 10);
    private string defaultLayerMask;

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

        nonObstructingObjectScale = new Vector3(2, 2, 2);
        nonObstructingObjectPosition = new Vector3(8, 0, 10);
        defaultLayerMask = "Default";
        FieldOfViewTestsHelper.SetUpNonPedestrianObject(nonObstructingObjectScale, nonObstructingObjectPosition, defaultLayerMask);

        pedestrians = SpawnPedestrians(2);
        viewingPedestrianFov = FieldOfViewTestsHelper.SetUpViewingPedestrian(pedestrians[0]);

        targetPedestrian = pedestrians[1];
        targetPedestrian.transform.position = insideViewAngleAndInsideRadius;
    }

    public override void Act()
    {
        viewingPedestrianFov.GetAllAgentsInViewAngle();
    }

    public override IEnumerator Assertion()
    {
        yield return null;
        Assert.AreEqual(1, viewingPedestrianFov.visiblePedestrians.Count);
        Assert.IsInstanceOf(typeof(Pedestrian), viewingPedestrianFov.visiblePedestrians[0]);
    }
}

public static class FieldOfViewTestsHelper
{
    public static FieldOfView SetUpViewingPedestrian(Pedestrian pedestrian)
    {
        Vector3 viewingPedestrianPosition = new Vector3(0, 0, 0);
        float defaultViewAngle = 90f;
        float defaultViewRadius = 100f;

        pedestrian.transform.position = viewingPedestrianPosition;
        FieldOfView viewingPedestrianFov = pedestrian.GetComponentInChildren<FieldOfView>();
        viewingPedestrianFov.viewAngle = defaultViewAngle;
        viewingPedestrianFov.viewRadius = defaultViewRadius;
        viewingPedestrianFov.visiblePedestrians.Clear();

        return viewingPedestrianFov;
    }

    public static GameObject SetUpNonPedestrianObject(Vector3 scale, Vector3 position, string layerMask)
    {
        GameObject nonPedestrianObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        nonPedestrianObject.transform.localScale = scale;
        nonPedestrianObject.transform.position = position;
        nonPedestrianObject.AddComponent<BoxCollider>();
        nonPedestrianObject.layer = LayerMask.NameToLayer(layerMask);

        return nonPedestrianObject;
    }
}
