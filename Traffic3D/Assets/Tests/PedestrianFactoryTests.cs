using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Tests")]
public class PedestrianFactoryTests : CommonSceneTest
{
    [UnityTest]
    public IEnumerator PedestrianFactorySpawnTest()
    {
        DisableLoops();
        yield return null;
        foreach(Pedestrian pedestrian in GameObject.FindObjectsOfType<Pedestrian>())
        {
            GameObject.Destroy(pedestrian);
        }
        Assert.Zero(GameObject.FindObjectsOfType<Pedestrian>().Length);
        PedestrianFactory pedestrianFactory = (PedestrianFactory)GameObject.FindObjectOfType(typeof(PedestrianFactory));
        pedestrianFactory.SpawnPedestrian();
        Assert.AreEqual(1, GameObject.FindObjectsOfType<Pedestrian>().Length);
    }
}
