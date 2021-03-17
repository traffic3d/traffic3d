using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;

public class GenericEnterLeaveBuildingBehaviour_ShouldTriggerBehaviour_ReturnsTrue_WhenEnterBuildingCoolDownIsNotActive : ArrangeActAssertStrategy
{
    private GameObject pedestrianGameObject;
    private GameObject genericEnterLeaveBuildingBehaviourGameObject;
    private GenericEnterLeaveBuildingBehaviour genericEnterLeaveBuildingBehaviour;
    private int secondsToWait;
    private bool isAbleToEnteBuilding;
    private bool actualBool;

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
        pedestrianGameObject = SpawnGameObjectWithInactivePedestrianScript();
        genericEnterLeaveBuildingBehaviourGameObject = GenericEnterLeaveBuildingBehaviourTestsHelper.SetUpGenericEnterLeaveBuildingBehaviourGameObject(pedestrianGameObject);
        genericEnterLeaveBuildingBehaviour = genericEnterLeaveBuildingBehaviourGameObject.GetComponent<GenericEnterLeaveBuildingBehaviour>();

        secondsToWait = 0;
        isAbleToEnteBuilding = true;
    }

    public override void Act()
    {
        genericEnterLeaveBuildingBehaviour.SetBuildingStayParamaters(secondsToWait, isAbleToEnteBuilding);
        actualBool = genericEnterLeaveBuildingBehaviour.ShouldTriggerBehaviour();
    }

    public override void Assertion()
    {
        Assert.IsTrue(actualBool);
    }
}

public class GenericEnterLeaveBuildingBehaviour_ShouldTriggerBehaviour_ReturnsFalse_WhenIsAbleToEnterBuilding_AndEnterBuildingCoolDownIsActive : ArrangeActAssertStrategy
{
    private GameObject pedestrianGameObject;
    private GameObject genericEnterLeaveBuildingBehaviourGameObject;
    private GenericEnterLeaveBuildingBehaviour genericEnterLeaveBuildingBehaviour;
    private int secondsToWait;
    private bool isAbleToEnteBuilding;
    private bool actualBool;

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
        pedestrianGameObject = SpawnGameObjectWithInactivePedestrianScript();
        genericEnterLeaveBuildingBehaviourGameObject = GenericEnterLeaveBuildingBehaviourTestsHelper.SetUpGenericEnterLeaveBuildingBehaviourGameObject(pedestrianGameObject);
        genericEnterLeaveBuildingBehaviour = genericEnterLeaveBuildingBehaviourGameObject.GetComponent<GenericEnterLeaveBuildingBehaviour>();

        secondsToWait = 0;
        isAbleToEnteBuilding = true;
    }

    public override void Act()
    {
        genericEnterLeaveBuildingBehaviour.StartCoroutine(genericEnterLeaveBuildingBehaviour.StartEnterBuildingCooldown());
        genericEnterLeaveBuildingBehaviour.SetBuildingStayParamaters(secondsToWait, isAbleToEnteBuilding);
        actualBool = genericEnterLeaveBuildingBehaviour.ShouldTriggerBehaviour();
    }

    public override void Assertion()
    {
        Assert.IsFalse(actualBool);
    }
}

public class GenericEnterLeaveBuildingBehaviour_ShouldTriggerBehaviour_EnterBuildingCoolDownResetsAfterTime : ArrangeActAssertStrategy
{
    private GameObject pedestrianGameObject;
    private GameObject genericEnterLeaveBuildingBehaviourGameObject;
    private GenericEnterLeaveBuildingBehaviour genericEnterLeaveBuildingBehaviour;
    private int secondsToWait;
    private int secondsToWaitAfterTriggeringCoolDown;
    private bool isAbleToEnteBuilding;
    private bool actualBool;

    [UnityTest]
    public override IEnumerator PerformTest()
    {
        Arrange();
        Act();
        yield return new WaitForSeconds(secondsToWaitAfterTriggeringCoolDown);
        Assertion();
    }

