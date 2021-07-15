using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class EvacuAgentCommonSceneTest : CommonSceneTest
    {
        public readonly float floatingPointTolerance = 0.005f;
        private static string friendGroupLeaderPrefabLocation = $"{EvacuAgentSceneParamaters.RESEOURCES_PREFABS_PREFIX}Pedestrian_Types/FriendGroupLeaderPedestrian";
        private static string friendGroupFollowerPrefabLocation = $"{EvacuAgentSceneParamaters.RESEOURCES_PREFABS_PREFIX}Pedestrian_Types/FriendGroupFollowerPedestrian";

        private static FriendGroupLeaderFollowerPedestrianFactory friendGroupLeaderFollowerPedestrianFactory;

        [SetUp]
        public override void SetUpTest()
        {
            try
            {
                SocketManager.GetInstance().SetSocket(new MockSocket());
                SceneManager.LoadScene("Evacu-agent_Test_Scene");
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
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.AddComponent<Pedestrian>().enabled = false;
            gameObject.AddComponent<NavMeshAgent>().isStopped = true;
            gameObject.tag = tag;
            return gameObject;
        }

        public static PedestrianPoint GetPedestrianPointFromLocation(Vector3 location)
        {
            foreach (PedestrianPoint pedestrianPoint in GameObject.FindObjectsOfType<PedestrianPoint>())
            {
                if (pedestrianPoint.GetPointLocation().Equals(location))
                {
                    return pedestrianPoint;
                }
            }

            return null;
        }

        public static GameObject CreateFriendGroupLeaderPedestrianFromResources()
        {
            GameObject pedestrianObj = SpawnGameObjectWithInactivePedestrianScript();
            GameObject friendGroupLeaderObj = GameObject.Instantiate(Resources.Load<GameObject>(friendGroupLeaderPrefabLocation));
            EvacuAgentPedestrianBase evacuAgentPedestrianBase = friendGroupLeaderObj.GetComponentInChildren<EvacuAgentPedestrianBase>();
            evacuAgentPedestrianBase.InitialisePedestrian(pedestrianObj.GetComponent<Pedestrian>());
            return friendGroupLeaderObj;
        }

        public static List<EvacuAgentPedestrianBase> SpawnFriendGroupOfEvacuAgentPedestrians(int numberInGroup)
        {
            List<EvacuAgentPedestrianBase> friendGroup = new List<EvacuAgentPedestrianBase>();

            // Minus 1 as the groups are spawned as one leader + a number of group members so we need to take away one for the leader
            int followerNumber = numberInGroup - 1;

            // Adjust the factory follower counts. Values need to be saved for resetting after logic is performed
            int initialMinumum = EvacuAgentSceneParamaters.FRIEND_GROUP_FOLLOWER_COUNT_MINIMUM;
            int initialMaximum = EvacuAgentSceneParamaters.FRIEND_GROUP_FOLLOWER_COUNT_MAXIMUM;

            EvacuAgentSceneParamaters.FRIEND_GROUP_FOLLOWER_COUNT_MINIMUM = followerNumber;
            EvacuAgentSceneParamaters.FRIEND_GROUP_FOLLOWER_COUNT_MAXIMUM = followerNumber;

            // Get the factory if the reference is null
            if(friendGroupLeaderFollowerPedestrianFactory == null)
                friendGroupLeaderFollowerPedestrianFactory = (FriendGroupLeaderFollowerPedestrianFactory)GameObject.FindObjectOfType(typeof(FriendGroupLeaderFollowerPedestrianFactory));

            for(int index = 0; index < numberInGroup; index++)
            {
                // Create a GameObject with a Pedestrian script to be passed into the factory
                Pedestrian pedestrian = SpawnGameObjectWithInactivePedestrianScript().GetComponent<Pedestrian>();
                EvacuAgentPedestrianBase evacuAgentPedestrianBase = friendGroupLeaderFollowerPedestrianFactory.CreateEvacuAgentPedestrian(pedestrian);

                // Turn off EvacuAgent pedestrian behaviours and field of view
                evacuAgentPedestrianBase.fieldOfView.StopAllCoroutines();
                evacuAgentPedestrianBase.behaviourController.isUpdateOn = false;
                evacuAgentPedestrianBase.GetComponentInChildren<BehaviourCollection>().enabled = false;

                friendGroup.Add(evacuAgentPedestrianBase);
            }

            // Reset folower values
            EvacuAgentSceneParamaters.FRIEND_GROUP_FOLLOWER_COUNT_MAXIMUM = initialMaximum;
            EvacuAgentSceneParamaters.FRIEND_GROUP_FOLLOWER_COUNT_MINIMUM = initialMinumum;

            return friendGroup;
        }

        public static void SetPosition<T>(T component, Vector3 position) where T : Component
        {
            component.transform.position = position;
        }

        public static void SetPositions<T>(List<T> components, List<Vector3> positions) where T : Component
        {
            for (int index = 0; index < components.Count; index++)
            {
                components[index].gameObject.transform.position = positions[index];
            }
        }

        public static void AssertTwoVectorsAreEqualWithinTolerance(Vector3 actualVector, Vector3 expectedVector, float tolerance)
        {
            try
            {
                Assert.That(actualVector.x, Is.EqualTo(expectedVector.x).Within(tolerance));
            }
            catch (Exception e)
            {
                Debug.LogError($"AssertTwoVectorsAreEqualWithinTolerance - Failure in X component. Message: {e}");
            }

            try
            {
                Assert.That(actualVector.y, Is.EqualTo(expectedVector.y).Within(tolerance));
            }
            catch (Exception e)
            {
                Debug.LogError($"AssertTwoVectorsAreEqualWithinTolerance - Failure in Y component. Message: {e}");
            }

            try
            {
                Assert.That(actualVector.z, Is.EqualTo(expectedVector.z).Within(tolerance));
            }
            catch (Exception e)
            {
                Debug.LogError($"AssertTwoVectorsAreEqualWithinTolerance - Failure in Z component. Message: {e}");
            }
        }
    }
}
