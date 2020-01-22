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
            SceneManager.LoadScene(0);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}
