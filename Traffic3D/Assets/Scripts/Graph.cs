using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour
{
    public Sprite dataPointSprite;
    private RectTransform graphContainer;
    private List<GameObject> graphComponents;
    private List<float> data;
    private Text textTemplate;
    public int maxDataPoints = 20;
    public int numberOfLabelsY = 5;

    void Awake()
    {
        graphContainer = gameObject.GetComponent<RectTransform>();
        graphComponents = new List<GameObject>();
        textTemplate = Resources.Load<Text>("UI/textTemplate");
    }

    private void Start()
    {
        data = new List<float>() { 35, 23, 58, 24, 89, 87, 76, 54, 23, 10, 23, 43, 54, 65, 66, 45, 34, 56, 26, 24 };
        UpdateGraph();
    }

    public void UpdateGraph()
    {
        graphComponents.ForEach(ob => Destroy(ob));
        DrawGraphLines();
        float graphHeight = graphContainer.sizeDelta.y;
        float graphWidth = graphContainer.sizeDelta.x;
        float yMax = data.Max();
        float dataPointAmount = Math.Min(data.Count, maxDataPoints);
        float xSize = graphWidth / dataPointAmount;
        GameObject lastDataPoint = null;
        for (int i = 0; i < dataPointAmount; i++)
        {
            float xPosition = xSize + i * xSize;
            float yPosition = (data[i] / yMax) * graphHeight;
            GameObject dataPoint = CreateDataPoint(new Vector2(xPosition, yPosition));
            graphComponents.Add(dataPoint);
            GameObject dataPointXLabel = CreateDataPointLabel(new Vector2(xPosition, -15), i + "");
            graphComponents.Add(dataPointXLabel);
            if (lastDataPoint != null)
            {
                GameObject connection = CreateDataPointConnection(lastDataPoint.GetComponent<RectTransform>().anchoredPosition,
                    dataPoint.GetComponent<RectTransform>().anchoredPosition);
                graphComponents.Add(connection);
            }
            lastDataPoint = dataPoint;
        }
        int distanceBetweenLabelsY = (int) Math.Ceiling(graphHeight / (numberOfLabelsY - 1));
        for(int i = 0; i <= (int) graphHeight; i = i + distanceBetweenLabelsY)
        {
            double amount = Math.Round((i / graphHeight) * yMax);
            GameObject dataPointYLabel = CreateDataPointLabel(new Vector2(-15, i), amount + "");
            graphComponents.Add(dataPointYLabel);
        }
    }

    private void DrawGraphLines()
    {
        GameObject xLine = new GameObject("xLine", typeof(Image));
        xLine.transform.SetParent(graphContainer, false);
        xLine.GetComponent<Image>().color = Color.white;
        RectTransform rectTransformX = xLine.GetComponent<RectTransform>();
        rectTransformX.sizeDelta = new Vector2(graphContainer.sizeDelta.x, 3);
        rectTransformX.anchorMin = Vector2.zero;
        rectTransformX.anchorMax = Vector2.zero;
        rectTransformX.anchoredPosition = Vector2.right * graphContainer.sizeDelta.x * 0.5f;
        graphComponents.Add(xLine);
        GameObject yLine = new GameObject("yLine", typeof(Image));
        yLine.transform.SetParent(graphContainer, false);
        yLine.GetComponent<Image>().color = Color.white;
        RectTransform rectTransformY = yLine.GetComponent<RectTransform>();
        rectTransformY.sizeDelta = new Vector2(3, graphContainer.sizeDelta.y);
        rectTransformY.anchorMin = Vector2.zero;
        rectTransformY.anchorMax = Vector2.zero;
        rectTransformY.anchoredPosition = Vector2.up * graphContainer.sizeDelta.y * 0.5f;
        graphComponents.Add(yLine);
    }

    private GameObject CreateDataPoint(Vector2 position)
    {
        GameObject dataPoint = new GameObject("dataPoint", typeof(Image));
        dataPoint.transform.SetParent(graphContainer, false);
        dataPoint.GetComponent<Image>().sprite = dataPointSprite;
        RectTransform rectTransform = dataPoint.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = new Vector2(10, 10);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        return dataPoint;
    }

    private GameObject CreateDataPointLabel(Vector2 position, string label)
    {
        Text dataPointLabel = Instantiate(textTemplate);
        dataPointLabel.transform.SetParent(graphContainer, false);
        dataPointLabel.text = label;
        dataPointLabel.alignment = TextAnchor.MiddleCenter;
        RectTransform rectTransform = dataPointLabel.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        return dataPointLabel.gameObject;
    }

    private GameObject CreateDataPointConnection(Vector2 dataPointPosition1, Vector2 dataPointPosition2)
    {
        GameObject connection = new GameObject("connection", typeof(Image));
        connection.transform.SetParent(graphContainer, false);
        connection.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        RectTransform rectTransform = connection.GetComponent<RectTransform>();
        Vector2 direction = (dataPointPosition2 - dataPointPosition1).normalized;
        float distance = Vector2.Distance(dataPointPosition1, dataPointPosition2);
        rectTransform.sizeDelta = new Vector2(distance, 3);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        rectTransform.anchoredPosition = dataPointPosition1 + direction * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, (float) Math.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        return connection;
    }

}
