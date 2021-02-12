using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class EvacuAgentCommonSceneTest : CommonSceneTest
    {
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
    }
}
