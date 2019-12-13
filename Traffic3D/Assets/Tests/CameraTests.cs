using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[Category("Tests")]
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
        CameraManager cameraManager = (CameraManager)UnityEngine.Object.FindObjectOfType(typeof(CameraManager));
        Assert.AreEqual(Time.captureFramerate, cameraManager.frameRate);
    }
}
