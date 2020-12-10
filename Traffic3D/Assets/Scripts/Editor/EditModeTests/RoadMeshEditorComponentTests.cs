using System.Collections;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

[Category("Tests")]
public class RoadMeshEditorComponentTests
{
    //create parent
    GameObject parent;
    //create Road
    GameObject road;
    //create RoadWay
    RoadWay roadWay;
    //nodes
    RoadNode node1;
    RoadNode node2;
    RoadMeshUpdater updater;

    [SetUp]
    public void SetUp()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        parent = new GameObject();
        road = new GameObject();
        GameObject pathObject = new GameObject();

        road.AddComponent<MeshFilter>();
        road.AddComponent<MeshRenderer>();
        road.AddComponent<RoadMeshUpdater>();

        roadWay = pathObject.AddComponent<RoadWay>();

        //give same Parent
        road.transform.parent = parent.transform;
        roadWay.transform.parent = parent.transform;

        GameObject nodeGameObject1 = new GameObject("node1");
        GameObject nodeGameObject2 = new GameObject("node2");

        nodeGameObject1.transform.position = new Vector3(1, 1, 1);
        nodeGameObject2.transform.position = new Vector3(11, 11, 11);

        node1 = nodeGameObject1.AddComponent<RoadNode>();
        node2 = nodeGameObject2.AddComponent<RoadNode>();

        updater = road.GetComponent<RoadMeshUpdater>();
        updater.road = road;
        updater.roadWay = roadWay;
    }

    [TearDown]
    public void ResetRoadWayNodes()
    {
        roadWay.nodes.Clear();
    }

    // Mesh emty if 1 node
    [Test]
    public void EmptyWhenNoChildNode()
    {
        //update Road Mesh
        updater.UpdateRoadMesh();
        Assert.True(road.GetComponent<MeshFilter>().sharedMesh.vertexCount == 0);
    }

    // Mesh emty if 2 nodes
    [Test]
    public void EmptyWhenOneChildNode()
    {
        //Add Nodes
        roadWay.nodes.Add(node1);
        //update Road Mesh
        updater.UpdateRoadMesh();
        Assert.True(road.GetComponent<MeshFilter>().sharedMesh.vertexCount == 0);
    }

    // Does adding child nodes let road mesh update
    [Test]
    public void MeshCreatedWhenValidChildNodes()
    {
        //Add Nodes
        roadWay.nodes.Add(node1);
        roadWay.nodes.Add(node2);
        //update road mesh
        updater.UpdateRoadMesh();
        Assert.True(road.GetComponent<MeshFilter>().sharedMesh.vertexCount > 0);
    }

    // Empty Mesh when no Path Class
    [Test]
    public void EmptyWhenNoPathObject()
    {
        GameObject.DestroyImmediate(roadWay);
        //update road mesh
        updater.UpdateRoadMesh();
        Assert.True(road.GetComponent<MeshFilter>().sharedMesh.vertexCount == 0);
        Assert.True(updater.roadWay == null);
    }

}

