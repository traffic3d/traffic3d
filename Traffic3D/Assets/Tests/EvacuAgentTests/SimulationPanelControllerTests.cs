using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Tests;
using UnityEngine;
using UnityEngine.TestTools;


public class SimulationPanelController_TogglePedestrianFieldOfViewVisuals_CorrectlyEnablesFieldOfViewMeshRenderers : ArrangeActAssertStrategy
{
    private SimulationPanelController simulationPanelController;
    private FieldOfView[] fieldOfViews;
    private MeshRenderer[] fieldOfViewmeshRenderers;
    private bool isTurnedOn;
    private int numberOfFieldOfViews;

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
        numberOfFieldOfViews = 3;
        simulationPanelController = SimulationPanelControllerTestsHelper.GetSimulationPanelController();
        isTurnedOn = false;

        fieldOfViews = new FieldOfView[numberOfFieldOfViews];
        fieldOfViewmeshRenderers = new MeshRenderer[numberOfFieldOfViews];

        for (int index = 0; index < numberOfFieldOfViews; index++)
        {
            fieldOfViews[index] = SimulationPanelControllerTestsHelper.SetUpFieldOfView(!isTurnedOn);
            fieldOfViewmeshRenderers[index] = fieldOfViews[index].GetComponent<MeshRenderer>();
        }

        EvacuAgentSceneParamaters.IS_FOV_VISUAL_ENABLED = !isTurnedOn;
        Assert.AreEqual(3, fieldOfViewmeshRenderers.Length);
        Assert.IsTrue(fieldOfViewmeshRenderers[0].enabled);
        Assert.IsTrue(fieldOfViewmeshRenderers[1].enabled);
        Assert.IsTrue(fieldOfViewmeshRenderers[2].enabled);
    }

    public override void Act()
    {
        simulationPanelController.TogglePedestrianFieldOfViewVisuals(isTurnedOn);
    }

    public override void Assertion()
    {
        Assert.AreEqual(3, fieldOfViewmeshRenderers.Length);
        Assert.IsFalse(fieldOfViewmeshRenderers[0].enabled);
        Assert.IsFalse(fieldOfViewmeshRenderers[1].enabled);
        Assert.IsFalse(fieldOfViewmeshRenderers[2].enabled);
        Assert.IsFalse(EvacuAgentSceneParamaters.IS_FOV_VISUAL_ENABLED);
    }
}

public class SimulationPanelController_TogglePedestrianFieldOfViewVisuals_CorrectlyDisablesFieldOfViewMeshRenderers : ArrangeActAssertStrategy
{
    private SimulationPanelController simulationPanelController;
    private FieldOfView[] fieldOfViews;
    private MeshRenderer[] fieldOfViewmeshRenderers;
    private bool isTurnedOn;
    private int numberOfFieldOfViews;

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
        numberOfFieldOfViews = 3;
        simulationPanelController = SimulationPanelControllerTestsHelper.GetSimulationPanelController();
        isTurnedOn = true;

        fieldOfViews = new FieldOfView[numberOfFieldOfViews];
        fieldOfViewmeshRenderers = new MeshRenderer[numberOfFieldOfViews];

        for (int index = 0; index < numberOfFieldOfViews; index++)
        {
            fieldOfViews[index] = SimulationPanelControllerTestsHelper.SetUpFieldOfView(!isTurnedOn);
            fieldOfViewmeshRenderers[index] = fieldOfViews[index].GetComponent<MeshRenderer>();
        }

        EvacuAgentSceneParamaters.IS_FOV_VISUAL_ENABLED = !isTurnedOn;
        Assert.AreEqual(3, fieldOfViewmeshRenderers.Length);
        Assert.IsFalse(fieldOfViewmeshRenderers[0].enabled);
        Assert.IsFalse(fieldOfViewmeshRenderers[1].enabled);
        Assert.IsFalse(fieldOfViewmeshRenderers[2].enabled);
    }

    public override void Act()
    {
        simulationPanelController.TogglePedestrianFieldOfViewVisuals(isTurnedOn);
    }

    public override void Assertion()
    {
        Assert.AreEqual(3, fieldOfViewmeshRenderers.Length);
        Assert.IsTrue(fieldOfViewmeshRenderers[0].enabled);
        Assert.IsTrue(fieldOfViewmeshRenderers[1].enabled);
        Assert.IsTrue(fieldOfViewmeshRenderers[2].enabled);
        Assert.IsTrue(EvacuAgentSceneParamaters.IS_FOV_VISUAL_ENABLED);
    }
}

public class SimulationPanelController_ToggleShooterPedestrianColour_CorrectlyEnablesShooterHighlightMeshRenderers : ArrangeActAssertStrategy
{
    private SimulationPanelController simulationPanelController;
    private List<MeshRenderer> meshRenderers;
    private bool isHighlightToggledOn;
    private int numberOfHighlights;

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
        simulationPanelController = SimulationPanelControllerTestsHelper.GetSimulationPanelController();
        numberOfHighlights = 2;
        meshRenderers = new List<MeshRenderer>();

