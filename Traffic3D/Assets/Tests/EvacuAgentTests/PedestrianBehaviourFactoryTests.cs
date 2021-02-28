using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PedestrianBehaviourFactory_AddEvacuAgentBehaviour_CorrectlyAddsNonShooterBehaviour : ArrangeActAssertStrategy
{
    private PedestrianBehaviourFactory pedestrianBehaviourFactory;
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

        Assert.Null(pedestrian.GetComponent<FieldOfView>());
        Assert.Null(pedestrian.GetComponentInChildren<FieldOfView>());
    }

    public override void Act()
    {
        pedestrianBehaviourFactory.AddEvacuAgentBehaviour(pedestrian);
    }

    public override void Assertion()
    {
        Assert.NotNull(pedestrian.GetComponentInChildren<FieldOfView>());
    }
}

public class PedestrianBehaviourFactory_AddEvacuAgentBehaviour_CorrectlyAddsShooterBehaviour : ArrangeActAssertStrategy
{
    private PedestrianBehaviourFactory pedestrianBehaviourFactory;
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
        pedestrianBehaviourFactory.AddEvacuAgentBehaviour(nonShooterPedestrian);
    }

    public override void Assertion()
    {
        Assert.IsFalse(nonShooterPedestrian.isShooterAgent);
        StringAssert.DoesNotContain(nonShooterPedestrian.tag, EvacuAgentSceneParamaters.SHOOTER_TAG);

        Assert.IsTrue(shooterPedestrian.isShooterAgent);
        StringAssert.Contains(shooterPedestrian.tag, EvacuAgentSceneParamaters.SHOOTER_TAG);

        Assert.AreEqual(1, GameObject.FindGameObjectsWithTag(EvacuAgentSceneParamaters.SHOOTER_HIGHLIGHT_TAG).Length);
        Assert.False(GameObject.FindGameObjectWithTag(EvacuAgentSceneParamaters.SHOOTER_HIGHLIGHT_TAG).GetComponent<MeshRenderer>().enabled);
    }
}