    public override void Arrange()
    {
        pedestrianGameObject = SpawnGameObjectWithInactivePedestrianScript();
        genericEnterLeaveBuildingBehaviourGameObject = GenericEnterLeaveBuildingBehaviourTestsHelper.SetUpGenericEnterLeaveBuildingBehaviourGameObject(pedestrianGameObject);
        genericEnterLeaveBuildingBehaviour = genericEnterLeaveBuildingBehaviourGameObject.GetComponent<GenericEnterLeaveBuildingBehaviour>();

        secondsToWait = 0;
        isAbleToEnteBuilding = true;
        secondsToWaitAfterTriggeringCoolDown = 6;
    }

    public override void Act()
    {
        genericEnterLeaveBuildingBehaviour.StartCoroutine(genericEnterLeaveBuildingBehaviour.StartEnterBuildingCooldown());
        genericEnterLeaveBuildingBehaviour.SetBuildingStayParamaters(secondsToWait, isAbleToEnteBuilding);
        actualBool = genericEnterLeaveBuildingBehaviour.ShouldTriggerBehaviour();
        Assert.IsFalse(actualBool);
    }

    public override void Assertion()
    {
        genericEnterLeaveBuildingBehaviour.SetBuildingStayParamaters(secondsToWait, isAbleToEnteBuilding);
        actualBool = genericEnterLeaveBuildingBehaviour.ShouldTriggerBehaviour();
        Assert.IsTrue(actualBool);
    }
}

public class GenericEnterLeaveBuildingBehaviour_PerformBehaviour_CorrectlyAltersPedestrianForGivenTime : ArrangeActAssertStrategy
{
    private GameObject pedestrianGameObject;
    private Pedestrian pedestrian;
    private GameObject genericEnterLeaveBuildingBehaviourGameObject;
    private GenericEnterLeaveBuildingBehaviour genericEnterLeaveBuildingBehaviour;
    private Vector3 pedestrianOriginalScale;
    private Collider pedestrianCollider;
    private int secondsToWait;
    private bool isAbleToEnteBuilding;

    [UnityTest]
    public override IEnumerator PerformTest()
    {
        Arrange();
        yield return null;
        Act();
        AssetionBeforeYieldReturnInStartAgentWaitAtBuilding();
        yield return new WaitForSeconds(0.1f);
        Assertion();
    }

    public override void Arrange()
    {
        pedestrianGameObject = SpawnGameObjectWithInactivePedestrianScript();
        pedestrian = pedestrianGameObject.GetComponent<Pedestrian>();
        pedestrianCollider = pedestrianGameObject.GetComponent<Collider>();
        pedestrianOriginalScale = pedestrian.transform.localScale;

        genericEnterLeaveBuildingBehaviourGameObject = GenericEnterLeaveBuildingBehaviourTestsHelper.SetUpGenericEnterLeaveBuildingBehaviourGameObject(pedestrianGameObject);
        genericEnterLeaveBuildingBehaviour = genericEnterLeaveBuildingBehaviourGameObject.GetComponent<GenericEnterLeaveBuildingBehaviour>();

        secondsToWait = 0;
        isAbleToEnteBuilding = true;

        Assert.IsTrue(pedestrianCollider.enabled);
        Assert.AreEqual(pedestrianOriginalScale, pedestrian.transform.localScale);
    }

    public override void Act()
    {
        genericEnterLeaveBuildingBehaviour.SetBuildingStayParamaters(secondsToWait, isAbleToEnteBuilding);
        genericEnterLeaveBuildingBehaviour.PerformBehaviour();
    }

    private void AssetionBeforeYieldReturnInStartAgentWaitAtBuilding()
    {
        Assert.IsFalse(pedestrianCollider.enabled);
        Assert.AreEqual(new Vector3(0.0001f, 0.0001f, 0.0001f), pedestrian.transform.localScale);
    }

    public override void Assertion()
    {
        Assert.IsTrue(pedestrianCollider.enabled);
        Assert.AreEqual(pedestrianOriginalScale, pedestrian.transform.localScale);
    }
}

public static class GenericEnterLeaveBuildingBehaviourTestsHelper
{
    public static GameObject SetUpGenericEnterLeaveBuildingBehaviourGameObject(GameObject parentObject)
    {
        parentObject.AddComponent<NavMeshAgent>();

        GameObject gameObject = GameObject.Instantiate(new GameObject());
        gameObject.AddComponent<GenericEnterLeaveBuildingBehaviour>();
        gameObject.transform.SetParent(parentObject.transform);
        return gameObject;
    }
}
