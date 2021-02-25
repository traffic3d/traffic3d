using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationDataTracker : MonoBehaviour
{
    public int GetNumberOfType(System.Type type)
    {
        return GameObject.FindObjectsOfType(type).Length;
    }

    public int GetNumberOfObjectsWithTag(string tag)
    {
        return GameObject.FindGameObjectsWithTag(tag).Length;
    }
}
