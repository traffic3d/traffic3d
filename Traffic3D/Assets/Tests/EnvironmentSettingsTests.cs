using DigitalRuby.RainMaker;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[Category("Tests")]
public class EnvironmentSettingsTests : CommonSceneTest
{
    [SetUp]
    public override void SetUpTest()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        base.SetUpTest();
    }

    [TearDown]
    public override void TearDown()
    {
        base.TearDown();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnvironmentSettings environmentSettings = GameObject.FindObjectOfType<EnvironmentSettings>();
        environmentSettings.activeEnvironmentStateTypes.Add(EnvironmentStateType.SNOW);
        environmentSettings.activeEnvironmentStateTypes.Add(EnvironmentStateType.NIGHT);
        environmentSettings.activeEnvironmentStateTypes.Add(EnvironmentStateType.RAIN);
    }

    [UnityTest]
    public IEnumerator EnvironmentStatePriorityTest()
    {
        DisableLoops();
        yield return null;
        EnvironmentSettings environmentSettings = GameObject.FindObjectOfType<EnvironmentSettings>();
        // Night should take priority of sky box
        Assert.AreEqual(environmentSettings.nightSkyBox, RenderSettings.skybox);
        // Snow should take priority on road surfaces
        foreach (GameObject roadway in GameObject.FindGameObjectsWithTag("roadway"))
        {
            if (roadway.GetComponent<Collider>() != null)
            {
                Assert.AreEqual(environmentSettings.snowSurfaceMaterial, roadway.GetComponent<Collider>().sharedMaterial);
            }
        }
        foreach (GameObject pathway in GameObject.FindGameObjectsWithTag("pathway"))
        {
            if (pathway.GetComponent<Collider>() != null)
            {
                Assert.AreEqual(environmentSettings.snowSurfaceMaterial, pathway.GetComponent<Collider>().sharedMaterial);
            }
        }
    }

    [UnityTest]
    public IEnumerator EnvironmentStateSnowPrefabTest()
    {
        DisableLoops();
        yield return null;
        // Remove (Clone) from object as it has been cloned and we need the original name
        List<GameObject> snowObjects = GameObject.FindObjectsOfType<ParticleSystem>().Select(s => s.gameObject).Where(s => s.name.Replace("(Clone)", "").Equals("Snow")).ToList();
        Assert.IsNotEmpty(snowObjects);
        foreach (Camera cam in GameObject.FindObjectsOfType<Camera>())
        {
            Vector3 camPosition = cam.transform.position;
            Assert.IsTrue(snowObjects.Exists(g => g.transform.position.Equals(new Vector3(camPosition.x, camPosition.y + SnowEnvironmentState.snowHeight, camPosition.z) + cam.transform.forward * SnowEnvironmentState.snowLengthForward)));
        }
    }

    [UnityTest]
    public IEnumerator EnvironmentStateRainPrefabTest()
    {
        DisableLoops();
        yield return null;
        List<RainScript> rainObjects = GameObject.FindObjectsOfType<RainScript>().ToList();
        Assert.IsNotEmpty(rainObjects);
        foreach (Camera cam in GameObject.FindObjectsOfType<Camera>())
        {
            RainScript rainScript = rainObjects.Find(r => r.Camera.Equals(cam));
            Assert.IsNotNull(rainScript);
            Assert.AreEqual(cam.transform.position, rainScript.transform.position);
        }
    }
}
