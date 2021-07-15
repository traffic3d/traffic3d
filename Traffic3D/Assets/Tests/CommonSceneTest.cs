using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CommonSceneTest
{
    [SetUp]
    public virtual void SetUpTest()
    {
        try
        {
            SocketManager.GetInstance().SetSocket(new MockSocket());
            SceneManager.LoadScene("DayDemo");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    [TearDown]
    public virtual void TearDown()
    {
        EventManager.Destroy();
    }

    public void DisableLoops()
    {
        TrafficLightManager trafficLightManager = (TrafficLightManager)GameObject.FindObjectOfType(typeof(TrafficLightManager));
        trafficLightManager.StopAllCoroutines();
        PythonManager.GetInstance().StopAllCoroutines();
        PedestrianFactory pedestrianFactory = (PedestrianFactory)GameObject.FindObjectOfType(typeof(PedestrianFactory));
        if (pedestrianFactory != null)
        {
            pedestrianFactory.StopAllCoroutines();
        }
        RoadNetworkManager.GetInstance().Reload();
    }
}
