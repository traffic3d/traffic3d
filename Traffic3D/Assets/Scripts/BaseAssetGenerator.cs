using System.Collections.Generic;
using UnityEngine;

/*
 * Abstracts functionality for generating meshes for asthetic elements of a scene e.g roads and buildings
 */
public abstract class BaseAssetGenerator
{
    protected MapReader map;
    protected GameObject rootParent; // all object stored under same parent (e.g "Roads" -> road1,road2....)
    
    private GameObject objectInstance; //the object instance e.g building
    private Dictionary<MapXmlWay, GameObject> parentObjectsForWays; //{Key: Way, Value: Parent_object} - Ensures all elements of a Way are all connected to the same Parent_Object

    public BaseAssetGenerator(MapReader mapReader)
    {
        map = mapReader;
        parentObjectsForWays = new Dictionary<MapXmlWay, GameObject>();
    }

    /*
        Returns the centre of an object
    */
    protected Vector3 GetCentre(MapXmlWay way)
    {
        Vector3 total = Vector3.zero;

        foreach (var id in way.NodeIDs)
        {
            total = total + map.nodes[id];
        }

        return total / way.NodeIDs.Count;
    }

    /// <summary>
    /// Store passed object under root parent. e.g store a road under root parent named "Roads"
    /// </summary>
    /// <param name="child"></param>
    protected void AddToRootParent(GameObject child)
    {
        child.transform.parent = rootParent.transform;
    }

    

    /// <summary>
    /// Procedurally generate an object from the data given in the MapXmlWay instance.
    /// </summary>
    /// <param name="way"> MapXmlWay instance</param>
    /// <param name="mat">Material to apply to the instance</param>
    /// <param name="objectName">The name of the object (i.e building name, road etc...)</param>
    /// <returns></returns>
    protected GameObject GenerateObject(MapXmlWay way, Material mat, string objectName)
    {
        // Make sure we have some name to display
        objectName = string.IsNullOrEmpty(objectName) ? "UnnameObject" : objectName;

        // Create an instance of the object and place it in the centre of its points
        objectInstance = new GameObject(objectName);
        Vector3 localOrigin = GetCentre(way);
        objectInstance.transform.position = localOrigin - map.bounds.Centre;

        // Add the mesh filter and renderer components to the object
        MeshFilter mf = objectInstance.AddComponent<MeshFilter>();
        MeshRenderer mr = objectInstance.AddComponent<MeshRenderer>();

        // Apply texture to mesh
        mr.material = mat;
        mr.sharedMaterial = mat;

        // Create the collections for the object's vertices, indices, UVs etc.
        List<Vector3> vectors = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        // Call the child class' object creation code
        mf.sharedMesh = InitializeMesh(way, localOrigin, vectors, normals, uvs, tris);

        //If Mesh not created, create using updated values
        if (mf.sharedMesh == null)
        {
            // Apply the data to the mesh
            mf.sharedMesh = new Mesh();
            mf.sharedMesh.vertices = vectors.ToArray();
            mf.sharedMesh.triangles = tris.ToArray();
            mf.sharedMesh.normals = normals.ToArray();
            mf.sharedMesh.uv = uvs.ToArray();
        }

        return objectInstance;
    }

    //Create Mesh for Object
    protected abstract Mesh InitializeMesh(MapXmlWay way, Vector3 origin, List<Vector3> vectors, List<Vector3> normals, List<Vector2> uvs, List<int> indices);
    
    //Initialize Parent Game Object with Custom Name
    protected void InitializeRootParent(string name)
    {
        rootParent = new GameObject(name);
    }


    public Dictionary<MapXmlWay, GameObject> GetWayObjects()
    {
        return parentObjectsForWays;
    }

    /// <summary>
    /// Get Parent object for a way
    /// </summary>
    /// <param name="way"></param>
    /// <returns>Parent object</returns>
    public GameObject GetParent(MapXmlWay way)
    {
        if (parentObjectsForWays.ContainsKey(way))
            return parentObjectsForWays[way];

        return null;
    }

    /// <summary>
    /// Link Way to paret object.
    /// </summary>
    /// <param name="way"></param>
    /// <param name="parent"></param>
    public void LinkWayToParent(MapXmlWay way,GameObject parent)
    {
        parentObjectsForWays.Add(way, parent);
    }

}
