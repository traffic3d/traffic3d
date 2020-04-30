using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PedestrianManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] pathways = GameObject.FindGameObjectsWithTag("pathway");
        foreach(GameObject pathway in pathways)
        {
            pathway.GetComponents<MeshRenderer>();
            NavMeshModifier navMeshModifier = pathway.AddComponent<NavMeshModifier>();
            navMeshModifier.ignoreFromBuild = false;
            navMeshModifier.overrideArea = true;
            navMeshModifier.area = 0;
        }
        GameObject[] roadways = GameObject.FindGameObjectsWithTag("roadway");
        foreach (GameObject roadway in roadways)
        {
            roadway.GetComponents<MeshRenderer>();
            NavMeshModifier navMeshModifier = roadway.AddComponent<NavMeshModifier>();
            navMeshModifier.ignoreFromBuild = false;
            navMeshModifier.overrideArea = true;
            navMeshModifier.area = 3;
        }
        NavMeshSurface navMeshSurface = GameObject.FindObjectOfType<NavMeshSurface>();
        navMeshSurface.BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
