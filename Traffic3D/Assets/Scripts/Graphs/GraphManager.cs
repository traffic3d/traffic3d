using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public bool flowGraph;
    public bool throughputGraph;
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
            UpdateFlowGraph();
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
        graphObject.name = "FlowGraph";
        RectTransform rectTransform = graphObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(rectTransform.sizeDelta.x / 2, -rectTransform.sizeDelta.y / 2);
        Graph graph = graphObject.GetComponentInChildren<Graph>();
        graph.graphType = graphType;
        graphs.Add(graph);
        return graph;
    }

    public void UpdateFlowGraph()
    {
        Graph graph = GetGraph(GraphType.FLOW);
        if (graph == null)
        {
            graph = CreateGraph(GraphType.FLOW);
        }
        graph.enabled = flowGraph;
        graph.transform.parent.gameObject.SetActive(flowGraph);
        if (flowGraph)
        {
            // Todo SET GRAPH DATA
            //string flow = Utils.ReadText("Flow.csv")[0];
            //List<string> resultString = flow.Split(',').ToList();
            //resultString.RemoveAll(s => s.Equals("NaN"));
            //List<float> results = resultString.Select(s => float.Parse(s)).ToList();
            //graph.SetData(results);
            graph.UpdateGraph();
        }
    }
}
