using System.Collections.Generic;
using System.Data;
using System.Xml; 
using UnityEngine;
using System.IO;

/// <summary>
/// Reads and an OpenStreetMap .txt file and stores data about Ways, Nodes and the Bounds of the map
/// </summary>
public class OpenStreetMapReader
{
    //Check when map data has finished uploading
    public bool finishedUploadingData;

    //Nodes: ID, <Bounds> Object : Encases the bounds of the Map
    public MapXmlBounds bounds;

    //Nodes: ID, <Node> Object : A Single Node object
    public Dictionary<ulong, MapXmlNode> nodes;

    //Nodes: ID, <Way> Object : A Collection of Nodes
    public List<MapXmlWay> ways;


	public void ImportFile(string mapFile)
    {

        // -- Get osm.txt Map File
        var txtMapFile = File.ReadAllText(mapFile);

        // -- Parse Map.txt into an xml file
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(txtMapFile);

        /* Note: .OSM XML structure 
         *      <osm>
         *          <bounds />
         *          <node>
         *              <tag />
         *         <node/>
         *      <osm/>

        
              <node><way><nd/><way/><node />
                When 2 <nd>'s are equal => Boundary i.e building
        */

        GetNodeDataFromFile(doc);
        finishedUploadingData = true;
    }

    void GetNodeDataFromFile(XmlDocument doc)
    {
        // -- Empty collections

        //Hold <Node>-tag data
        nodes = new Dictionary<ulong, MapXmlNode>();

        //Hold <Way>-tag data
        ways = new List<MapXmlWay>();

        // -- Get 3 Types of Nodes <Bounds />, <Nodes />, <Ways />

        //Set the bounds of Map to the center of the Unity Scene
        InitializeBoundary(doc.SelectSingleNode("/osm/bounds"));

        //Get All the individual Nodes
        InitializeNodes(doc.SelectNodes("/osm/node"));

        //Get each list of nodes: I.e Road, Building...
        InitializeWays(doc.SelectNodes("/osm/way"));
    }

    void InitializeWays(XmlNodeList xmlNodeList)
    {
        foreach (XmlNode node in xmlNodeList)
        {
            MapXmlWay way = new MapXmlWay(node);
            ways.Add(way);
        }
    }

    /// <summary>
    /// For every XML 'Node' Tag, creates a MapXmlNode Object & adds to dictionary
    /// </summary>
    /// <param name="xmlNodeList"></param>
    void InitializeNodes(XmlNodeList xmlNodeList)
    {
        foreach (XmlNode xmlNode in xmlNodeList)
        {
            //Create object
            MapXmlNode node = new MapXmlNode(xmlNode);
            //Add Id & Node to dictionary
            nodes[node.ID] = node;
        }
    }

    void InitializeBoundary(XmlNode xmlNode)
    {
        bounds = new MapXmlBounds(xmlNode);
    }
    
    
    public void DrawLinesBetweenNodes()
    {
        //Debugging. -> Draw lines between nodes
        foreach (MapXmlWay w in ways)
        {
            if (w.Visible)
            {Debug.Log("WAY: " +w.Name);
                Color c = Color.cyan;               // cyan for buildings
                if (!w.IsBoundary) c = Color.red; // red for roads

                for (int i = 1; i<w.NodeIDs.Count; i++)
                {
                   
                    MapXmlNode p1 = nodes[w.NodeIDs[i - 1]];
                    MapXmlNode p2 = nodes[w.NodeIDs[i]];

                    Vector3 v1 = p1 - bounds.Centre;
                    Vector3 v2 = p2 - bounds.Centre;

                    Debug.Log("Node" + (i-1)+" POSITION: "+ v1);
                    

                    Debug.DrawLine(v1, v2, c);                   
                }
            }
        }
    }
}
