using NUnit.Framework;
using System;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Tests")]
public class InvalidJSONConfigTest
{
    private bool originalHeadlessMode;
    private JSONConfigParser.JSONConfig originalConfig;

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        originalHeadlessMode = Settings.IsHeadlessMode();
        originalConfig = JSONConfigParser.GetConfig();
        yield return new EnterPlayMode();
    }

    [SetUp]
    public void SetUpTest()
    {
        try
        {
            SocketManager.GetInstance().SetSocket(new MockSocket());
            EditorSceneManager.LoadScene(0);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    [UnityTearDown]
    public IEnumerator UnityTearDown()
    {
        JSONConfigParser.SetConfig(originalConfig);
        Settings.SetHeadlessMode(originalHeadlessMode);
        yield return null;
    }

    [UnityTest]
    public IEnumerator InvalidJSONParserTest()
    {
        Assert.Throws(typeof(ArgumentException), () => JSONConfigParser.Parse("Assets/Tests/TestFiles/test_config_invalid.json"));
        yield return null;
    }
}
