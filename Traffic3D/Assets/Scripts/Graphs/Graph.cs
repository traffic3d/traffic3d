using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour
{
    public Sprite dataPointSprite;
    public Vector2 dataPointSize = new Vector2(5, 5);
    private RectTransform graphContainer;
    private List<GameObject> graphComponents;
    private List<float> data;
    private Text textTemplate;
    public int maxDataPoints = 20;
    public int numberOfLabelsY = 6;
    public GraphType graphType;
    public Color axisColor = Color.white;
    public Color lineColor = new Color(0.5664f, 0.7461f, 0.8555f);
    public string xLabel = "";
    public bool displayLatestXLabel = false;
    private List<GameObject> deactivatedImageObjectPool;

    void Awake()
    {
        graphContainer = gameObject.GetComponent<RectTransform>();
        graphComponents = new List<GameObject>();
        deactivatedImageObjectPool = new List<GameObject>();
        textTemplate = Resources.Load<Text>("UI/textTemplate");
    }

    public void SetData(List<float> data)
    {
        this.data = data;
    }

    public void UpdateGraph()
    {
        new List<GameObject>(graphComponents).ForEach(ob => RemoveGameObject(ob));
        graphComponents.Clear();
        if (graphContainer == null)
        {
            return;
        }
        DrawGraphLines();
        DrawTitles();
        if (data == null || data.Count == 0)
        {
            return;
        }
        float graphHeight = graphContainer.sizeDelta.y;
        float graphWidth = graphContainer.sizeDelta.x;
        float yMax = data.Max();
        // Currently yMin will always be 0
        float dataPointAmount = Math.Min(data.Count, maxDataPoints);
        float xSize = graphWidth / dataPointAmount;
        GameObject lastDataPoint = null;
        for (int i = 0; i < dataPointAmount; i++)
        {
            float xPosition = xSize + i * xSize;
            float yPosition = (data[i] / yMax) * graphHeight;
            GameObject dataPoint = CreateDataPoint(new Vector2(xPosition, yPosition));
            if (lastDataPoint != null)
            {
                GameObject connection = JoinConsecutiveDataPoints(lastDataPoint.GetComponent<RectTransform>().anchoredPosition,
                    dataPoint.GetComponent<RectTransform>().anchoredPosition);
            }
            lastDataPoint = dataPoint;
        }
        int distanceBetweenLabelsY = (int)Math.Ceiling(graphHeight / (numberOfLabelsY - 1));
        for (int i = 0; i <= (int)graphHeight; i = i + distanceBetweenLabelsY)
        {
            float amount = (float)Math.Round((i / graphHeight) * yMax);
            float yPosition = (amount / yMax) * graphHeight;
            GameObject dataPointYLabel = CreateLabel(new Vector2(-15, yPosition), amount + "");
        }
    }

    private void DrawGraphLines()
    {
        GameObject xLine = CreateGameObjectFromPool("xLine", typeof(Image));
        xLine.transform.SetParent(graphContainer, false);
        xLine.GetComponent<Image>().color = axisColor;
        RectTransform rectTransformX = xLine.GetComponent<RectTransform>();
        rectTransformX.sizeDelta = new Vector2(graphContainer.sizeDelta.x, 3);
        rectTransformX.anchorMin = Vector2.zero;
        rectTransformX.anchorMax = Vector2.zero;
        rectTransformX.anchoredPosition = Vector2.right * graphContainer.sizeDelta.x * 0.5f;
        GameObject yLine = CreateGameObjectFromPool("yLine", typeof(Image));
        yLine.transform.SetParent(graphContainer, false);
        yLine.GetComponent<Image>().color = axisColor;
        RectTransform rectTransformY = yLine.GetComponent<RectTransform>();
        rectTransformY.sizeDelta = new Vector2(3, graphContainer.sizeDelta.y);
        rectTransformY.anchorMin = Vector2.zero;
        rectTransformY.anchorMax = Vector2.zero;
        rectTransformY.anchoredPosition = Vector2.up * graphContainer.sizeDelta.y * 0.5f;
    }

    private void DrawTitles()
    {
        string graphTitle = graphType.ToString().ToLower().Replace('_', ' ');
        graphTitle = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(graphTitle);
        GameObject title = CreateLabel(new Vector2(graphContainer.sizeDelta.x / 2, graphContainer.sizeDelta.y + 10), graphTitle);
        GameObject xAxisTitle = CreateLabel(new Vector2(graphContainer.sizeDelta.x / 2, -10), xLabel);
        if (displayLatestXLabel)
        {
            GameObject xAxisTitleLatest = CreateLabel(new Vector2(graphContainer.sizeDelta.x, -10), "Latest");
        }
    }

    private GameObject CreateDataPoint(Vector2 position)
    {
        GameObject dataPoint = CreateGameObjectFromPool("dataPoint", typeof(Image));
        dataPoint.transform.SetParent(graphContainer, false);
        dataPoint.GetComponent<Image>().sprite = dataPointSprite;
        RectTransform rectTransform = dataPoint.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = dataPointSize;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        return dataPoint;
    }

    private GameObject CreateLabel(Vector2 position, string label)
    {
        Text dataPointLabel = Instantiate(textTemplate);
        graphComponents.Add(dataPointLabel.gameObject);
        dataPointLabel.transform.SetParent(graphContainer, false);
        dataPointLabel.text = label;
        dataPointLabel.alignment = TextAnchor.MiddleCenter;
        RectTransform rectTransform = dataPointLabel.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        return dataPointLabel.gameObject;
    }

    private GameObject JoinConsecutiveDataPoints(Vector2 dataPointPosition1, Vector2 dataPointPosition2)
    {
        GameObject connection = CreateGameObjectFromPool("connection", typeof(Image));
        connection.transform.SetParent(graphContainer, false);
        connection.GetComponent<Image>().color = lineColor;
        RectTransform rectTransform = connection.GetComponent<RectTransform>();
        rectTransform.SetAsFirstSibling();
        // Get direction of the first point to the second.
        Vector2 direction = (dataPointPosition2 - dataPointPosition1).normalized;
        // Get distance of between the two points.
        float distance = Vector2.Distance(dataPointPosition1, dataPointPosition2);
        rectTransform.sizeDelta = new Vector2(distance, 2);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        // Change the line position to the first data point and move it to halfway across the two points (as the anchor point is in the middle of the line). 
        rectTransform.anchoredPosition = dataPointPosition1 + direction * distance * 0.5f;
        // Rotate the line so its between the two points by working out the angle using the previously calculated direction
        rectTransform.localEulerAngles = new Vector3(0, 0, (float)Math.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        return connection;
    }

    private GameObject CreateGameObjectFromPool(string name, Type type)
    {
        GameObject gameObject = null;
        if (type == typeof(Image))
        {
            gameObject = deactivatedImageObjectPool.FirstOrDefault();
        }
        if (gameObject != null)
        {
            deactivatedImageObjectPool.Remove(gameObject);
            gameObject.SetActive(true);
            gameObject.name = name;
        }
        else
        {
            gameObject = new GameObject(name, type);
        }
        graphComponents.Add(gameObject);
        return gameObject;
    }

    private void RemoveGameObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return;
        }
        graphComponents.Remove(gameObject);
        if (gameObject.GetComponent(typeof(Image)) != null)
        {
            gameObject.SetActive(false);
            deactivatedImageObjectPool.Add(gameObject);
            return;
        }
        Destroy(gameObject);
    }

}
