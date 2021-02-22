using NUnit.Framework;
using System.Collections;
using System.Linq;
using Tests;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Tests")]
public class FieldOfViewTests : EvacuAgentCommonSceneTest
{
    private const float DEFAULT_VIEW_ANGLE = 90f;
    private const float DEFAULT_VIEW_RADIUS = 100f;
    private const string OBSTACLE_LAYER_MASK = "Obstacle";
    private const string DEFAULT_LAYER_MASK = "Default";

    private Vector3 viewingPedestrianPosition = new Vector3(0, 0, 0);
    private Vector3 insideViewAngleAndInsideRadius = new Vector3(5, 0, 10);
    private Vector3 insideViewAngleAndOutsideRadius = new Vector3(0, 0, 150);
    private Vector3 outsideViewAngleAndInsideRadius = new Vector3(15, 0, -5);

    private Vector3 obstructionScale = new Vector3(20, 15, 2);
    private Vector3 obstructionPosition = new Vector3(-5, 0, 5);
    private Vector3 nonObstructingObjectScale= new Vector3(2, 2, 2);
    private Vector3 nonObstructingObjectPosition = new Vector3(8, 0, 10);

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        yield return null;
        foreach (Pedestrian pedestrian in GameObject.FindObjectsOfType<Pedestrian>())
        {
            GameObject.Destroy(pedestrian);
        }
    }

    [UnityTest]
    public IEnumerator FieldOfView_CanDetectOtherPedestrian_WhenPedestrianIsWithinViewAngleAndViewRadius()
    {
        // Arrange
        DisableLoops();
        yield return null;

        Pedestrian[] pedestrians = base.SpawnPedestrians(2);

        FieldOfView viewingPedestrianFov = SetUpViewingPedestrian(pedestrians[0]);

        Pedestrian targetPedestrian = pedestrians[1];
        targetPedestrian.transform.position = insideViewAngleAndInsideRadius;

        // Act
        viewingPedestrianFov.GetAllAgentsInViewAngle();

        // Assert
        Assert.AreEqual(1, viewingPedestrianFov.visiblePedestrians.Count);
        Assert.AreSame(viewingPedestrianFov.visiblePedestrians.First(), targetPedestrian);
    }

    [UnityTest]
    public IEnumerator FieldOfView_CannnotDetectOtherPedestrian_WhenPedestrianIsWithinViewAngle_AndOutsideOfViewRadius()
    {
        // Arrange
        DisableLoops();
        yield return null;

        Pedestrian[] pedestrians = base.SpawnPedestrians(2);

        FieldOfView viewingPedestrianFov = SetUpViewingPedestrian(pedestrians[0]);

        Pedestrian targetPedestrian = pedestrians[1];
        targetPedestrian.transform.position = insideViewAngleAndOutsideRadius;

        // Act
        viewingPedestrianFov.GetAllAgentsInViewAngle();

        // Assert
        Assert.AreEqual(0, viewingPedestrianFov.visiblePedestrians.Count);
    }

    [UnityTest]
    public IEnumerator FieldOfView_CannnotDetectOtherPedestrian_WhenPedestrianIsOutsideOfViewAngle_AndInsideOfViewRadius()
    {
        // Arrange
        DisableLoops();
        yield return null;

        Pedestrian[] pedestrians = base.SpawnPedestrians(2);

        FieldOfView viewingPedestrianFov = SetUpViewingPedestrian(pedestrians[0]);

        Pedestrian targetPedestrian = pedestrians[1];
        targetPedestrian.transform.position = outsideViewAngleAndInsideRadius;

        // Act
        viewingPedestrianFov.GetAllAgentsInViewAngle();

        // Assert
        Assert.AreEqual(0, viewingPedestrianFov.visiblePedestrians.Count);
    }

    [UnityTest]
    public IEnumerator FieldOfView_CannnotDetectOtherPedestrian_WhenPedestrianIsWithinViewAngleAndViewRadius_WhenObstacleInBetween()
    {
        // Arrange
        DisableLoops();
        yield return null;

        SetUpNonPedestrianObject(obstructionScale, obstructionPosition, OBSTACLE_LAYER_MASK);
        Pedestrian[] pedestrians = base.SpawnPedestrians(2);

        FieldOfView viewingPedestrianFov = SetUpViewingPedestrian(pedestrians[0]);

        Pedestrian targetPedestrian = pedestrians[1];
        targetPedestrian.transform.position = insideViewAngleAndInsideRadius;

        // Act
        viewingPedestrianFov.GetAllAgentsInViewAngle();

        // Assert
        Assert.AreEqual(0, viewingPedestrianFov.visiblePedestrians.Count);
    }

    [UnityTest]
    public IEnumerator FieldOfView_OnlyAddsPedestriansToVisiblePedestrainList()
    {
        // Arrange
        DisableLoops();
        yield return null;

        Pedestrian[] pedestrians = base.SpawnPedestrians(2);
        SetUpNonPedestrianObject(nonObstructingObjectScale, nonObstructingObjectPosition, DEFAULT_LAYER_MASK);

        FieldOfView viewingPedestrianFov = SetUpViewingPedestrian(pedestrians[0]);

        Pedestrian targetPedestrian = pedestrians[1];
        targetPedestrian.transform.position = insideViewAngleAndInsideRadius;

        // Act
        viewingPedestrianFov.GetAllAgentsInViewAngle();

        // Assert
        Assert.AreEqual(1, viewingPedestrianFov.visiblePedestrians.Count);
        Assert.IsInstanceOf(typeof(Pedestrian), viewingPedestrianFov.visiblePedestrians[0]);
    }

    private FieldOfView SetUpViewingPedestrian(Pedestrian pedestrian)
    {
        pedestrian.transform.position = viewingPedestrianPosition;
        FieldOfView viewingPedestrianFov = pedestrian.GetComponentInChildren<FieldOfView>();
        viewingPedestrianFov.viewAngle = DEFAULT_VIEW_ANGLE;
        viewingPedestrianFov.viewRadius = DEFAULT_VIEW_RADIUS;
        viewingPedestrianFov.visiblePedestrians.Clear();

        return viewingPedestrianFov;
    }

    private GameObject SetUpNonPedestrianObject(Vector3 scale, Vector3 position, string layerMask)
    {
        GameObject nonPedestrianObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        nonPedestrianObject.transform.localScale = scale;
        nonPedestrianObject.transform.position = position;
        nonPedestrianObject.AddComponent<BoxCollider>();
        nonPedestrianObject.layer = LayerMask.NameToLayer(layerMask);

        return nonPedestrianObject;
    }
}
