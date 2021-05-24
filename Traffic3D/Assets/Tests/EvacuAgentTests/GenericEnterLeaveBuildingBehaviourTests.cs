using System.Collections;
using System.Linq;
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
    private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
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
        evacuAgentPedestrianBase = SpawnFriendGroupOfEvacuAgentPedestrians(1).First();
        genericEnterLeaveBuildingBehaviour = evacuAgentPedestrianBase.GetComponentInChildren<GenericEnterLeaveBuildingBehaviour>();
        genericEnterLeaveBuildingBehaviour.Start();

        pedestrianCollider = evacuAgentPedestrianBase.GetComponentInParent<Collider>();
        pedestrianOriginalScale = evacuAgentPedestrianBase.transform.root.localScale;

        secondsToWait = 0;
        isAbleToEnteBuilding = true;

        Assert.IsTrue(pedestrianCollider.enabled);
        Assert.AreEqual(pedestrianOriginalScale, evacuAgentPedestrianBase.transform.root.localScale);
    }

    public override void Act()
    {
        genericEnterLeaveBuildingBehaviour.SetBuildingStayParamaters(secondsToWait, isAbleToEnteBuilding);
        genericEnterLeaveBuildingBehaviour.PerformBehaviour();
    }

    private void AssetionBeforeYieldReturnInStartAgentWaitAtBuilding()
    {
        Assert.IsFalse(pedestrianCollider.enabled);
        Assert.AreEqual(new Vector3(0.0001f, 0.0001f, 0.0001f), evacuAgentPedestrianBase.transform.root.localScale);
    }

    public override void Assertion()
    {
        Assert.IsTrue(pedestrianCollider.enabled);
        Assert.AreEqual(pedestrianOriginalScale, evacuAgentPedestrianBase.transform.root.localScale);
    }
}

public static class GenericEnterLeaveBuildingBehaviourTestsHelper
{
    public static GameObject SetUpGenericEnterLeaveBuildingBehaviourGameObject(GameObject parentObject)
    {
        GameObject gameObject = GameObject.Instantiate(new GameObject());
        gameObject.AddComponent<GenericEnterLeaveBuildingBehaviour>();
        gameObject.transform.SetParent(parentObject.transform);
        return gameObject;
    }
}
