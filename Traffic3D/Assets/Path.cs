using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{

    public Color lineColor;

    public List<Transform> nodes = new List<Transform>();

    void Awake()
    {
        SetNodes();
    }

    private void SetNodes()
    {
        Transform[] pathTransforms = GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = lineColor;
        SetNodes();
        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 currentNode = nodes[i].position;
            Vector3 previousNode = Vector3.zero;
            Vector3 lastNode = Vector3.zero;
            if (i > 0)
            {
                previousNode = nodes[i - 1].position;
            }
            else if (i == 0 && nodes.Count > 1)
            {
                currentNode = lastNode;
            }

            Gizmos.DrawLine(previousNode, currentNode);
            Gizmos.DrawWireSphere(currentNode, 0.25f);

        }

    }

}
