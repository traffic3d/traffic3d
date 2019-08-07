using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class RedLightTests
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
    public IEnumerator RedLightOneTest()
    {

        redlight redlight = (redlight)GameObject.FindObjectOfType(typeof(redlight));

        redlight.StartCoroutine(redlight.one());

        yield return null;

        Assert.AreEqual(redlight.m.material2, redlight.m.CM);
        Assert.AreEqual(redlight.m1.material5, redlight.m1.CM);
        Assert.AreEqual(redlight.v.material2, redlight.v.CM);
        Assert.AreEqual(redlight.v1.material5, redlight.v1.CM);
        Assert.AreEqual(redlight.n.material2, redlight.n.CM);
        Assert.AreEqual(redlight.n1.material5, redlight.n1.CM);
        Assert.AreEqual(redlight.u.material2, redlight.u.CM);
        Assert.AreEqual(redlight.u1.material5, redlight.u1.CM);

        redlight.StopAllCoroutines();

    }

    [UnityTest]
    public IEnumerator RedLightTwoTest()
    {

        redlight redlight = (redlight)GameObject.FindObjectOfType(typeof(redlight));

        redlight.StartCoroutine(redlight.two());

        yield return null;

        Assert.AreEqual(redlight.m.material5, redlight.m.CM);
        Assert.AreEqual(redlight.m1.material3, redlight.m1.CM);
        Assert.AreEqual(redlight.v.material2, redlight.v.CM);
        Assert.AreEqual(redlight.v1.material5, redlight.v1.CM);
        Assert.AreEqual(redlight.n.material2, redlight.n.CM);
        Assert.AreEqual(redlight.n1.material5, redlight.n1.CM);
        Assert.AreEqual(redlight.u.material2, redlight.u.CM);
        Assert.AreEqual(redlight.u1.material5, redlight.u1.CM);

        redlight.StopAllCoroutines();

    }

    [UnityTest]
    public IEnumerator RedLightThirdTest()
    {

        redlight redlight = (redlight)GameObject.FindObjectOfType(typeof(redlight));

        redlight.StartCoroutine(redlight.third());

        yield return null;

        Assert.AreEqual(redlight.m.material2, redlight.m.CM);
        Assert.AreEqual(redlight.m1.material5, redlight.m1.CM);
        Assert.AreEqual(redlight.v.material2, redlight.v.CM);
        Assert.AreEqual(redlight.v1.material5, redlight.v1.CM);
        Assert.AreEqual(redlight.n.material2, redlight.n.CM);
        Assert.AreEqual(redlight.n1.material5, redlight.n1.CM);
        Assert.AreEqual(redlight.u.material5, redlight.u.CM);
        Assert.AreEqual(redlight.u1.material3, redlight.u1.CM);

        redlight.StopAllCoroutines();

    }

    [UnityTest]
    public IEnumerator RedLightFourTest()
    {

        redlight redlight = (redlight)GameObject.FindObjectOfType(typeof(redlight));

        redlight.StartCoroutine(redlight.four());

        yield return null;

        Assert.AreEqual(redlight.m.material2, redlight.m.CM);
        Assert.AreEqual(redlight.m1.material5, redlight.m1.CM);
        Assert.AreEqual(redlight.v.material2, redlight.v.CM);
        Assert.AreEqual(redlight.v1.material5, redlight.v1.CM);
        Assert.AreEqual(redlight.n.material5, redlight.n.CM);
        Assert.AreEqual(redlight.n1.material3, redlight.n1.CM);
        Assert.AreEqual(redlight.u.material2, redlight.u.CM);
        Assert.AreEqual(redlight.u1.material5, redlight.u1.CM);

        redlight.StopAllCoroutines();

    }

    [UnityTest]
    public IEnumerator RedLightFiveTest()
    {

        redlight redlight = (redlight)GameObject.FindObjectOfType(typeof(redlight));

        redlight.StartCoroutine(redlight.five());

        yield return null;

        Assert.AreEqual(redlight.m.material2, redlight.m.CM);
        Assert.AreEqual(redlight.m1.material5, redlight.m1.CM);
        Assert.AreEqual(redlight.v.material5, redlight.v.CM);
        Assert.AreEqual(redlight.v1.material3, redlight.v1.CM);
        Assert.AreEqual(redlight.n.material2, redlight.n.CM);
        Assert.AreEqual(redlight.n1.material5, redlight.n1.CM);
        Assert.AreEqual(redlight.u.material2, redlight.u.CM);
        Assert.AreEqual(redlight.u1.material5, redlight.u1.CM);

        redlight.StopAllCoroutines();

    }

    private void DisableLoops()
    {

        redlight redlight = (redlight)GameObject.FindObjectOfType(typeof(redlight));
        redlight.StopAllCoroutines();

    }

}
