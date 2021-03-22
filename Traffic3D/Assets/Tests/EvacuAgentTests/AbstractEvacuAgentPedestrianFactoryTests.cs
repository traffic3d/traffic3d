using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class AbstractEvacuAgentPedestrianFactory_CorrectlyCreatesBehaviourCollection : ArrangeActAssertStrategy
{
    private BehaviourTypeOrder mockBehaviourTypeOrder;
    private WorkerPedestrianFactory workerPedestrianFactory;
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

public static class BehaviourCollectionFactoryTestsHelper
{
    public static BehaviourTypeOrder GetMockBehaviourTypeOrder()
    {
        GameObject shooterBehaviourTypeOrderGameObject = GameObject.Instantiate(new GameObject());
        return shooterBehaviourTypeOrderGameObject.AddComponent<MockBehaviourTypeOrder>();
    }

    public static WorkerPedestrianFactory GetBehaviourCollectionFactory()
    {
        return GameObject.FindObjectOfType<WorkerPedestrianFactory>();
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
            private readonly string behaviourStrategyName = "FollowClosestTargetBehaviour";
            private readonly float behaviourStrategyChanceToUse = 1f;

            public override string GetBehaviourStrategyName() => behaviourStrategyName;
            public override float GetBehaviourStrategyChance() => behaviourStrategyChanceToUse;
        }

        private class MockCreateWeightedPathOfPedestrianPointsBehaviourType : BehaviourType
        {
            private readonly string behaviourStrategyName = "CreateWeightedPathOfPedestrianPointsBehaviour";
            private readonly float behaviourStrategyChanceToUse = 0f;

            public override string GetBehaviourStrategyName() => behaviourStrategyName;
            public override float GetBehaviourStrategyChance() => behaviourStrategyChanceToUse;
        }

        private class MockMoveToNextDestinationBehaviourtBehaviourType : BehaviourType
        {
            private readonly string behaviourStrategyName = "MoveToNextDestinationBehaviour";
            private readonly float behaviourStrategyChanceToUse = 1f;

            public override string GetBehaviourStrategyName() => behaviourStrategyName;
            public override float GetBehaviourStrategyChance() => behaviourStrategyChanceToUse;
        }
    }
}
