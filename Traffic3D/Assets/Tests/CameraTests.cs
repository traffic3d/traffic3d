using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Tests")]
public class CameraTests : CommonSceneTest
{
    [UnityTest]
    public IEnumerator FrameRateTest()
    {
        yield return null;
        CameraManager cameraManager = (CameraManager)UnityEngine.Object.FindObjectOfType(typeof(CameraManager));
        Assert.AreEqual(Time.captureFramerate, cameraManager.frameRate);
    }
}
