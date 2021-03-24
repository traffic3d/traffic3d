using System.Collections.Generic;
using UnityEngine;

public abstract class PedestrianPointPathCreator : MonoBehaviour
{
    public abstract List<PedestrianPoint> CreatePath();
}
