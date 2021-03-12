using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class PedestrianBehaviourFactory_AddEvacuAgentBehaviour_CorrectlyAddsNonShooterBehaviour : ArrangeActAssertStrategy
{
    private PedestrianBehaviourFactory pedestrianBehaviourFactory;
    private BehaviourController behaviourController;
    private Pedestrian pedestrian;
    private GameObject gameObject;

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
        pedestrianBehaviourFactory = GameObject.FindObjectOfType<PedestrianBehaviourFactory>();
        gameObject = SpawnGameObjectWithInactivePedestrianScript();
        pedestrian = gameObject.GetComponent<Pedestrian>();
        Assert.Null(pedestrian.GetComponentInChildren<FieldOfView>());

        EvacuAgentSceneParamaters.NUMBER_OF_SHOOTER_AGENTS = 0;
    }

    public override void Act()
    {
        pedestrianBehaviourFactory.AddEvacuAgentBehaviour(pedestrian);
        behaviourController = pedestrian.GetComponentInChildren<BehaviourController>();
        behaviourController.enabled = false;
    }

    public override void Assertion()
    {
        Assert.NotNull(pedestrian.GetComponentInChildren<FieldOfView>());
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        EvacuAgentSceneParamaters.NUMBER_OF_SHOOTER_AGENTS = 1;
        yield return new ExitPlayMode();
    }
}

public class PedestrianBehaviourFactory_AddEvacuAgentBehaviour_CorrectlyAddsShooterBehaviour : ArrangeActAssertStrategy
{
    private PedestrianBehaviourFactory pedestrianBehaviourFactory;
    private BehaviourController shooterBehaviourController;
    private BehaviourController nonShooterbehaviourController;
    private Pedestrian nonShooterPedestrian;
    private Pedestrian shooterPedestrian;
    private GameObject nonShooterGameObject;
    private GameObject shooterGameObject;

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
        pedestrianBehaviourFactory = GameObject.FindObjectOfType<PedestrianBehaviourFactory>();

        shooterGameObject = SpawnGameObjectWithInactivePedestrianScript();
        shooterPedestrian = shooterGameObject.GetComponent<Pedestrian>();

        nonShooterGameObject = SpawnGameObjectWithInactivePedestrianScript();
        nonShooterPedestrian = nonShooterGameObject.GetComponent<Pedestrian>();
    }

    public override void Act()
    {
        pedestrianBehaviourFactory.AddEvacuAgentBehaviour(shooterPedestrian);
        shooterBehaviourController = shooterPedestrian.GetComponentInChildren<BehaviourController>();
        shooterBehaviourController.enabled = false;
        pedestrianBehaviourFactory.AddEvacuAgentBehaviour(nonShooterPedestrian);
        nonShooterbehaviourController = nonShooterPedestrian.GetComponentInChildren<BehaviourController>();
        nonShooterbehaviourController.enabled = false;

    }

    public override void Assertion()
    {
        Assert.IsFalse(nonShooterPedestrian.isShooterAgent);
        StringAssert.DoesNotContain(nonShooterPedestrian.tag, EvacuAgentSceneParamaters.SHOOTER_TAG);

        Assert.IsTrue(shooterPedestrian.isShooterAgent);
        StringAssert.Contains(shooterPedestrian.tag, EvacuAgentSceneParamaters.SHOOTER_TAG);

        Assert.AreEqual(1, GameObject.FindGameObjectsWithTag(EvacuAgentSceneParamaters.SHOOTER_HIGHLIGHT_TAG).Length);
        Assert.False(GameObject.FindGameObjectWithTag(EvacuAgentSceneParamaters.SHOOTER_HIGHLIGHT_TAG).GetComponent<MeshRenderer>().enabled);
        Assert.NotNull(shooterPedestrian.GetComponentInChildren<BehaviourController>());
        Assert.NotNull(shooterPedestrian.GetComponentInChildren<BehaviourCollection>());
    }
}
