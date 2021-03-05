using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class EvacuAgentCommonSceneTest : CommonSceneTest
    {
        public readonly float floatingPointTolerance = 0.005f;

        [SetUp]
        public override void SetUpTest()
        {
            try
            {
                SocketManager.GetInstance().SetSocket(new MockSocket());
                SceneManager.LoadScene(2);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public void DisableVehicles()
        {
            VehicleFactory vehicleFactory = (VehicleFactory)GameObject.FindObjectOfType(typeof(VehicleFactory));
            vehicleFactory.StopAllCoroutines();
        }

        public static Pedestrian[] SpawnPedestrians(int numberOfPedestrians)
        {
            foreach (Pedestrian pedestrian in GameObject.FindObjectsOfType<Pedestrian>())
            {
                GameObject.Destroy(pedestrian);
            }

            PedestrianFactory pedestrianFactory = (PedestrianFactory)GameObject.FindObjectOfType(typeof(PedestrianFactory));

            for (int index = 0; index < numberOfPedestrians; index++)
            {
                pedestrianFactory.SpawnPedestrian();
            }

            return GameObject.FindObjectsOfType<Pedestrian>();
        }

        public void StopAllPedstrianCoroutines()
        {
            foreach (Pedestrian pedestrian in GameObject.FindObjectsOfType<Pedestrian>())
            {
                pedestrian.StopAllCoroutines();
            }
        }

        public static GameObject SpawnGameObjectWithInactivePedestrianScript(string tag = "Untagged")
        {
            GameObject gameObject = GameObject.Instantiate(new GameObject());
            gameObject.AddComponent<Pedestrian>().enabled = false;
            gameObject.tag = tag;
            return gameObject;
        }
    }
}
