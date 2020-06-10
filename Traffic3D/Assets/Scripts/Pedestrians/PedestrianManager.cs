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
        int walkableArea = NavMesh.GetAreaFromName(WALKABLE_AREA);
        foreach (GameObject pathway in pathways)
        {
            pathway.GetComponents<MeshRenderer>();
            NavMeshModifier navMeshModifier = pathway.AddComponent<NavMeshModifier>();
            navMeshModifier.ignoreFromBuild = false;
            navMeshModifier.overrideArea = true;
            navMeshModifier.area = walkableArea;
        }
        GameObject[] roadways = GameObject.FindGameObjectsWithTag("roadway");
        int roadArea = NavMesh.GetAreaFromName(ROAD_AREA);
        foreach (GameObject roadway in roadways)
        {
            roadway.GetComponents<MeshRenderer>();
            NavMeshModifier navMeshModifier = roadway.AddComponent<NavMeshModifier>();
            navMeshModifier.ignoreFromBuild = false;
            navMeshModifier.overrideArea = true;
            navMeshModifier.area = roadArea;
        }
        PedestrianCrossing[] pedestrianCrossings = GameObject.FindObjectsOfType<PedestrianCrossing>();
        int pedestrianCrossingArea = NavMesh.GetAreaFromName(PEDESTRIAN_CROSSING_AREA);
        foreach (PedestrianCrossing crossing in pedestrianCrossings)
        {
            crossing.gameObject.GetComponents<MeshRenderer>();
            NavMeshModifier navMeshModifier = crossing.gameObject.AddComponent<NavMeshModifier>();
            navMeshModifier.ignoreFromBuild = false;
            navMeshModifier.overrideArea = true;
            navMeshModifier.area = pedestrianCrossingArea;
        }
        navMeshSurface = GameObject.FindObjectOfType<NavMeshSurface>();
        navMeshSurface.BuildNavMesh();
        this.pedestrianCrossings = pedestrianCrossings.ToList();
    }

    public List<PedestrianCrossing> GetPedestrianCrossings()
    {
        return pedestrianCrossings;
    }

    public PedestrianCrossing GetPedestrianCrossing(string id)
    {
        return pedestrianCrossings.Find(p => p.GetPedestrianCrossingId() == id);
    }
}
