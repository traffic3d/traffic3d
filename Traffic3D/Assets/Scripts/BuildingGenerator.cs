using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates Building Meshes and stores all buildings under same parent gameObject
/// </summary>
public class BuildingGenerator : BaseAssetGenerator
{
    private Material building_material;
    public int buildingsCreated = 0;

    public BuildingGenerator(OpenStreetMapReader mapReader, Material buildingMaterial) : base(mapReader)
    {
        building_material = buildingMaterial;
        InitializeRootParent("Buildings");
    }

    /// <summary>
    /// Loop through all ways, and generate mesh if building
    /// </summary>
    public void GenerateBuildings()
    {

        // Iterate through ways...
        foreach (var way in osmMapReader.ways)
        {
            // if building...
            if (way.isBuilding && way.NodeIDs.Count > 1)
            {
                // Create Meshes for faces of building
                GameObject wayObject = GenerateObject(way, building_material, "Building");
                AddToRootParent(wayObject);
                buildingsCreated++;
            }
        }
    }

    /// <summary>
    /// Generate mesh for building
    /// </summary>
    /// <param name="way">MapXmlWay instance</param>
    /// <param name="origin">The origin of the structure</param>
    /// <param name="vectors">The vectors (vertices) list</param>
    /// <param name="normals">The normals list</param>
    /// <param name="uvs">The UVs list</param>
    /// <param name="tris">The triangles list</param>
    protected override Mesh InitializeMesh(MapXmlWay way, Vector3 origin, List<Vector3> vectors, List<Vector3> normals, List<Vector2> uvs, List<int> tris)
    {
        // Get Height of current building
        float buildingHeight = way.buildingHeight;

        // Get the centre of the roof
        Vector3 oTop = new Vector3(0, buildingHeight, 0);

        // First vector is the middle point in the roof
        vectors.Add(oTop); // Holds vectors
        normals.Add(Vector3.up); // holds normals
        uvs.Add(new Vector2(0.5f, 0.5f)); // holds UVs


        // Loop through all nodes in building
        for (int i = 0; i < (way.NodeIDs.Count - 1); i++)
        {
            MapXmlNode currentNodeLocation = osmMapReader.nodes[way.NodeIDs[i]]; // Current Nodes' Location
            MapXmlNode nextNodeLocation = osmMapReader.nodes[way.NodeIDs[i + 1]]; // Next Nodes' Location
            // 4 Points of a single face of building
            Vector3 v1 = currentNodeLocation - origin; // Bottom-left
            Vector3 v2 = nextNodeLocation - origin; // Bottom-right
            Vector3 v3 = v1 + new Vector3(0, buildingHeight, 0); // Top-left
            Vector3 v4 = v2 + new Vector3(0, buildingHeight, 0); // Top-right
            // Add vectors for face to list
            vectors.Add(v1);
            vectors.Add(v2);
            vectors.Add(v3);
            vectors.Add(v4);
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            // Add normals for face to list
            normals.Add(-Vector3.forward);
            normals.Add(-Vector3.forward);
            normals.Add(-Vector3.forward);
            normals.Add(-Vector3.forward);
            // Index each vector
            int index1, index2, index3, index4;
            index1 = vectors.Count - 4;
            index2 = vectors.Count - 3;
            index3 = vectors.Count - 2;
            index4 = vectors.Count - 1;
            // first triangle (v1, v3, v2)
            tris.Add(index1);
            tris.Add(index3);
            tris.Add(index2);
            // second triangle (v2, v3, v1) - Inverse of first
            tris.Add(index2);
            tris.Add(index3);
            tris.Add(index1);
            // third triangle (v3, v4, v2)
            tris.Add(index3);
            tris.Add(index4);
            tris.Add(index2);
            // fourth triangle (v2, v4, v3) -- Inverse of third
            tris.Add(index2);
            tris.Add(index4);
            tris.Add(index3);
            // top Roof
            // roof triangle
            tris.Add(0); //12
            tris.Add(index3);
            tris.Add(index4);
            // roof triangle -- Inverse
            tris.Add(index4);
            tris.Add(index3);
            tris.Add(0); //17
        }// End for
        return null;
    }

}
