using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


public class SimulationPanelController_TogglePedestrianFieldOfViewVisuals_CorrectlyEnablesFieldOfViewMeshRenderers : ArrangeActAssertStrategy
{
    private SimulationPanelController simulationPanelController;
    private Pedestrian[] pedestrians;
    private MeshRenderer[] fieldOfViewmeshRenderers;
    private bool isTurnedOn;

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
        pedestrians = SpawnPedestrians(3);
        isTurnedOn = false;
        EvacuAgentSceneParamaters.IS_FOV_VISUAL_ENABLED = !isTurnedOn;
        fieldOfViewmeshRenderers = SimulationPanelControllerTestsHelper.SetUpInitialFieldOfViewMeshRenderers(pedestrians, !isTurnedOn);

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
    private Pedestrian[] pedestrians;
    private MeshRenderer[] fieldOfViewmeshRenderers;
    private bool isTurnedOn;

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
        pedestrians = SpawnPedestrians(3);
        isTurnedOn = true;
        EvacuAgentSceneParamaters.IS_FOV_VISUAL_ENABLED = !isTurnedOn;
        fieldOfViewmeshRenderers = SimulationPanelControllerTestsHelper.SetUpInitialFieldOfViewMeshRenderers(pedestrians, !isTurnedOn);

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
    private Pedestrian[] pedestrians;
    private List<MeshRenderer> meshRenderers;
    private bool isHighlightToggledOn;

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
        EvacuAgentSceneParamaters.NUMBER_OF_SHOOTER_AGENTS = 2;

        pedestrians = SpawnPedestrians(2);
        Assert.IsTrue(pedestrians[0].isShooterAgent);
        Assert.IsTrue(pedestrians[1].isShooterAgent);

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
    private Pedestrian[] pedestrians;
    private List<MeshRenderer> meshRenderers;
    private bool isHighlightToggledOn;

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
        EvacuAgentSceneParamaters.NUMBER_OF_SHOOTER_AGENTS = 2;

        pedestrians = SpawnPedestrians(2);
        Assert.IsTrue(pedestrians[0].isShooterAgent);
        Assert.IsTrue(pedestrians[1].isShooterAgent);

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
    public static SimulationPanelController GetSimulationPanelController()
    {
        return GameObject.FindObjectOfType<SimulationPanelController>();
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
}
