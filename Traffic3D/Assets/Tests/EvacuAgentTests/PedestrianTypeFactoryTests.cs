using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class WorkerPedestrianFactory_AddEvacuAgentBehaviour_CorrectlyAddsWorkerBehaviours : ArrangeActAssertStrategy
{
    private AbstractEvacuAgentPedestrianFactory pedestrianBehaviourFactory;
    private BehaviourController behaviourController;
    private Pedestrian pedestrian;
    private GameObject gameObject;
    private EvacuAgentPedestrianBase actualEvacuAgentPedestrian;

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
        pedestrianBehaviourFactory = GameObject.FindObjectOfType<WorkerPedestrianFactory>();
        gameObject = SpawnGameObjectWithInactivePedestrianScript();
        pedestrian = gameObject.GetComponent<Pedestrian>();
        Assert.Null(pedestrian.GetComponentInChildren<FieldOfView>());
    }

    public override void Act()
    {
        actualEvacuAgentPedestrian = pedestrianBehaviourFactory.CreateEvacuaAgentPedestrian(pedestrian);
        behaviourController = actualEvacuAgentPedestrian.behaviourController;
        behaviourController.isUpdateOn = false;
    }

    public override void Assertion()
    {
        Assert.IsFalse(pedestrian.isShooterAgent);
        StringAssert.Contains(actualEvacuAgentPedestrian.tag, EvacuAgentSceneParamaters.PEDESTRIAN_TAG);

        Assert.NotNull(pedestrian.GetComponentInChildren<FieldOfView>());
        Assert.NotNull(actualEvacuAgentPedestrian.behaviourController);
        Assert.NotNull(actualEvacuAgentPedestrian.behaviourController.behaviourCollections);
        Assert.IsInstanceOf(typeof(WorkerBehaviourTypeOrder), actualEvacuAgentPedestrian.behaviourTypeOrder);
        Assert.IsInstanceOf(typeof(WorkerPedestrianPointPathCreator), actualEvacuAgentPedestrian.PedestrianPointPathCreator);
    }
}

public class ShooterPedestrianFactory_AddEvacuAgentBehaviour_CorrectlyAddsShooterBehaviours : ArrangeActAssertStrategy
{
    private AbstractEvacuAgentPedestrianFactory shooterPedestrianFactory;
    private BehaviourController shooterBehaviourController;
    private Pedestrian pedestrian;
    private ShooterPedestrian shooterPedestrian;
    private GameObject pedestrianGameObject;

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
        shooterPedestrianFactory = GameObject.FindObjectOfType<ShooterPedestrianFactory>();

        pedestrianGameObject = SpawnGameObjectWithInactivePedestrianScript();
        pedestrian = pedestrianGameObject.GetComponent<Pedestrian>();
        Assert.Null(pedestrian.GetComponentInChildren<FieldOfView>());
    }

    public override void Act()
    {
        shooterPedestrian = (ShooterPedestrian)shooterPedestrianFactory.CreateEvacuaAgentPedestrian(pedestrian);
        shooterBehaviourController = shooterPedestrian.behaviourController;
        shooterBehaviourController.isUpdateOn = false;
    }

    public override void Assertion()
    {
        Assert.IsTrue(pedestrian.isShooterAgent);
        StringAssert.Contains(shooterPedestrian.tag, EvacuAgentSceneParamaters.SHOOTER_TAG);

        Assert.NotNull(pedestrian.GetComponentInChildren<FieldOfView>());
        Assert.AreEqual(1, GameObject.FindGameObjectsWithTag(EvacuAgentSceneParamaters.SHOOTER_TAG).Length);
        Assert.NotNull(shooterPedestrian.behaviourController);
        Assert.NotNull(shooterPedestrian.behaviourController.behaviourCollections);
        Assert.IsInstanceOf(typeof(ShooterBehaviourTypeOrder), shooterPedestrian.behaviourTypeOrder);
        Assert.IsInstanceOf(typeof(ShooterPedestrianPointPathCreator), shooterPedestrian.PedestrianPointPathCreator);
    }
}
