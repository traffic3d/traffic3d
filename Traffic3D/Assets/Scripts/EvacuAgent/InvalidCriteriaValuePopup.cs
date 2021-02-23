using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvalidCriteriaValuePopup : MonoBehaviour
{
    private const int canvasLifetime = 5;

    [SerializeField]
    private GameObject criteriaValuePopupPrefab;

    [SerializeField]
    private GameObject exclamationMarkPrefab;

    private Queue<Transform> criteriaValuePopupQueue = new Queue<Transform>();
    private Dictionary<Transform, string> criteriaValuePopupMap = new Dictionary<Transform, string>();
    private Canvas curretCanvas;
    private GameObject currentExclamationMark;
    private bool isDisplayingError = false;

    public void FixedUpdate()
    {
        if(criteriaValuePopupMap.Count > 0 && isDisplayingError == false)
        {
            StartCoroutine(DequeueCanvasAndDestroyAfterDelay());
        }
    }

    public void CreateCriteriaValuePopup(string invalidString, bool boolToUse, Transform transform)
    {
        string canvasText = $"\'{invalidString}\' is not in criteriaValues. The boolean \'{boolToUse}\' is being applied.{System.Environment.NewLine}Check \'IsDecisionNodeBeneficial\' values in \'PedestrianPathCreator\'";

        if (!criteriaValuePopupMap.ContainsKey(transform))
        {
            criteriaValuePopupQueue.Enqueue(transform);
            criteriaValuePopupMap.Add(transform, canvasText);
        }
    }

    public IEnumerator DequeueCanvasAndDestroyAfterDelay()
    {
        isDisplayingError = true;
        Transform transform = criteriaValuePopupQueue.Dequeue();
        string canvasText = criteriaValuePopupMap[transform];
        criteriaValuePopupMap.Remove(transform);

        curretCanvas = Instantiate(criteriaValuePopupPrefab).GetComponent<Canvas>();
        Text text = curretCanvas.GetComponentInChildren<Text>();
        text.text = canvasText;

        currentExclamationMark = Instantiate(exclamationMarkPrefab, new Vector3(transform.position.x, transform.position.y + 5f, transform.position.z), Quaternion.identity);
        currentExclamationMark.transform.SetParent(transform);

        yield return new WaitForSeconds(canvasLifetime);
        Destroy(curretCanvas.gameObject);
        Destroy(currentExclamationMark.gameObject);
        isDisplayingError = false;
    }
}
