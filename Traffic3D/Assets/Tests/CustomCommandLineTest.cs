using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[Category("Tests")]
public class CustomCommandLineTest : CommonSceneTest
{
    private bool originalHeadlessMode;
    private JSONConfigParser.JSONConfig originalConfig;

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        originalHeadlessMode = Settings.IsHeadlessMode();
        originalConfig = JSONConfigParser.GetConfig();
        string[] arguments = new string[] { "-JSONConfigFile", "Assets/Tests/TestFiles/test_config.json", "-RunHeadless", "true" };
        CustomCommandLineArguments.SetMockArgument(arguments);
        CustomCommandLineArguments.Run();
        yield return new EnterPlayMode();
    }

    [UnityTearDown]
    public IEnumerator UnityTearDown()
    {
        JSONConfigParser.SetConfig(originalConfig);
        Settings.SetHeadlessMode(originalHeadlessMode);
        yield return new ExitPlayMode();
    }

    [UnityTest]
    public IEnumerator JSONParserTest()
    {
        yield return null;
        DisableLoops();
        VehicleFactory vehicleFactory = (VehicleFactory)GameObject.FindObjectOfType(typeof(VehicleFactory));
        Assert.AreEqual(1, vehicleFactory.highRangeRespawnTime);
        Assert.AreEqual(1, vehicleFactory.lowRangeRespawnTime);
        Assert.AreEqual(1, vehicleFactory.maximumVehicleCount);
        Assert.AreEqual(1, vehicleFactory.slowDownVehicleRateAt);
        Assert.AreEqual(1, vehicleFactory.timeOfStartInvisibility);
        foreach (VehicleFactory.VehicleProbability vehicleProbability in vehicleFactory.vehicleProbabilities)
        {
            Assert.AreEqual(0.125, vehicleProbability.probability);
        }
        SceneManager.LoadScene("Sumo");
        yield return new WaitUntil(() => GameObject.FindObjectOfType(typeof(SumoManager)) != null);
        DisableLoops();
        SumoManager sumoManager = (SumoManager)GameObject.FindObjectOfType(typeof(SumoManager));
        Assert.AreEqual(12345, sumoManager.port);
        Assert.AreEqual("0.0.0.0", sumoManager.ip);
        SumoManager.SumoLinkControlPointObject sumoLinkControlPointObject = sumoManager.sumoControlSettings.Find(o => o.sumoLinkControlPoint == SumoLinkControlPoint.TRAFFIC_FLOW);
        Assert.AreEqual(true, sumoLinkControlPointObject.controlledBySumo);
    }

    [UnityTest]
    public IEnumerator RunHeadlessModeTest()
    {
        yield return null;
        Assert.True(Settings.IsHeadlessMode());
    }

}
