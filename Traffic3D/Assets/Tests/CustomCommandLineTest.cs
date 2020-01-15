using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;

[Category("Tests")]
public class CustomCommandLineTest
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
        Assert.AreEqual(1, JSONConfigParser.GetConfig().vehicleFactoryConfig.highRangeRespawnTime);
        Assert.AreEqual(1, JSONConfigParser.GetConfig().vehicleFactoryConfig.lowRangeRespawnTime);
        Assert.AreEqual(1, JSONConfigParser.GetConfig().vehicleFactoryConfig.maximumVehicleCount);
        Assert.AreEqual(1, JSONConfigParser.GetConfig().vehicleFactoryConfig.slowDownVehicleRateAt);
        Assert.AreEqual(1, JSONConfigParser.GetConfig().vehicleFactoryConfig.timeOfStartInvisibility);
    }

    [UnityTest]
    public IEnumerator RunHeadlessModeTest()
    {
        yield return null;
        Assert.True(Settings.IsHeadlessMode());
    }
}
