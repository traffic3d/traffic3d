using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AI;

public class PedestrianManager : MonoBehaviour
{
    public const string WALKABLE_AREA = "Walkable";
    public const string ROAD_AREA = "Road";
    public const string PEDESTRIAN_CROSSING_AREA = "PedestrianCrossing";

    public static PedestrianManager instance;

    public static PedestrianManager GetInstance()
    {
        return instance;
    }

    private List<PedestrianCrossing> pedestrianCrossings;
    private NavMeshSurface navMeshSurface;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        GameObject[] pathways = GameObject.FindGameObjectsWithTag("pathway");
        foreach (GameObject pathway in pathways)
        {
            pathway.GetComponents<MeshRenderer>();
            NavMeshModifier navMeshModifier = pathway.AddComponent<NavMeshModifier>();
            navMeshModifier.ignoreFromBuild = false;
            navMeshModifier.overrideArea = true;
            navMeshModifier.area = NavMesh.GetAreaFromName(WALKABLE_AREA);
        }
        GameObject[] roadways = GameObject.FindGameObjectsWithTag("roadway");
        foreach (GameObject roadway in roadways)
        {
            roadway.GetComponents<MeshRenderer>();
            NavMeshModifier navMeshModifier = roadway.AddComponent<NavMeshModifier>();
            navMeshModifier.ignoreFromBuild = false;
            navMeshModifier.overrideArea = true;
            navMeshModifier.area = NavMesh.GetAreaFromName(ROAD_AREA);
        }
        PedestrianCrossing[] pedestrianCrossings = GameObject.FindObjectsOfType<PedestrianCrossing>();
        foreach (PedestrianCrossing crossing in pedestrianCrossings)
        {
            crossing.gameObject.GetComponents<MeshRenderer>();
            NavMeshModifier navMeshModifier = crossing.gameObject.AddComponent<NavMeshModifier>();
            navMeshModifier.ignoreFromBuild = false;
            navMeshModifier.overrideArea = true;
            navMeshModifier.area = NavMesh.GetAreaFromName(PEDESTRIAN_CROSSING_AREA);
        }
        navMeshSurface = GameObject.FindObjectOfType<NavMeshSurface>();
        navMeshSurface.BuildNavMesh();
        this.pedestrianCrossings = pedestrianCrossings.ToList();
    }

    public PedestrianCrossing GetPedestrianCrossing(string id)
    {
        return pedestrianCrossings.Find(p => p.GetPedestrianCrossingId() == id);
    }
}
