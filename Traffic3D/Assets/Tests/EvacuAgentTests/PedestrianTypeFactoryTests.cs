using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

public class AbstractEvacuAgentPedestrianFactory_CorrectlyCreatesBehaviourCollection : ArrangeActAssertStrategy
{
    private BehaviourTypeOrder mockBehaviourTypeOrder;
    private WorkerLeaderFollowerPedestrianFactory workerPedestrianFactory;
    private BehaviourCollection behaviourCollection;
    private BehaviourController behaviourController;
    private List<BehaviourStrategy> actualBehaviourStrategies;

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
        mockBehaviourTypeOrder = BehaviourCollectionFactoryTestsHelper.GetMockBehaviourTypeOrder();
        behaviourController = BehaviourCollectionFactoryTestsHelper.GetBehaviourController();
        behaviourController.isUpdateOn = false;
        workerPedestrianFactory = BehaviourCollectionFactoryTestsHelper.GetBehaviourCollectionFactory();
        actualBehaviourStrategies = new List<BehaviourStrategy>();
    }

    public override void Act()
    {
        behaviourCollection = workerPedestrianFactory.GenerateBehaviourCollection(behaviourController, mockBehaviourTypeOrder);

        foreach (BehaviourStrategy behaviourStrategy in behaviourCollection.behaviours)
        {
            behaviourStrategy.enabled = false;
            actualBehaviourStrategies.Add(behaviourStrategy);
        }
    }

    public override void Assertion()
    {
        Assert.IsNotNull(behaviourCollection);
        Assert.AreEqual(2, behaviourCollection.behaviours.Count);
        Assert.IsInstanceOf(typeof(FollowClosestTargetBehaviour), actualBehaviourStrategies[0]);
        Assert.IsInstanceOf(typeof(MoveToNextDestinationBehaviour), actualBehaviourStrategies[1]);
    }
}

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
        pedestrianBehaviourFactory = GameObject.FindObjectOfType<WorkerLeaderFollowerPedestrianFactory>();
        gameObject = SpawnGameObjectWithInactivePedestrianScript();
        pedestrian = gameObject.GetComponent<Pedestrian>();
        Assert.Null(pedestrian.GetComponentInChildren<FieldOfView>());
    }

    public override void Act()
    {
        actualEvacuAgentPedestrian = pedestrianBehaviourFactory.CreateEvacuAgentPedestrian(pedestrian);
        behaviourController = actualEvacuAgentPedestrian.behaviourController;
        behaviourController.isUpdateOn = false;
    }

    public override void Assertion()
    {
        Assert.IsFalse(pedestrian.isShooterAgent);
        StringAssert.Contains(EvacuAgentSceneParamaters.WORKER_TAG, actualEvacuAgentPedestrian.tag);

        Assert.NotNull(pedestrian.GetComponentInChildren<FieldOfView>());
        Assert.NotNull(actualEvacuAgentPedestrian.behaviourController);
        Assert.NotNull(actualEvacuAgentPedestrian.behaviourController.behaviourCollections);
        Assert.IsInstanceOf(typeof(WorkerLeaderBehaviourTypeOrder), actualEvacuAgentPedestrian.behaviourTypeOrder);
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
        shooterPedestrian = (ShooterPedestrian)shooterPedestrianFactory.CreateEvacuAgentPedestrian(pedestrian);
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

public static class BehaviourCollectionFactoryTestsHelper
{
    public static BehaviourTypeOrder GetMockBehaviourTypeOrder()
    {
        GameObject shooterBehaviourTypeOrderGameObject = GameObject.Instantiate(new GameObject());
        return shooterBehaviourTypeOrderGameObject.AddComponent<MockBehaviourTypeOrder>();
    }

    public static WorkerLeaderFollowerPedestrianFactory GetBehaviourCollectionFactory()
    {
        return GameObject.FindObjectOfType<WorkerLeaderFollowerPedestrianFactory>();
    }

    public static BehaviourController GetBehaviourController()
    {
        GameObject behaviourControllerGameObject = GameObject.Instantiate(new GameObject());
        return behaviourControllerGameObject.AddComponent<BehaviourController>();
    }

    private class MockBehaviourTypeOrder : BehaviourTypeOrder
    {
        public override List<BehaviourType> GetBehaviourTypes()
        {
            return new List<BehaviourType>
            {
                new MockFollowClosestTargetBehaviourType(),
                new MockCreateWeightedPathOfPedestrianPointsBehaviourType(),
                new MockMoveToNextDestinationBehaviourtBehaviourType()
            };
        }

        private class MockFollowClosestTargetBehaviourType : BehaviourType
        {
            private readonly float behaviourStrategyChanceToUse = 1f;

            public override Type GetBehaviourStrategyClass<BehaviourStrategy>() => typeof(FollowClosestTargetBehaviour);
            public override float GetBehaviourStrategyChance() => behaviourStrategyChanceToUse;
        }

        private class MockCreateWeightedPathOfPedestrianPointsBehaviourType : BehaviourType
        {
            private readonly float behaviourStrategyChanceToUse = 0f;

            public override Type GetBehaviourStrategyClass<BehaviourStrategy>() => typeof(CreateWeightedPathOfPedestrianPointsBehaviour);
            public override float GetBehaviourStrategyChance() => behaviourStrategyChanceToUse;
        }

        private class MockMoveToNextDestinationBehaviourtBehaviourType : BehaviourType
        {
            private readonly float behaviourStrategyChanceToUse = 1f;

            public override Type GetBehaviourStrategyClass<BehaviourStrategy>() => typeof(MoveToNextDestinationBehaviour);
            public override float GetBehaviourStrategyChance() => behaviourStrategyChanceToUse;
        }
    }
}
