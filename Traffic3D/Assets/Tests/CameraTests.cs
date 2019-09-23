using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class CameraTests
{
    [SetUp]
    public void SetUpTest()
    {
        try
        {
            SocketManager.GetInstance().SetSocket(new MockSocket());
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
