using UnityEngine;
using UnityEngine.UI;

public class InformationPanelController : MonoBehaviour
{
    [SerializeField]
    private Text NumberOfPedestriansText;

    public void Update()
    {
        NumberOfPedestriansText.text = GetNumberOfType(typeof(Pedestrian)).ToString();
        //NumberOfShootersText.text = simulationDataTracker.GetNumberOfObjectsWithTag(shooterTag).ToString(); commented out as the shooter tag does not exist yet
    }

    public int GetNumberOfType(System.Type type)
    {
        return GameObject.FindObjectsOfType(type).Length;
    }

    public int GetNumberOfObjectsWithTag(string tag)
    {
        return GameObject.FindGameObjectsWithTag(tag).Length;
    }
}
