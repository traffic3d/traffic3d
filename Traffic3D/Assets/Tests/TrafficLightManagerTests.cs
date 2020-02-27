using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Tests")]
public class TrafficLightManagerTests : CommonSceneTest
{
    [UnityTest]
    public IEnumerator TrafficLightFirstEventTest()
    {
        DisableLoops();
        TrafficLightManager trafficLightManager = (TrafficLightManager)GameObject.FindObjectOfType(typeof(TrafficLightManager));
        trafficLightManager.StartCoroutine(trafficLightManager.FirstEvent());
        yield return null;
        foreach (TrafficLight trafficLight in trafficLightManager.GetTrafficLights())
        {
            Assert.AreEqual(TrafficLight.LightColour.RED, trafficLight.GetCurrentLightColour());
        }
        trafficLightManager.StopAllCoroutines();
    }

    [UnityTest]
    public IEnumerator TrafficLightFireEventTest()
    {
        DisableLoops();
        TrafficLightManager trafficLightManager = (TrafficLightManager)GameObject.FindObjectOfType(typeof(TrafficLightManager));
        foreach (int i in trafficLightManager.demoOrder)
        {
            trafficLightManager.StartCoroutine(trafficLightManager.FireEvent(i + ""));
            yield return new WaitForSeconds(6);
            CheckTrafficLightIsGreen(i + "");
        }
        trafficLightManager.StopAllCoroutines();
    }

    [UnityTest]
    public IEnumerator TrafficLightNodeTest()
    {
        DisableLoops();
        yield return null;
        TrafficLightManager trafficLightManager = (TrafficLightManager)GameObject.FindObjectOfType(typeof(TrafficLightManager));
        GameObject pathObject = new GameObject("TestPath", typeof(Path));
        GameObject node1 = new GameObject("1");
        GameObject node2 = new GameObject("2");
        GameObject trafficLight = new GameObject("TrafficLight", typeof(TrafficLight));
        pathObject = GameObject.Instantiate(pathObject);
        node1 = GameObject.Instantiate(node1, pathObject.transform);
        node2 = GameObject.Instantiate(node2, pathObject.transform);
        trafficLight = GameObject.Instantiate(trafficLight);

        trafficLightManager.trafficLights = new TrafficLight[1];
        trafficLightManager.trafficLights[0] = trafficLight.GetComponent<TrafficLight>();
        trafficLightManager.trafficLights[0].stopNodes.Add(node2.transform);

        Assert.False(trafficLightManager.IsStopNode(node1.transform));
        Assert.True(trafficLightManager.IsStopNode(node2.transform));

        Assert.AreEqual(trafficLightManager.trafficLights[0], trafficLightManager.GetTrafficLightFromStopNode(node2.transform));

    }

    private void CheckTrafficLightIsGreen(string id)
    {
        foreach (TrafficLight trafficLight in TrafficLightManager.GetInstance().GetTrafficLights())
        {
            if (trafficLight.GetTrafficLightId().Equals(id))
            {
                Assert.AreEqual(TrafficLight.LightColour.GREEN, trafficLight.GetCurrentLightColour());
            }
            else
            {
                Assert.AreEqual(TrafficLight.LightColour.RED, trafficLight.GetCurrentLightColour());
            }
        }
    }
}
