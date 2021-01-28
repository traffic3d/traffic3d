﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VehiclePath
{
    public Color lineColor;
    public List<Transform> nodes = new List<Transform>();

    public VehiclePath(List<Transform> nodes)
    {
        this.nodes = nodes;
    }

    public float GetDistanceToNextStopNode(Transform currentNode, Transform vehicleTransform)
    {
        float totalDistance = Vector3.Distance(currentNode.position, vehicleTransform.position);
        if (TrafficLightManager.GetInstance().IsStopNode(currentNode))
        {
            return totalDistance;
        }
        for (int i = nodes.IndexOf(currentNode) + 1; i < nodes.Count; i++)
        {
            Transform lastNode = nodes[i - 1];
            Transform node = nodes[i];
            float distanceBetweenNodes = Vector3.Distance(lastNode.position, node.position);
            totalDistance = totalDistance + distanceBetweenNodes;
            if (TrafficLightManager.GetInstance().IsStopNode(node))
            {
                return totalDistance;
            }
        }
        // No next stop node found, return NaN.
        return float.NaN;
    }

    public Transform GetNextStopNode(Transform currentNode)
    {
        for (int i = nodes.IndexOf(currentNode); i < nodes.Count; i++)
        {
            if (TrafficLightManager.GetInstance().IsStopNode(nodes[i]))
            {
                return nodes[i];
            }
        }
        return null;
    }

    /// <summary>
    /// First, gets the position on the road which uses distanceAhead to find that position from the vehicle
    /// Then returns the angle in degrees from the vehicle direction to the direction of the position.
    /// </summary>
    /// <param name="currentNode">The current node the vehicle is at.</param>
    /// <param name="vehicleTransform">The vehicle.</param>
    /// <param name="distanceAhead">The distance to scan ahead to from the vehicle which gets the position.</param>
    /// <param name="debug">Show the rays from the vehicle to the position on the road for debugging.</param>
    /// <returns>The angle in degress from the vehicle direction to the direction of the position.</returns>
    public float GetDirectionDifferenceToRoadAheadByDistanceMeasured(Transform currentNode, Transform vehicleTransform, float distanceAhead, bool debug)
    {
        float distanceFromCurrentNode = Vector3.Distance(currentNode.position, vehicleTransform.position);
        Vector3 finalPosition = Vector3.negativeInfinity;
        if (distanceFromCurrentNode > distanceAhead)
        {
            // Distance is within the vehicle position and the current node.
            finalPosition = GetPositionFromNode1ToNode2(vehicleTransform.position, currentNode.position, distanceAhead);
        }
        else
        {
            float totalDistanceEvaluated = distanceFromCurrentNode;
            int currentNodeIndex = nodes.IndexOf(currentNode);
            // Iterate through the nodes to find the position on the road.
            for (int i = nodes.IndexOf(currentNode) + 1; i < nodes.Count; i++)
            {
                Transform lastNode = nodes[i - 1];
                Transform node = nodes[i];
                float distanceBetweenNodes = Vector3.Distance(lastNode.position, node.position);
                totalDistanceEvaluated = totalDistanceEvaluated + distanceBetweenNodes;
                if (totalDistanceEvaluated >= distanceAhead)
                {
                    // Distance is within the iterated node and the last node.
                    float extraDistanceAhead = distanceAhead - (totalDistanceEvaluated - distanceBetweenNodes);
                    finalPosition = GetPositionFromNode1ToNode2(lastNode.position, node.position, extraDistanceAhead);
                    break;
                }
            }
        }
        if (finalPosition != Vector3.negativeInfinity)
        {
            Vector3 normalizedDirection = Vector3.Normalize(finalPosition - vehicleTransform.position);
            normalizedDirection.y = 0; // Set Y to zero to ignore Y direction
            float angle = Vector3.Angle(vehicleTransform.forward, normalizedDirection);
            if (debug)
            {
                Debug.DrawLine(vehicleTransform.position, finalPosition, Color.Lerp(Color.green, Color.red, angle / 90f));
            }
            return angle;
        }
        else
        {
            // Return NaN as difference doesn't exist. 
            // An example of this is if the path is only 50 metres but we try to evaluate 100 metres, 
            // there is no difference as the path doesn't exist that far ahead
            return float.NaN;
        }
    }

    private Vector3 GetPositionFromNode1ToNode2(Vector3 node1, Vector3 node2, float distanceToMeasure)
    {
        float distanceBetweenNodes = Vector3.Distance(node1, node2);
        // Finds the ratio of the distance to measure between the two nodes
        // E.g. If 2 nodes with a distance of 10m and the distance to measure was 4m
        // The ratio would be 0.4.
        double ratio = distanceToMeasure / distanceBetweenNodes;
        if (ratio < 0 || ratio > 1)
        {
            throw new System.ArgumentOutOfRangeException("distanceToMeasure " + distanceToMeasure + " is out of bounds from nodes " + node1.ToString() + " and " + node2.ToString() + " where the ratio is " + ratio);
        }
        float xDest = (float)((1 - ratio) * node1.x + ratio * node2.x);
        float yDest = (float)((1 - ratio) * node1.y + ratio * node2.y);
        float zDest = (float)((1 - ratio) * node1.z + ratio * node2.z);
        return new Vector3(xDest, yDest, zDest);
    }
}
