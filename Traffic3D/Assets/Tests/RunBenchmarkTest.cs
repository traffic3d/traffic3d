using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class RunBenchmarkTest : CommonSceneTest
{
    [SetUp]
    public override void SetUpTest()
    {
        Settings.SetBenchmark();
        base.SetUpTest();
    }

    [UnityTest]
    [Timeout(int.MaxValue)]
    public IEnumerator RunBenchmarkTestWithEnumeratorPasses()
    {
        yield return new WaitForSeconds(300);
    }
}
