using NUnit.Framework;
using System;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Tests")]
public class InvalidJSONConfigTest : CommonSceneTest
{
    private JSONConfigParser.JSONConfig originalConfig;

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        originalConfig = JSONConfigParser.GetConfig();
        yield return new EnterPlayMode();
    }

    [UnityTearDown]
    public IEnumerator UnityTearDown()
    {
        JSONConfigParser.SetConfig(originalConfig);
        yield return null;
    }

    [UnityTest]
    public IEnumerator InvalidJSONParserTest()
    {
        Assert.Throws(typeof(ArgumentException), () => JSONConfigParser.Parse("Assets/Tests/TestFiles/test_config_invalid.json"));
        yield return null;
    }
}
