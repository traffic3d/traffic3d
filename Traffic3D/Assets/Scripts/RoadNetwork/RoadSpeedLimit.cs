using System;
using UnityEngine;

public class RoadSpeedLimit
{
    public enum Limit
    {
        // Speeds in Km/h
        TRUNK = 100,
        PRIMARY = 80,
        SECONDARY = 60,
        TERTIARY = 50,
        UNCLASSIFIED = 50,
        TRUNK_LINK = 100,
        PRIMARY_LINK = 80,
        TERTIARY_LINK = 50,
        MOTORWAY = 110,
        MOTORWAY_LINK = 110,
        RESIDENTIAL = 40,
        SERVICE = 40,
        TRACK = 40,
        ROAD = 50
    }

    public static int GetSpeedLimitFromOSM(string roadType)
    {
        if (Enum.TryParse(roadType.ToUpper(), out Limit result))
        {
            return (int) result;
        }
        else
        {
            Debug.LogError("Unable to parse road type for speed limit: " + roadType);
            return 10;
        }
    }
}
