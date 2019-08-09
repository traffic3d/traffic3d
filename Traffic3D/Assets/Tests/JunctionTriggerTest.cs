using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
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

        TRIGGERjunction tRIGGERjunction = GameObject.FindObjectOfType<TRIGGERjunction>();

        Vector3 position = tRIGGERjunction.gameObject.GetComponent<BoxCollider>().bounds.center;

        position = new Vector3(position.x, position.y + tRIGGERjunction.gameObject.GetComponent<BoxCollider>().bounds.size.y + 0.1f, position.z);

        GameObject emptyObject = GameObject.Instantiate(new GameObject(), position, Quaternion.identity);
        emptyObject.AddComponent<Rigidbody>();
        emptyObject.AddComponent<BoxCollider>();

        yield return new WaitForSeconds(1f);

        Assert.AreEqual("drive", emptyObject.tag);

        GameObject.Destroy(emptyObject);

    }

}