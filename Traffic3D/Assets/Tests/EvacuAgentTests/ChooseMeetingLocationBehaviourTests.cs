using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;

public class ChooseMeetingLocationBehaviourTests_ShouldTriggerBehaviour_ReturnsTrue_WhenMeetingLocationIsNotChosen : ArrangeActAssertStrategy
{
    private GameObject parentObj;
    private ChooseMeetingLocationBehaviour chooseMeetingLocationBehaviour;
    private bool actualBool;

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
        parentObj = ChooseMeetingLocationBehaviourTestsHelper.SetupChooseMeetingLocationBehaviourGameObject();
        chooseMeetingLocationBehaviour = parentObj.GetComponentInChildren<ChooseMeetingLocationBehaviour>();

        chooseMeetingLocationBehaviour.isMeetingLocationChosen = false;
    }

    public override void Act()
    {
        actualBool = chooseMeetingLocationBehaviour.ShouldTriggerBehaviour();
    }

    public override void Assertion()
    {
        Assert.True(actualBool);
    }
}

public class ChooseMeetingLocationBehaviourTests_ShouldTriggerBehaviour_ReturnsFalse_WhenMeetingLocationIsChosen : ArrangeActAssertStrategy
{
    private GameObject parentObj;
    private ChooseMeetingLocationBehaviour chooseMeetingLocationBehaviour;
    private bool actualBool;

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
        parentObj = ChooseMeetingLocationBehaviourTestsHelper.SetupChooseMeetingLocationBehaviourGameObject();
        chooseMeetingLocationBehaviour = parentObj.GetComponentInChildren<ChooseMeetingLocationBehaviour>();

        chooseMeetingLocationBehaviour.isMeetingLocationChosen = true;
    }

    public override void Act()
    {
        actualBool = chooseMeetingLocationBehaviour.ShouldTriggerBehaviour();
    }

    public override void Assertion()
    {
        Assert.False(actualBool);
    }
}

public class ChooseMeetingLocationBehaviourTests_PerformBehaviour_CorrectlySetsNavMeshAgentDestination : ArrangeActAssertStrategy
{
    private GameObject parentObj;
    private ChooseMeetingLocationBehaviour chooseMeetingLocationBehaviour;
    private NavMeshAgent navMeshAgent;
    private Vector3 originalDestination;

    [UnityTest]
    public override IEnumerator PerformTest()
    {
        Arrange();
        yield return null;
        Act();
        Assertion();
    }

    public override void Arrange()
    {
        parentObj = ChooseMeetingLocationBehaviourTestsHelper.SetupChooseMeetingLocationBehaviourGameObject();
        chooseMeetingLocationBehaviour = parentObj.GetComponentInChildren<ChooseMeetingLocationBehaviour>();
        navMeshAgent = parentObj.GetComponent<NavMeshAgent>();

        originalDestination = navMeshAgent.destination;
    }

    public override void Act()
    {
        chooseMeetingLocationBehaviour.PerformBehaviour();
    }

    public override void Assertion()
    {
        Assert.AreNotEqual(navMeshAgent.destination, originalDestination);
    }
}

public static class ChooseMeetingLocationBehaviourTestsHelper
{
    public static GameObject SetupChooseMeetingLocationBehaviourGameObject()
    {
        GameObject parentObj = GameObject.Instantiate(new GameObject());
        parentObj.AddComponent<NavMeshAgent>();

        GameObject chooseMeetingLocationBehaviourObj = GameObject.Instantiate(new GameObject());
        chooseMeetingLocationBehaviourObj.transform.SetParent(parentObj.transform);
        chooseMeetingLocationBehaviourObj.AddComponent<ChooseMeetingLocationBehaviour>();

        return parentObj;
    }
}
