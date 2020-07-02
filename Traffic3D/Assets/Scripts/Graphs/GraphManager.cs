using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public bool flowGraph;
    public bool throughputGraph;
    public bool densityPerKmGraph;
    private List<Graph> graphs;

    void Start()
    {
        graphs = new List<Graph>();
        StartCoroutine(UpdateGraphs());
    }

    public IEnumerator UpdateGraphs()
    {
        while (true)
        {
            UpdateGeneralGraph(GraphType.FLOW, Utils.FLOW_FILE_NAME, flowGraph);
            UpdateGeneralGraph(GraphType.THROUGHPUT, Utils.THROUGHPUT_FILE_NAME, throughputGraph);
            UpdateGeneralGraph(GraphType.DENSITY_PER_KM, Utils.DENSITY_PER_KM_FILE_NAME, densityPerKmGraph);
            yield return new WaitForSeconds(10);
        }
    }

    public Graph GetGraph(GraphType graphType)
    {
        return graphs.FirstOrDefault(g => g.graphType == graphType);
    }

    public Graph CreateGraph(GraphType graphType)
    {
        GameObject template = Resources.Load<GameObject>("UI/GraphTemplate");
        GameObject graphObject = Instantiate(template, FindObjectOfType<Canvas>().transform);
        graphObject.name = graphType.ToString();
        RectTransform rectTransform = graphObject.GetComponent<RectTransform>();
        float distanceToMoveX = graphs.Sum(g => g.transform.parent.GetComponent<RectTransform>().sizeDelta.x);
        rectTransform.anchoredPosition = new Vector2(distanceToMoveX + rectTransform.sizeDelta.x / 2, -rectTransform.sizeDelta.y / 2);
        Graph graph = graphObject.GetComponentInChildren<Graph>();
        graph.graphType = graphType;
        graphs.Add(graph);
        return graph;
    }

    private void UpdateGeneralGraph(GraphType graphType, string fileName, bool enabled)
    {
        Graph graph = GetGraph(graphType);
        if (graph == null)
        {
            graph = CreateGraph(graphType);
        }
        graph.enabled = enabled;
        graph.transform.parent.gameObject.SetActive(enabled);
        if (enabled)
        {
            string[] stringData = Utils.ReadResultText(fileName);
            if (stringData == null)
            {
                return;
            }
            string resultString = stringData[0];
            List<string> resultStrings = resultString.Split(',').ToList();
            resultStrings.RemoveAll(s => s == null || s.Equals("") || s.Equals("NaN"));
            resultStrings = resultStrings.Skip(Math.Max(0, resultStrings.Count() - graph.maxDataPoints)).ToList();
            List<float> data = new List<float>();
            foreach (String s in resultStrings)
            {
                try
                {
                    data.Add(float.Parse(s));
                }
                catch (Exception e)
                {
                    Debug.Log(s + " + " + e.Message);
                }
            }
            graph.SetData(data);
            graph.UpdateGraph();
        }
    }
}
