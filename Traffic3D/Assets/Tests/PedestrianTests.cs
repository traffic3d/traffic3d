using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;

[Category("Tests")]
public class PedestrianTests : CommonSceneTest
{
    [UnityTest]
    [Timeout(120000)]
    public IEnumerator PedestrianFullWalkTest()
    {
        DisableLoops();
        DisableVehicles();
        foreach (Pedestrian p in GameObject.FindObjectsOfType<Pedestrian>())
        {
            GameObject.Destroy(p);
        }
        foreach (PedestrianCrossing pedestrianCrossing in PedestrianManager.GetInstance().GetPedestrianCrossings())
        {
            pedestrianCrossing.SetAllowCrossing(true);
        }
        PedestrianFactory pedestrianFactory = GameObject.FindObjectOfType<PedestrianFactory>();
        pedestrianFactory.SpawnPedestrian();
        yield return new WaitForSeconds(1);
        Pedestrian pedestrian = GameObject.FindObjectOfType<Pedestrian>();
        pedestrian.GetComponent<NavMeshAgent>().speed = 5;
        pedestrian.SetAllowCrossing(true);
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (pedestrian == null)
            {
                break;
            }
            else
            {
                pedestrian.GetComponent<NavMeshAgent>().speed = 5;
            }
        }
    }

    [UnityTest]
    public IEnumerator PedestrianAllowCrossingMaskTest()
    {
        DisableLoops();
        DisableVehicles();
        int roadAreaMask = 1 << NavMesh.GetAreaFromName(PedestrianManager.ROAD_AREA);
        int walkableAreaMask = 1 << NavMesh.GetAreaFromName(PedestrianManager.WALKABLE_AREA);
        int pedestrianCrossingAreaMask = 1 << NavMesh.GetAreaFromName(PedestrianManager.PEDESTRIAN_CROSSING_AREA);
        PedestrianFactory pedestrianFactory = GameObject.FindObjectOfType<PedestrianFactory>();
        pedestrianFactory.SpawnPedestrian();
        yield return new WaitForSeconds(1);
        Pedestrian pedestrian = GameObject.FindObjectOfType<Pedestrian>();
        pedestrian.SetAllowCrossing(true);
        NavMeshAgent navMeshAgent = pedestrian.GetComponent<NavMeshAgent>();
        navMeshAgent.speed = 0;
        yield return new WaitForSeconds(1);
        Assert.IsTrue(navMeshAgent.areaMask == (walkableAreaMask | pedestrianCrossingAreaMask) || navMeshAgent.areaMask == (roadAreaMask | walkableAreaMask | pedestrianCrossingAreaMask));
        pedestrian.SetAllowCrossing(false);
        yield return new WaitForSeconds(1);
        Assert.IsTrue(navMeshAgent.areaMask == (walkableAreaMask) || navMeshAgent.areaMask == (roadAreaMask | walkableAreaMask));
    }

    // Crossing Tests
    [UnityTest]
    public IEnumerator PedestrianCrossingEnterAndExitTest()
    {
        DisableLoops();
        DisableVehicles();
        PedestrianCrossing pedestrianCrossing = GameObject.FindObjectOfType<PedestrianCrossing>();
        Vector3 centerPosition = pedestrianCrossing.GetComponent<BoxCollider>().bounds.center;
        GameObject pedestrian = SpawnPedestrian(true);
        yield return new WaitForSeconds(1);
        pedestrian.transform.position = centerPosition;
        yield return new WaitForSeconds(1);
        Assert.IsTrue(pedestrianCrossing.pedestriansCurrentlyInCrossingArea.Contains(pedestrian.GetComponent<Pedestrian>()));
        pedestrian.transform.position = Vector3.forward * 1000;
        yield return new WaitForSeconds(1);
        Assert.IsFalse(pedestrianCrossing.pedestriansCurrentlyInCrossingArea.Contains(pedestrian.GetComponent<Pedestrian>()));
    }

    [UnityTest]
    public IEnumerator PedestrianCrossingAllowCrossingTest()
    {
        DisableLoops();
        DisableVehicles();
        PedestrianCrossing pedestrianCrossing = GameObject.FindObjectOfType<PedestrianCrossing>();
        pedestrianCrossing.SetAllowCrossing(false);
        Vector3 centerPosition = pedestrianCrossing.GetComponent<BoxCollider>().bounds.center;
        GameObject pedestrian = SpawnPedestrian(true);
        yield return new WaitForSeconds(1);
        pedestrian.transform.position = centerPosition;
        yield return new WaitForSeconds(1);
        Assert.IsFalse(pedestrian.GetComponent<Pedestrian>().GetAllowCrossing());
        yield return new WaitForSeconds(1);
        pedestrianCrossing.SetAllowCrossing(true);
        Assert.IsTrue(pedestrian.GetComponent<Pedestrian>().GetAllowCrossing());
    }

    private GameObject SpawnPedestrian(bool withoutFunctionality)
    {
        GameObject pedestrianTemplate = GameObject.FindObjectOfType<PedestrianFactory>().GetRandomPedestrian().gameObject;
        GameObject pedestrian = GameObject.Instantiate(pedestrianTemplate, new Vector3(0, 1000, 0), Quaternion.identity);
        if (withoutFunctionality)
        {
            GameObject.DestroyImmediate(pedestrian.GetComponent<NavMeshAgent>());
            pedestrian.GetComponent<Pedestrian>().enabled = false;
        }
        return pedestrian;
    }

    private void DisableVehicles()
    {
        VehicleFactory vehicleFactory = (VehicleFactory)GameObject.FindObjectOfType(typeof(VehicleFactory));
        vehicleFactory.StopAllCoroutines();
    }

}