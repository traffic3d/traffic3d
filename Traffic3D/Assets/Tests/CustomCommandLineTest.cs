using NUnit.Framework;
using System.Collections;
using UnityEngine;
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
        yield return new EnterPlayMode();
    }

    [UnityTest]
    public IEnumerator JSONParserTest()
    {
        yield return null;
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
    }

    [UnityTest]
    public IEnumerator RunHeadlessModeTest()
    {
        yield return null;
        Assert.True(Settings.IsHeadlessMode());
    }
}
