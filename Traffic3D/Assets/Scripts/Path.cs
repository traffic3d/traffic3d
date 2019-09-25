using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Path : MonoBehaviour
{
    public Color lineColor;
    public List<Transform> nodes = new List<Transform>();

    void Awake()
    {
        SetNodes();
    }

    /// <summary>
    /// Sets the nodes of the path using the child game objects.
    /// </summary>
    private void SetNodes()
    {
        nodes = GetComponentsInChildren<Transform>().ToList().FindAll(node => node != transform);
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
