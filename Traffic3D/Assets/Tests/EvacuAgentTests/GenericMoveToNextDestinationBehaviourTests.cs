using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;


public class GenericMoveToNextDestinationBehaviour_ReturnsFalse_WhenPedestrianIsNotAtCurrentDestination : ArrangeActAssertStrategy
{
    private GameObject pedestrianGameObject;
    private GameObject genericMoveToNextDestinationBehaviourGameObject;
    private GenericMoveToNextDestinationBehaviour genericMoveToNextDestinationBehaviour;
    private NavMeshAgent navMeshAgent;
    private Vector3 navMeshDestination;
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
        genericMoveToNextDestinationBehaviourGameObject = GenericMoveToNextDestinationBehaviourTestsHelper.SetUpGenericMoveToNextDestinationBehaviourGameObject(pedestrianGameObject);

        navMeshAgent = pedestrianGameObject.GetComponent<NavMeshAgent>();
        navMeshDestination = new Vector3(20f, 0f, 20f);
        navMeshAgent.SetDestination(navMeshDestination);

        genericMoveToNextDestinationBehaviour = genericMoveToNextDestinationBehaviourGameObject.GetComponent<GenericMoveToNextDestinationBehaviour>();
    }

    public override void Act()
    {
        actualBool = genericMoveToNextDestinationBehaviour.ShouldTriggerBehaviour();
    }

    public override void Assertion()
    {
        Assert.AreSame(navMeshAgent, genericMoveToNextDestinationBehaviourGameObject.GetComponentInParent<NavMeshAgent>());
        Assert.AreEqual(navMeshAgent.destination, genericMoveToNextDestinationBehaviourGameObject.GetComponentInParent<NavMeshAgent>().destination);
        Assert.IsFalse(actualBool);
    }
}

public class GenericMoveToNextDestinationBehaviour_ReturnsTrue_WhenPedestrianIsAtCurrentDestination : ArrangeActAssertStrategy
{
    private GameObject pedestrianGameObject;
    private GameObject genericMoveToNextDestinationBehaviourGameObject;
    private GenericMoveToNextDestinationBehaviour genericMoveToNextDestinationBehaviour;
    private NavMeshAgent navMeshAgent;
    private Vector3 navMeshDestination;
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
        genericMoveToNextDestinationBehaviourGameObject = GenericMoveToNextDestinationBehaviourTestsHelper.SetUpGenericMoveToNextDestinationBehaviourGameObject(pedestrianGameObject);

        navMeshAgent = pedestrianGameObject.GetComponent<NavMeshAgent>();
        navMeshDestination = new Vector3(0.5f, 0f, 0.5f);
        navMeshAgent.SetDestination(navMeshDestination);

        genericMoveToNextDestinationBehaviour = genericMoveToNextDestinationBehaviourGameObject.GetComponent<GenericMoveToNextDestinationBehaviour>();
    }

    public override void Act()
    {
        actualBool = genericMoveToNextDestinationBehaviour.ShouldTriggerBehaviour();
    }

    public override void Assertion()
    {
        Assert.AreSame(navMeshAgent, genericMoveToNextDestinationBehaviourGameObject.GetComponentInParent<NavMeshAgent>());
        Assert.AreEqual(navMeshAgent.destination, genericMoveToNextDestinationBehaviourGameObject.GetComponentInParent<NavMeshAgent>().destination);
        Assert.IsTrue(actualBool);
    }
}

public class GenericMoveToNextDestinationBehaviour_CorrectlySetsCurrentDestination_WhenThereIsAPedestrianPointToTravelTo : ArrangeActAssertStrategy
{
    private GameObject pedestrianGameObject;
    private GameObject genericMoveToNextDestinationBehaviourGameObject;
    private GenericMoveToNextDestinationBehaviour genericMoveToNextDestinationBehaviour;
    private GenericPathCreationBehaviour genericPathCreationBehaviour;
    private PedestrianPoint expectedPedestrianPoint;
    private PedestrianPoint[] pedestrianPoints;
    private PedestrianPoint actualPedestrianPoint;

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
        genericMoveToNextDestinationBehaviourGameObject = GenericMoveToNextDestinationBehaviourTestsHelper.SetUpGenericMoveToNextDestinationBehaviourGameObject(pedestrianGameObject);
        genericMoveToNextDestinationBehaviour = genericMoveToNextDestinationBehaviourGameObject.GetComponent<GenericMoveToNextDestinationBehaviour>();
        genericPathCreationBehaviour = genericMoveToNextDestinationBehaviourGameObject.GetComponent<GenericPathCreationBehaviour>();

        pedestrianPoints = GameObject.FindObjectsOfType<PedestrianPoint>();
        Assert.GreaterOrEqual(pedestrianPoints.Length, 2);

        genericPathCreationBehaviour.PathOfPedestrianPoints = pedestrianPoints.ToList();
        expectedPedestrianPoint = pedestrianPoints[1];
    }

    public override void Act()
    {
        genericMoveToNextDestinationBehaviour.PerformBehaviour();
        actualPedestrianPoint = genericMoveToNextDestinationBehaviour.CurrentPedestrianPointDestination;
    }

    public override void Assertion()
    {
        Assert.AreSame(expectedPedestrianPoint, actualPedestrianPoint);
    }
}

public class GenericMoveToNextDestinationBehaviour_DoesNotSetNewDestination_WhenNoDestinationsLeftInPath : ArrangeActAssertStrategy
{
    private GameObject pedestrianGameObject;
    private GameObject genericMoveToNextDestinationBehaviourGameObject;
    private GenericMoveToNextDestinationBehaviour genericMoveToNextDestinationBehaviour;
    private GenericPathCreationBehaviour genericPathCreationBehaviour;

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
        genericMoveToNextDestinationBehaviourGameObject = GenericMoveToNextDestinationBehaviourTestsHelper.SetUpGenericMoveToNextDestinationBehaviourGameObject(pedestrianGameObject);
        genericMoveToNextDestinationBehaviour = genericMoveToNextDestinationBehaviourGameObject.GetComponent<GenericMoveToNextDestinationBehaviour>();
        genericPathCreationBehaviour = genericMoveToNextDestinationBehaviourGameObject.GetComponent<GenericPathCreationBehaviour>();
        genericPathCreationBehaviour.PathOfPedestrianPoints = new List<PedestrianPoint>();
    }

    public override void Act()
    {
        genericMoveToNextDestinationBehaviour.PerformBehaviour();
    }

    public override void Assertion()
    {
        Assert.IsNull(genericMoveToNextDestinationBehaviour.CurrentPedestrianPointDestination);
    }
}

public static class GenericMoveToNextDestinationBehaviourTestsHelper
{
    public static GameObject SetUpGenericMoveToNextDestinationBehaviourGameObject(GameObject parentObject)
    {
        parentObject.AddComponent<NavMeshAgent>();
        parentObject.transform.position = new Vector3(0f, 0f, 0f);

        GameObject gameObject = GameObject.Instantiate(new GameObject());
        gameObject.AddComponent<GenericPathCreationBehaviour>();
        gameObject.AddComponent<GenericMoveToNextDestinationBehaviour>();
        gameObject.transform.SetParent(parentObject.transform);
        return gameObject;
    }
}
