﻿using UnityEngine;
using UnityEngine.AI;

public class PedestrianPoint : MonoBehaviour
{
    public Vector3 GetPointLocation()
    {
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(transform.position, out hit, float.MaxValue, NavMesh.GetAreaFromName("walkable")))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }
}
