using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[Category("Tests")]
public class CustomCommandLineTest : CommonSceneTest
{
    private JSONConfigParser.JSONConfig originalConfig;
    private const int randomNumberSeedTestValue = 10;

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        originalConfig = JSONConfigParser.GetConfig();
        string[] arguments = new string[] { "-JSONConfigFile", "Assets/Tests/TestFiles/test_config.json", "-RandomSeed", randomNumberSeedTestValue + "" };
        CustomCommandLineArguments.SetMockArgument(arguments);
        CustomCommandLineArguments.Run();
        yield return new EnterPlayMode();
    }

    [UnityTearDown]
    public IEnumerator UnityTearDown()
    {
        JSONConfigParser.SetConfig(originalConfig);
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
        PedestrianFactory pedestrianFactory = (PedestrianFactory)GameObject.FindObjectOfType(typeof(PedestrianFactory));
        Assert.AreEqual(1, pedestrianFactory.highRangeRespawnTime);
        Assert.AreEqual(1, pedestrianFactory.lowRangeRespawnTime);
        Assert.AreEqual(1, pedestrianFactory.maximumPedestrianCount);
        foreach (PedestrianFactory.PedestrianProbability pedestrianProbability in pedestrianFactory.pedestrianProbabilities)
        {
            Assert.AreEqual(0.25, pedestrianProbability.probability);
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
    public IEnumerator RandomNumberSeedTest()
    {
        yield return null;
        Assert.AreEqual(randomNumberSeedTestValue, RandomNumberGenerator.GetSeed());
    }
}
