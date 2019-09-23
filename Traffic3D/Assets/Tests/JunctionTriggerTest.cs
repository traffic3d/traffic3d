using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class JunctionTriggerTest
{
    [SetUp]
    public void SetUpTest()
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

    [UnityTest]
    public IEnumerator TriggerTest()
    {
        JunctionTrigger juctionTrigger = GameObject.FindObjectOfType<JunctionTrigger>();
        Vector3 position = juctionTrigger.gameObject.GetComponent<BoxCollider>().bounds.center;
        position = new Vector3(position.x, position.y + juctionTrigger.gameObject.GetComponent<BoxCollider>().bounds.size.y + 0.1f, position.z);
        GameObject emptyObject = GameObject.Instantiate(new GameObject(), position, Quaternion.identity);
        emptyObject.AddComponent<Rigidbody>();
        emptyObject.AddComponent<BoxCollider>();
        yield return new WaitForSeconds(1f);
        Assert.AreEqual("drive", emptyObject.tag);
        GameObject.Destroy(emptyObject);
    }
}
