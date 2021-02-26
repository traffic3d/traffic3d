using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class InvalidCriteriaValuePopup_DequeueCanvasAndDestroyAfterDelay_SpwansCanvasWithMessage : ArrangeActAssertStrategy
{
    private InvalidCriteriaValuePopup invalidCriteriaValuePopup;
    private GameObject[] actualSpawnedCanvasGameObjects;
    private string canvasText;

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
        DisableLoops();
        canvasText = "Fake error message";
        invalidCriteriaValuePopup = InvalidCriteriaValuePopupTestHelper.SetUpInvalidCriteriaValuePopupQueueAndDictionary(canvasText);
    }

    public override void Act()
    {
        invalidCriteriaValuePopup.StartCoroutine(invalidCriteriaValuePopup.DequeueCanvasAndDestroyAfterDelay());
        actualSpawnedCanvasGameObjects = GameObject.FindGameObjectsWithTag(InvalidCriteriaValuePopupTestHelper.invalidCriterisPopupTag);
    }

    public override void Assertion()
    {
        Assert.AreEqual(1, actualSpawnedCanvasGameObjects.Length);
        Assert.AreEqual(canvasText, actualSpawnedCanvasGameObjects[0].GetComponentInChildren<Text>().text);
    }
}

public class InvalidCriteriaValuePopup_CreateCriteriaValuePopup_QueuesOnlyOneMessagePerTransform : ArrangeActAssertStrategy
{
    private InvalidCriteriaValuePopup invalidCriteriaValuePopup;
    private List<Transform> transforms;
    private Transform transform;
    private Transform actualQueueEntry;
    private KeyValuePair<Transform, string> actualDictionaryEntry;
    private string firstErrorMessage;
    private string secondErrorMessage;
    private bool firstErrorBoolToUse;
    private bool secondErrorBoolToUse;

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
        DisableLoops();
        invalidCriteriaValuePopup = InvalidCriteriaValuePopupTestHelper.GetCriteriaValuesPopupController();
        invalidCriteriaValuePopup.StopAllCoroutines();

        // Stops fixed update being called
        invalidCriteriaValuePopup.enabled = false;
        transforms = InvalidCriteriaValuePopupTestHelper.SetUpTransforms(1);
        transform = transforms[0];
        firstErrorMessage = "First error message";
        secondErrorMessage = "Second error message";
        firstErrorBoolToUse = true;
        secondErrorBoolToUse = false;
    }

    public override void Act()
    {
        invalidCriteriaValuePopup.CreateCriteriaValuePopup(firstErrorMessage, firstErrorBoolToUse, transform);
        invalidCriteriaValuePopup.CreateCriteriaValuePopup(secondErrorMessage, secondErrorBoolToUse, transform);
        actualQueueEntry = invalidCriteriaValuePopup.criteriaValuePopupQueue.Dequeue();
        actualDictionaryEntry = new KeyValuePair<Transform, string>(invalidCriteriaValuePopup.criteriaValuePopupDictionary.ElementAt(0).Key, invalidCriteriaValuePopup.criteriaValuePopupDictionary.ElementAt(0).Value);
    }

    public override void Assertion()
    {
        // We dequeue in Act() and we expect only 1 item to be added to the queue so we expect 0 after dequeue
        Assert.AreEqual(0, invalidCriteriaValuePopup.criteriaValuePopupQueue.Count);
        Assert.AreEqual(1, invalidCriteriaValuePopup.criteriaValuePopupDictionary.Count);
        Assert.AreSame(transform, actualQueueEntry);
        StringAssert.Contains(firstErrorMessage, actualDictionaryEntry.Value);
        StringAssert.Contains(firstErrorBoolToUse.ToString(), actualDictionaryEntry.Value);
        StringAssert.DoesNotContain(secondErrorMessage, actualDictionaryEntry.Value);
        StringAssert.DoesNotContain(secondErrorBoolToUse.ToString(), actualDictionaryEntry.Value);
    }
}

