using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopLine : MonoBehaviour
{
    public Type type = Type.MERGE;

    public enum Type
    {
        MERGE,
        JUNCTION
    }
}
