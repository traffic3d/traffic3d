using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using System;

public class RunBenchmarkTest
{
    [SetUp]
    public void SetUpTest()
    {
        try
        {
            SocketManager.GetInstance().SetSocket(new MockSocket());
            SceneManager.LoadScene(0);
            Settings.SetBenchmark();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    [UnityTest]
    [Timeout(int.MaxValue)]
    public IEnumerator RunBenchmarkTestWithEnumeratorPasses()
    {
        yield return new WaitForSeconds(300);
    }
}
