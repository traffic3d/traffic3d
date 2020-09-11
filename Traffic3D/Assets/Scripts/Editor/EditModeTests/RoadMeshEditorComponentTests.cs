using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Tests")]
public class RoadMeshEditorComponentTests
{
    //create parent
    GameObject parent;
    //create Road
    GameObject road;
    //create Path
    GameObject path;
    //nodes
    GameObject node1;
    GameObject node2;
    RoadMeshUpdater updater;

    [SetUp]
    public void SetUp()
    {
        parent = new GameObject();
        road = new GameObject();
        path = new GameObject();

        road.AddComponent<MeshFilter>();
        road.AddComponent<MeshRenderer>();
        road.AddComponent<RoadMeshUpdater>();

        path.AddComponent<Path>();

        //give same Parent
        road.transform.parent = parent.transform;
        path.transform.parent = parent.transform;

        node1 = new GameObject("node1");
        node2 = new GameObject("node2");

        node1.transform.position = new Vector3(1, 1, 1);
        node2.transform.position = new Vector3(11, 11, 11);

        updater = road.GetComponent<RoadMeshUpdater>();
        updater.road = road;
        updater.pathObject = path;
    }


    // Mesh emty if 1 node
    [Test]
    public void EmptyWhenNoChildNode()
    {
          
        //update list
        path.GetComponent<Path>().SetNodes();
        //update Road Mesh
        updater.UpdateRoadMesh();

        Assert.True(road.GetComponent<MeshFilter>().sharedMesh.vertexCount == 0);
    }

    // Mesh emty if 2 nodes
    [Test]
    public void EmptyWhenOneChildNode()
    {
        //Add Children
        node1.transform.parent = path.transform;

        //update list
        path.GetComponent<Path>().SetNodes();
        //update Road Mesh
        updater.UpdateRoadMesh();

        Assert.True(road.GetComponent<MeshFilter>().sharedMesh.vertexCount == 0);
    }

    // Does adding child nodes let road mesh update
    [Test]
    public void MeshCreatedWhenValidChildNodes()
    {
        //Add Children
        node1.transform.parent = path.transform;
        node2.transform.parent = path.transform;

        //update list
        path.GetComponent<Path>().SetNodes();
        //update road mesh
        updater.UpdateRoadMesh();

        Assert.True(road.GetComponent<MeshFilter>().sharedMesh.vertexCount > 0);
    }

    // Empty Mesh when no Path Class
    [Test]
    public void EmptyWhenNoPathObject()
    {
          
        GameObject.DestroyImmediate(path);
            
        //update road mesh
        updater.UpdateRoadMesh();

        Assert.True(road.GetComponent<MeshFilter>().sharedMesh.vertexCount == 0);
        Assert.True(updater.pathObject == null);
    }

}