        for (int index = 0; index < numberOfHighlights; index++)
        {
            GameObject gameObject = EvacuAgentCommonSceneTest.SpawnGameObjectWithInactivePedestrianScript(EvacuAgentSceneParamaters.SHOOTER_HIGHLIGHT_TAG);
            meshRenderers.Add(gameObject.GetComponent<MeshRenderer>());
        }

        isHighlightToggledOn = true;
        meshRenderers = SimulationPanelControllerTestsHelper.SetUpInitialShooterHighlightMeshRenderers(!isHighlightToggledOn);
    }

    public override void Act()
    {
        simulationPanelController.ToggleShooterPedestrianColour(isHighlightToggledOn);
    }

    public override void Assertion()
    {
        Assert.AreEqual(2, meshRenderers.Count);
        Assert.IsTrue(meshRenderers[0].enabled);
        Assert.IsTrue(meshRenderers[1].enabled);
    }
}

public class SimulationPanelController_ToggleShooterPedestrianColour_CorrectlyDisablesShooterHighlightMeshRenderers : ArrangeActAssertStrategy
{
    private SimulationPanelController simulationPanelController;
    private List<MeshRenderer> meshRenderers;
    private bool isHighlightToggledOn;
    private int numberOfHighlights;

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
        simulationPanelController = SimulationPanelControllerTestsHelper.GetSimulationPanelController();
        numberOfHighlights = 2;
        meshRenderers = new List<MeshRenderer>();

        for (int index = 0; index < numberOfHighlights; index++)
        {
            GameObject gameObject = EvacuAgentCommonSceneTest.SpawnGameObjectWithInactivePedestrianScript(EvacuAgentSceneParamaters.SHOOTER_HIGHLIGHT_TAG);
            meshRenderers.Add(gameObject.GetComponent<MeshRenderer>());
        }

        isHighlightToggledOn = false;
        meshRenderers = SimulationPanelControllerTestsHelper.SetUpInitialShooterHighlightMeshRenderers(!isHighlightToggledOn);
    }

    public override void Act()
    {
        simulationPanelController.ToggleShooterPedestrianColour(isHighlightToggledOn);
    }

    public override void Assertion()
    {
        Assert.AreEqual(2, meshRenderers.Count);
        Assert.IsFalse(meshRenderers[0].enabled);
        Assert.IsFalse(meshRenderers[1].enabled);
    }
}

public static class SimulationPanelControllerTestsHelper
{
    public static string untaggedString = "Untagged";
    private static string userInterfacePrefabPathWay = $"{EvacuAgentSceneParamaters.RESEOURCES_PREFABS_PREFIX}UserInterface";
    private static string fieldofViewPrefabPath = $"{EvacuAgentSceneParamaters.RESEOURCES_PREFABS_PREFIX}FieldOfView";

    public static SimulationPanelController GetSimulationPanelController()
    {
        GameObject userInterface = GameObject.Instantiate(new GameObject());
        return userInterface.AddComponent<SimulationPanelController>();
    }

    public static MeshRenderer[] SetUpInitialFieldOfViewMeshRenderers(Pedestrian[] pedestrians, bool isTurnedOnBeforeToggle)
    {
        MeshRenderer[] fieldOfViewMeshRenderers = new MeshRenderer[pedestrians.Length];

        for(int index = 0; index < pedestrians.Length; index++)
        {
            fieldOfViewMeshRenderers[index] = pedestrians[index].GetComponentInChildren<FieldOfView>().GetComponent<MeshRenderer>();
            fieldOfViewMeshRenderers[index].enabled = isTurnedOnBeforeToggle;
        }

        return fieldOfViewMeshRenderers;
    }

    public static List<MeshRenderer> SetUpInitialShooterHighlightMeshRenderers(bool isTurnedOnBeforeToggle)
    {
        List<MeshRenderer> shooterHighlightMeshRenderers = new List<MeshRenderer>();

        foreach (GameObject shooterHighlight in GameObject.FindGameObjectsWithTag(EvacuAgentSceneParamaters.SHOOTER_HIGHLIGHT_TAG))
        {
            shooterHighlight.GetComponent<MeshRenderer>().enabled = isTurnedOnBeforeToggle;
            shooterHighlightMeshRenderers.Add(shooterHighlight.GetComponent<MeshRenderer>());
        }

        return shooterHighlightMeshRenderers;
    }

    public static FieldOfView SetUpFieldOfView(bool isTurnedOnBeforeToggle)
    {
        GameObject fovGameObject = (GameObject)GameObject.Instantiate(Resources.Load(fieldofViewPrefabPath));
        FieldOfView fov = fovGameObject.GetComponent<FieldOfView>();
        fov.GetComponent<FieldOfView>().enabled = false;
        fov.GetComponent<MeshRenderer>().enabled = isTurnedOnBeforeToggle;
        fov.StopAllCoroutines();

        return fov;
    }
}
