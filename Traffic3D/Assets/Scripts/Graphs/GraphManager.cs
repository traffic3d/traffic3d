using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public bool timeTraveledGraph;
    public bool throughputGraph;
    public bool delayGraph;
    private List<Graph> graphs;
    private const float SECONDS_BETWEEN_INDIVIDUAL_GRAPH_UPDATES = 3f;
    private const string SIMULATION_TIME_STEPS = "Simulation Time-Steps";
    private const string NUMBER_VEHICLES_OBSERVED = "Number of Vehicles Observed";

    void Start()
    {
        graphs = new List<Graph>();
        StartCoroutine(UpdateGraphs());
    }

    public IEnumerator UpdateGraphs()
    {
        while (true)
        {
            UpdateGeneralGraph(GraphType.TIME_TRAVELED, Utils.VEHICLE_TIMES_FILE_NAME, timeTraveledGraph);
            yield return new WaitForSeconds(SECONDS_BETWEEN_INDIVIDUAL_GRAPH_UPDATES);
            UpdateGeneralGraph(GraphType.THROUGHPUT, Utils.THROUGHPUT_FILE_NAME, throughputGraph);
            yield return new WaitForSeconds(SECONDS_BETWEEN_INDIVIDUAL_GRAPH_UPDATES);
            UpdateGeneralGraph(GraphType.DELAY, Utils.VEHICLE_DELAY_TIMES_FILE_NAME, delayGraph);
            yield return new WaitForSeconds(SECONDS_BETWEEN_INDIVIDUAL_GRAPH_UPDATES);
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

    private async void UpdateGeneralGraph(GraphType graphType, string fileName, bool enabled)
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
            await Task.Run(() =>
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
                if (graphType == GraphType.THROUGHPUT)
                {
                    graph.displayLatestXLabel = true;
                    graph.xLabel = SIMULATION_TIME_STEPS;
                }
                else if (graphType == GraphType.TIME_TRAVELED || graphType == GraphType.DELAY)
                {
                    graph.displayLatestXLabel = true;
                    graph.xLabel = NUMBER_VEHICLES_OBSERVED;
                }
            });
            graph.UpdateGraph();
        }
    }
}