public class InvalidCriteriaValuePopup_DequeueCanvasAndDestroyAfterDelay_DestroysCanvasAfterTime : ArrangeActAssertStrategy
{
    private InvalidCriteriaValuePopup invalidCriteriaValuePopup;
    private string canvasText;

    [UnityTest]
    public override IEnumerator PerformTest()
    {
        yield return null;
        Arrange();
        Act();
        yield return new WaitForSeconds(6);
        Assertion();
    }

    public override void Arrange()
    {
        DisableLoops();
        canvasText = "Fake error message";
        invalidCriteriaValuePopup = InvalidCriteriaValuePopupTestHelper.SetUpInvalidCriteriaValuePopupQueueAndDictionary(canvasText);
    }

    public override void Act()
    {
        invalidCriteriaValuePopup.StartCoroutine(invalidCriteriaValuePopup.DequeueCanvasAndDestroyAfterDelay());
    }

    public override void Assertion()
    {
        Assert.AreEqual(0, GameObject.FindGameObjectsWithTag(InvalidCriteriaValuePopupTestHelper.invalidCriterisPopupTag).Length);
    }
}

public static class InvalidCriteriaValuePopupTestHelper
{
    public static readonly string invalidCriterisPopupTag = "criteriaValueError";

    public static InvalidCriteriaValuePopup GetCriteriaValuesPopupController()
    {
        InvalidCriteriaValuePopup invalidCriteriaValuePopup = GameObject.FindObjectOfType<InvalidCriteriaValuePopup>();
        invalidCriteriaValuePopup.criteriaValuePopupDictionary.Clear();
        invalidCriteriaValuePopup.criteriaValuePopupQueue.Clear();
        return invalidCriteriaValuePopup;
    }

    public static Queue<Transform> SetUpTransformQueue(List<Transform> transforms)
    {
        Queue<Transform> transformQueue = new Queue<Transform>();

        foreach(Transform transform in transforms)
        {
            transformQueue.Enqueue(transform);
        }

        return transformQueue;
    }

    public static List<Transform> SetUpTransforms(int numberOfTransforms)
    {
        List<Transform> gameObjects = new List<Transform>();

        for(int index = 0; index < numberOfTransforms; index++)
        {
            gameObjects.Add(GameObject.Instantiate(new GameObject().transform));
        }

        return gameObjects;
    }

    public static Dictionary<Transform, string> SetUpTransformStringDictionary(List<KeyValuePair<Transform, string>> dictionaryValues)
    {
        Dictionary<Transform, string> transformStringDictionary = new Dictionary<Transform, string>();

        foreach (KeyValuePair<Transform, string> keyValuePair in dictionaryValues)
        {
            transformStringDictionary.Add(keyValuePair.Key, keyValuePair.Value);
        }

        return transformStringDictionary;
    }

    public static Canvas SetUpFakeCanvas(string fakeCanvasText)
    {
        Canvas fakeCanvas = GameObject.Instantiate(new Canvas());
        fakeCanvas.gameObject.AddComponent<Text>();
        Text text = fakeCanvas.GetComponent<Text>();
        text.text = fakeCanvasText;

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.SetParent(fakeCanvas.transform);

        return fakeCanvas;
    }

    public static InvalidCriteriaValuePopup SetUpInvalidCriteriaValuePopupQueueAndDictionary(string canvasText)
    {
        InvalidCriteriaValuePopup invalidCriterisPopup = GetCriteriaValuesPopupController();
        List<Transform> transforms = SetUpTransforms(1);
        Transform transform = transforms[0];

        List<KeyValuePair<Transform, string>> listOfTransformdStringPairs = new List<KeyValuePair<Transform, string>>()
        {
            new KeyValuePair<Transform, string>(transform, canvasText)
        };

        Dictionary<Transform, string> transformStringDictionary = SetUpTransformStringDictionary(listOfTransformdStringPairs);

        invalidCriterisPopup.criteriaValuePopupDictionary = transformStringDictionary;
        invalidCriterisPopup.criteriaValuePopupQueue = SetUpTransformQueue(transforms);

        return invalidCriterisPopup;
    }
}
