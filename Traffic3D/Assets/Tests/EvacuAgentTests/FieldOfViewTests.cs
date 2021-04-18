using NUnit.Framework;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

public class FieldOfView_CanDetectOtherPedestrian_WhenPedestrianIsWithinViewAngleAndViewRadius : ArrangeActAssertStrategy
{
    private FieldOfView viewingObjectFov;
    private Vector3 insideViewAngleAndInsideRadius;
    private GameObject targetObject;

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

        viewingObjectFov = FieldOfViewTestsHelper.SetUpViewingGameObject();
        insideViewAngleAndInsideRadius = new Vector3(5, 0, 10);
        targetObject = FieldOfViewTestsHelper.SetUpNonViewingObject(insideViewAngleAndInsideRadius);
    }

    public override void Act()
    {
        viewingObjectFov.GetAllAgentsInViewAngle();
    }

    public override void Assertion()
    {
        Assert.AreEqual(1, viewingObjectFov.allVisiblePedestrians.Count);
        Assert.AreSame(targetObject.GetComponent<Pedestrian>(), viewingObjectFov.allVisiblePedestrians.First());
    }
}

public class FieldOfView_CannnotDetectOtherPedestrian_WhenPedestrianIsWithinViewAngle_AndOutsideOfViewRadius : ArrangeActAssertStrategy
{
    private FieldOfView viewingObjectFov;
    private Vector3 insideViewAngleAndOutsideRadius;
    private GameObject targetObject;

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

        viewingObjectFov = FieldOfViewTestsHelper.SetUpViewingGameObject();
        insideViewAngleAndOutsideRadius = new Vector3(0, 0, 150);
        targetObject = FieldOfViewTestsHelper.SetUpNonViewingObject(insideViewAngleAndOutsideRadius);
    }

    public override void Act()
    {
        viewingObjectFov.GetAllAgentsInViewAngle();
    }

    public override void Assertion()
    {
        Assert.AreEqual(0, viewingObjectFov.allVisiblePedestrians.Count);
    }
}

public class FieldOfView_CannnotDetectOtherPedestrian_WhenPedestrianIsOutsideOfViewAngle_AndInsideOfViewRadius : ArrangeActAssertStrategy
{
    private FieldOfView viewingObjectFov;
    private Vector3 outsideViewAngleAndInsideRadius;
    private GameObject targetObject;


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

        viewingObjectFov = FieldOfViewTestsHelper.SetUpViewingGameObject();
        outsideViewAngleAndInsideRadius = new Vector3(15, 0, -5);
        targetObject = FieldOfViewTestsHelper.SetUpNonViewingObject(outsideViewAngleAndInsideRadius);
    }

    public override void Act()
    {
        viewingObjectFov.GetAllAgentsInViewAngle();
    }

    public override void Assertion()
    {
        Assert.AreEqual(0, viewingObjectFov.allVisiblePedestrians.Count);
    }
}

public class FieldOfView_CannnotDetectOtherPedestrian_WhenPedestrianIsWithinViewAngleAndViewRadius_WhenObstacleInBetween : ArrangeActAssertStrategy
{
    private Vector3 insideViewAngleAndInsideRadius;
    private Vector3 obstructionScale;
    private Vector3 obstructionPosition;
    private GameObject obstacleObject;
    private FieldOfView viewingObjectFov;
    private GameObject targetObject;
    private string obstacleLayerMask;


    [UnityTest]
    public override IEnumerator PerformTest()
    {
        Arrange();
        yield return new WaitForSeconds(2);
        Act();
        Assertion();
    }

    public override void Arrange()
    {
        DisableLoops();

        obstructionScale = new Vector3(20, 15, 2);
        obstructionPosition = new Vector3(-2, 0, 5);
        obstacleLayerMask = "Obstacle";
        obstacleObject = FieldOfViewTestsHelper.SetUpNonViewingObject(obstructionPosition, obstacleLayerMask);
        obstacleObject.transform.localScale = obstructionScale;

        viewingObjectFov = FieldOfViewTestsHelper.SetUpViewingGameObject();

        insideViewAngleAndInsideRadius = new Vector3(5, 0, 10);
        targetObject = FieldOfViewTestsHelper.SetUpNonViewingObject(insideViewAngleAndInsideRadius);
    }

    public override void Act()
    {
        viewingObjectFov.GetAllAgentsInViewAngle();
    }

    public override void Assertion()
    {
        Assert.AreEqual(0, viewingObjectFov.allVisiblePedestrians.Count);
    }
}

public class FieldOfView_OnlyAddsPedestriansToVisiblePedestrainList : ArrangeActAssertStrategy
{
    private Vector3 insideViewAngleAndInsideRadius;
    private Vector3 nonTargetPosition;
    private GameObject nonTargetObject;
    private FieldOfView viewingObjectFov;
    private GameObject targetObject;
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

        viewingObjectFov = FieldOfViewTestsHelper.SetUpViewingGameObject();

        nonTargetPosition = new Vector3(-2, 0, 7);
        obstacleLayerMask = "Obstacle";
        nonTargetObject = FieldOfViewTestsHelper.SetUpNonViewingObject(nonTargetPosition, obstacleLayerMask);

        insideViewAngleAndInsideRadius = new Vector3(5, 0, 10);
        targetObject = FieldOfViewTestsHelper.SetUpNonViewingObject(insideViewAngleAndInsideRadius);
    }

    public override void Act()
    {
        viewingObjectFov.GetAllAgentsInViewAngle();
    }

    public override void Assertion()
    {
        Assert.AreEqual(1, viewingObjectFov.allVisiblePedestrians.Count);
        Assert.IsInstanceOf(typeof(Pedestrian), viewingObjectFov.allVisiblePedestrians[0]);
    }
}

public static class FieldOfViewTestsHelper
{
    public static Vector3 viewingObjectPosition = new Vector3(0, 0, 0);
    public const string pedestrianLayerMask = "Pedestrian";

    private static string fieldFOViewPrefabPath = $"{EvacuAgentSceneParamaters.RESEOURCES_PREFABS_PREFIX}FieldOfView";

    public static FieldOfView SetUpViewingGameObject()
    {
        float defaultViewAngle = 90f;
        float defaultViewRadius = 100f;

        GameObject viewingFovGameObject = (GameObject)GameObject.Instantiate(Resources.Load(fieldFOViewPrefabPath));
        FieldOfView viewingFov = viewingFovGameObject.GetComponent<FieldOfView>();
        viewingFov.StopAllCoroutines();

        viewingFov.viewAngle = defaultViewAngle;
        viewingFov.viewRadius = defaultViewRadius;
        viewingFov.allVisiblePedestrians.Clear();
        viewingFovGameObject.layer = LayerMask.NameToLayer(pedestrianLayerMask);

        return viewingFov;
    }

    public static GameObject SetUpNonViewingObject(Vector3 position, string layerMask = pedestrianLayerMask)
    {
        GameObject nonPedestrianObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        nonPedestrianObject.transform.localScale = new Vector3(2, 2, 3);
        nonPedestrianObject.transform.position = position;
        nonPedestrianObject.AddComponent<Pedestrian>().enabled = false;
        nonPedestrianObject.layer = LayerMask.NameToLayer(layerMask);

        return nonPedestrianObject;
    }
}
