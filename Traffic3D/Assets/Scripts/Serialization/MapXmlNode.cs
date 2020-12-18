using System;
using System.Xml;
using UnityEngine;


public class MapXmlNode
{
    // <node id = "27457906" visible="true" version="17" changeset="67050987" timestamp="2019-02-09T14:57:21Z" user="mrpacmanmap" uid="563773" lat="52.5110832" lon="-2.0816813">
    public ulong ID { get; private set; }
    public float X { get; private set; }
    public float Y { get; private set; }
    public float Latitude { get; private set; }
    public float Longitude { get; private set; }
    public bool hasTrafficLight = false;
    public string amenity = null;
    public string emergency = null;

    /// <summary>
    /// Convert xmlNode into Vector3
    /// </summary>
    /// <param name="node">Node to be converted</param>
    public static implicit operator Vector3(MapXmlNode node)
    {
        return new Vector3(node.X, 0, node.Y);
    }


    public MapXmlNode(XmlNode node)
    {

        // Get each of the attribute values from the xml 'Node' Tag
        // I.e <node id="27457906" ... lat="52.5110832" lon="-2.0816813">
        ID = GetAttribute<ulong>("id", node.Attributes);
        Latitude = GetAttribute<float>("lat", node.Attributes);
        Longitude = GetAttribute<float>("lon", node.Attributes);

        // Calculate the position in Unity units (Convert double to float)
        X = (float)LatLonConverter.lonToX(Longitude);
        Y = (float)LatLonConverter.latToY(Latitude);

        XmlNodeList ndNodeTags = node.SelectNodes("tag");

        foreach (XmlNode tag in ndNodeTags)
        {

            string node_attribute = GetAttribute<string>("k", tag.Attributes);

            //Flag if road has traffic lights
            if (node_attribute == OpenStreetMapTagName.highwayTag)
            {
                if (GetAttribute<string>("v", tag.Attributes) == OpenStreetMapTagName.trafficSignalsTag)
                    hasTrafficLight = true;
            }
            if (node_attribute == OpenStreetMapTagName.trafficSignalsTag)
            {
                if (GetAttribute<string>("v", tag.Attributes) == OpenStreetMapTagName.signalTag)
                    hasTrafficLight = true;
            }
            if (node_attribute == OpenStreetMapTagName.crossingTag)
            {
                if (GetAttribute<string>("v", tag.Attributes) == OpenStreetMapTagName.trafficSignalsTag)
                    hasTrafficLight = true;
            }
            // Flag if amenity
            if (node_attribute == OpenStreetMapTagName.amenityTag)
            {
                amenity = GetAttribute<string>("v", tag.Attributes);
            }
            // Flag if emergency
            if(node_attribute == OpenStreetMapTagName.emergencyTag)
            {
                emergency = GetAttribute<string>("v", tag.Attributes);
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
