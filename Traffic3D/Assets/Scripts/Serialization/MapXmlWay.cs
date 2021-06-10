using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;

public class MapXmlWay
{
    //<way id="21657981" visible="true" version="17" changeset="67545613" timestamp="2019-02-25T11:41:31Z" user="brianboru" uid="9065">
    public ulong ID { get; private set; }
    public bool Visible { get; private set; }
    //List of child nodes <nd />
    public List<ulong> NodeIDs { get; private set; }
    public bool IsBoundary { get; private set; }
    //Stores height of entire building
    public float buildingHeight { get; private set; }
    public bool isBuilding { get; private set; }
    public bool IsRoad { get; private set; }
    public string Name { get; private set; }
    // Road Specific
    public string RoadType { get; private set; }
    public int Lanes { get; private set; }
    public int ForwardLanes { get; private set; }
    public int BackwardLanes { get; private set; }
    public bool IsOneWay { get; private set; }

    public Dictionary<string, string> Tags { get; private set; }


    //Development purposes: Help track what types of "Highways" are in the scene
    static Dictionary<String, int> Highways = new Dictionary<string, int>();
    public void PrintDictionary()
    {
        foreach (KeyValuePair<String, int> kvp in Highways)
        {
            //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            Debug.Log(kvp.Key + " -- " + kvp.Value);
        }
    }

    public MapXmlWay(XmlNode node)
    {
        // Get each of the attribute values from the xml 'Way' Tag
        // I.e  <way id="24989638" visible="true" version="5" changeset="67999144">

        //List of all child node ref ID (<nd ref="" />) 
        NodeIDs = new List<ulong>();

        //Default values
        buildingHeight = 7.0f;
        Lanes = 1;
        ForwardLanes = 1;
        BackwardLanes = 0;
        Name = "";

        // Get the data from the attributes
        ID = GetAttribute<ulong>("id", node.Attributes);
        Visible = GetAttribute<bool>("visible", node.Attributes);


        // Get all child nodes named 'nd'
        XmlNodeList listOfChildNodes = node.SelectNodes("nd");

        //Loop through each child node
        foreach (XmlNode childNode in listOfChildNodes)
        {
            //Get Ref ID from node, as uLong
            ulong refNo = GetAttribute<ulong>("ref", childNode.Attributes);
            NodeIDs.Add(refNo);
        }

        //IF(Have Multiple Child Nodes <nd .../>)
        if (NodeIDs.Count > 1)
        {
            //IF: First & Last node have same RefID => IsBoundary == true
            IsBoundary = NodeIDs[0] == NodeIDs[NodeIDs.Count - 1];
        }

        //-----List of tags

        // Read the tags <way>...<tag k="building" v="apartments" />...</way>
        XmlNodeList ndNodeTags = node.SelectNodes("tag");
        Tags = new Dictionary<string, string>();

        // Will Loop Through Each Tag (Attribute) and do something specific 
        foreach (XmlNode tag in ndNodeTags)
        {
            // Add all tags to dictionary
            string node_attribute = GetAttribute<string>("k", tag.Attributes);
            Tags[node_attribute] = GetAttribute<string>("v", tag.Attributes);
            if (node_attribute == OpenStreetMapTagName.highwayTag)
            {
                string tagValue = GetAttribute<string>("v", tag.Attributes);

                RoadType = tagValue;

                //A value of "ROAD" implies the road type was not known during the mapping process. Therefore, it's likely a tag we to ignore.
                if (tagValue != OpenStreetMapTagName.footwayTag && tagValue != OpenStreetMapTagName.stepsTag &&
                    tagValue != OpenStreetMapTagName.cyclewayTag && tagValue != OpenStreetMapTagName.pedestrianTag &&
                    tagValue != OpenStreetMapTagName.constructionTag && tagValue != OpenStreetMapTagName.serviceTag &&
                    tagValue != OpenStreetMapTagName.motorwayTag && tagValue != OpenStreetMapTagName.trackTag &&
                    tagValue != OpenStreetMapTagName.pathTag)
                {
                    IsRoad = true;
                }

            }
            else if (node_attribute == OpenStreetMapTagName.buildingLevelsTag || node_attribute == OpenStreetMapTagName.buildingHeightTag)
            {
                //Building height is recorded in floors. 1 floor == ~4m
                buildingHeight = GetAttribute<float>("v", tag.Attributes) * 6.2f;
            }
            else if (node_attribute == OpenStreetMapTagName.lanesTag)
            {
                Lanes = GetAttribute<int>("v", tag.Attributes);
            }
            else if (node_attribute == OpenStreetMapTagName.nameTag)
            {
                Name = GetAttribute<string>("v", tag.Attributes);
            }
            else if (node_attribute == OpenStreetMapTagName.buildingTag)
            {
                isBuilding = Utils.IsTruthy(GetAttribute<string>("v", tag.Attributes));
            }
            else if (node_attribute == OpenStreetMapTagName.onewayTag)
            {
                IsOneWay = Utils.IsTruthy(GetAttribute<string>("v", tag.Attributes));
            }

        }

        // Check road lanes
        if (IsRoad)
        {
            if (IsOneWay)
            {
                ForwardLanes = Lanes;
                BackwardLanes = 0;
            }
            else
            {
                ForwardLanes = Lanes;
                BackwardLanes = Lanes;
                if (Tags.ContainsKey(OpenStreetMapTagName.lanesForwardTag))
                {
                    ForwardLanes = int.Parse(Tags[OpenStreetMapTagName.lanesForwardTag]);
                }
                if (Tags.ContainsKey(OpenStreetMapTagName.lanesBackwardTag))
                {
                    BackwardLanes = int.Parse(Tags[OpenStreetMapTagName.lanesBackwardTag]);
                }
            }
        }

    }

    protected T GetAttribute<T>(string attrName, XmlAttributeCollection attributes)
    {
        //strValue = the value of our attribute (XML Tag stores this value as a string.)
        string strValue = attributes[attrName].Value;
        //Returning value as the required data type (E.g would return a String-Id as uLong-Id)
        return (T)Convert.ChangeType(strValue, typeof(T));
    }
}
