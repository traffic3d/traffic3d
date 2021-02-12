using NUnit.Framework;
using System.Collections;
using System.Linq;
using Tests;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Tests")]
public class FieldOfViewTests : EvacuAgentCommonSceneTest
{
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

        Pedestrian viewingPedestrian = pedestrians[0];
        FieldOfView viewingPedestrianFov = viewingPedestrian.GetComponentInChildren<FieldOfView>();
        viewingPedestrian.transform.position = new Vector3(0, 0, 0);
        viewingPedestrianFov.viewAngle = 90f;
        viewingPedestrianFov.viewRadius = 50f;

        Pedestrian targetPedestrian = pedestrians[1];
        FieldOfView targetPedestrianFov = targetPedestrian.GetComponentInChildren<FieldOfView>();
        targetPedestrian.transform.position = new Vector3(5, 0, 10);

        // Act
        viewingPedestrianFov.GetAllAgentsInViewAngle();

        // Assert
        Assert.AreNotSame(viewingPedestrianFov, targetPedestrianFov);
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

        Pedestrian viewingPedestrian = pedestrians[0];
        FieldOfView viewingPedestrianFov = viewingPedestrian.GetComponentInChildren<FieldOfView>();
        viewingPedestrian.transform.position = new Vector3(0, 0, 0);
        viewingPedestrianFov.viewAngle = 90f;
        viewingPedestrianFov.viewRadius = 20f;

        Pedestrian targetPedestrian = pedestrians[1];
        FieldOfView targetPedestrianFov = targetPedestrian.GetComponentInChildren<FieldOfView>();
        targetPedestrian.transform.position = new Vector3(0, 0, 35);

        // Act
        viewingPedestrianFov.GetAllAgentsInViewAngle();

        // Assert
        Assert.AreNotSame(viewingPedestrianFov, targetPedestrianFov);
        Assert.AreEqual(0, viewingPedestrianFov.visiblePedestrians.Count);
    }

    [UnityTest]
    public IEnumerator FieldOfView_CannnotDetectOtherPedestrian_WhenPedestrianIsOutsideOfViewAngle_AndInsideOfViewRadius()
    {
        // Arrange
        DisableLoops();
        yield return null;

        Pedestrian[] pedestrians = base.SpawnPedestrians(2);

        Pedestrian viewingPedestrian = pedestrians[0];
        FieldOfView viewingPedestrianFov = viewingPedestrian.GetComponentInChildren<FieldOfView>();
        viewingPedestrian.transform.position = new Vector3(0, 0, 0);
        viewingPedestrianFov.viewAngle = 90f;
        viewingPedestrianFov.viewRadius = 20f;

        Pedestrian targetPedestrian = pedestrians[1];
        FieldOfView targetPedestrianFov = targetPedestrian.GetComponentInChildren<FieldOfView>();
        targetPedestrian.transform.position = new Vector3(10, 0, 5);

        // Act
        viewingPedestrianFov.GetAllAgentsInViewAngle();

        // Assert
        Assert.AreNotSame(viewingPedestrianFov, targetPedestrianFov);
        Assert.AreEqual(0, viewingPedestrianFov.visiblePedestrians.Count);
    }

    [UnityTest]
    public IEnumerator FieldOfView_CannnotDetectOtherPedestrian_WhenPedestrianIsWithinViewAngleAndViewRadius_WhenObstacleInBetween()
    {
        // Arrange
        DisableLoops();
        yield return null;

        GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obstacle.transform.localScale = new Vector3(10, 15, 2);
        obstacle.transform.position = new Vector3(5, -2, 3);
        obstacle.AddComponent<BoxCollider>();
        obstacle.layer = LayerMask.NameToLayer("Obstacle");

        Pedestrian[] pedestrians = base.SpawnPedestrians(2);

        Pedestrian viewingPedestrian = pedestrians[0];
        FieldOfView viewingPedestrianFov = viewingPedestrian.GetComponentInChildren<FieldOfView>();
        viewingPedestrian.transform.position = new Vector3(0, 0, 0);
        viewingPedestrianFov.viewAngle = 90f;
        viewingPedestrianFov.viewRadius = 50f;

        Pedestrian targetPedestrian = pedestrians[1];
        FieldOfView targetPedestrianFov = targetPedestrian.GetComponentInChildren<FieldOfView>();
        targetPedestrian.transform.position = new Vector3(5, 0, 10);

        // Act
        viewingPedestrianFov.GetAllAgentsInViewAngle();

        // Assert
        Assert.AreNotSame(viewingPedestrianFov, targetPedestrianFov);
        Assert.AreEqual(0, viewingPedestrianFov.visiblePedestrians.Count);
    }

    [UnityTest]
    public IEnumerator FieldOfView_OnlyAddsPedestriansToVisiblePedestrainList()
    {
        // Arrange
        DisableLoops();
        yield return null;

        Pedestrian[] pedestrians = base.SpawnPedestrians(2);

        GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obstacle.transform.localScale = new Vector3(2, 2, 2);
        obstacle.transform.position = new Vector3(8, 0, 10);
        obstacle.AddComponent<BoxCollider>();
        obstacle.layer = LayerMask.NameToLayer("Default");

        Pedestrian viewingPedestrian = pedestrians[0];
        FieldOfView viewingPedestrianFov = viewingPedestrian.GetComponentInChildren<FieldOfView>();
        viewingPedestrian.transform.position = new Vector3(0, 0, 0);
        viewingPedestrianFov.viewAngle = 90f;
        viewingPedestrianFov.viewRadius = 50f;

        Pedestrian targetPedestrian = pedestrians[1];
        targetPedestrian.transform.position = new Vector3(5, 0, 10);

        // Act
        viewingPedestrianFov.GetAllAgentsInViewAngle();

        // Assert
        Assert.AreEqual(1, viewingPedestrianFov.visiblePedestrians.Count);
        Assert.IsInstanceOf(typeof(Pedestrian), viewingPedestrianFov.visiblePedestrians[0]);
    }
}
