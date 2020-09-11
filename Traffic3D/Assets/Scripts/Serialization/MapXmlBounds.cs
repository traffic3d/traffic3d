using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;

public class MapXmlBounds
{
    //  <bounds minlat="52.5043000" minlon="-2.0882000" maxlat="52.5128000" maxlon="-2.0793000"/>
    public float MinLat { get; private set; }
    public float MinLon { get; private set; }
    public float MaxLat { get; private set; }
    public float MaxLon { get; private set; }
    public Vector3 Centre { get; private set; }

    public float SizeX { get; private set; } 
    public float SizeY { get; private set; }
    public Vector3 Size;



    public MapXmlBounds(XmlNode node)
    {
        // Get the values from the node
        MinLat = GetAttribute<float>("minlat", node.Attributes);
        MaxLat = GetAttribute<float>("maxlat", node.Attributes);
        MinLon = GetAttribute<float>("minlon", node.Attributes);
        MaxLon = GetAttribute<float>("maxlon", node.Attributes);


        float xMax = (float)LatLonConverter.lonToX(MaxLon);
        float xMin = (float)LatLonConverter.lonToX(MinLon);

        float yMax = (float)LatLonConverter.latToY(MaxLat);
        float yMin = (float)LatLonConverter.latToY(MinLat);

        SizeX = xMax - xMin;
        SizeY = yMax - yMin;

        Size = new Vector3(SizeX*2f, 0, SizeY*2f);


        // Get Center X & Y (Max+min)/2 - (Convert double to float)
        float centerX = (float)( (xMax + xMin) / 2f);
        float centerY = (float)( (yMax + yMin) / 2f);
        
        //centre of map
        Centre = new Vector3(centerX, 0, centerY);

    }


    /// <summary>
    /// Get's an attributes' value from a collection and returns it as another Data Type
    /// </summary>
    /// <typeparam name="T">A required data-type</typeparam>
    /// <param name="attributeName">Name of Attribute</param>
    /// <param name="attributes">Collection holding attributes</param>
    /// <returns>attributeName converted into type T </returns>
    protected T GetAttribute<T>(string attributeName, XmlAttributeCollection attributes)
    {
        //strValue = the value of our attribute (XML Tag stores this value as a string.)
        string strValue = attributes[attributeName].Value;
        //Returning value as the required data type (E.g would return a String-Id as uLong-Id)
        return (T)Convert.ChangeType(strValue, typeof(T));
    }

}
