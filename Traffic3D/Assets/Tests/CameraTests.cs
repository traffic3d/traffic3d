using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using System.Collections;
using System;

public class CameraTests
{

    [SetUp]
    public void SetUpTest()
    {
        try
        {
            SceneManager.LoadScene(0);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    [UnityTest]
    public IEnumerator FrameRateTest()
    {

        yield return null;

        FrameRate frameRate = (FrameRate)UnityEngine.Object.FindObjectOfType(typeof(FrameRate));

        Assert.AreEqual(Time.captureFramerate, frameRate.frameRate);

    }

}
