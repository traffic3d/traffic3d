using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationPanelController : MonoBehaviour
{
    public Text NumberOfPedestriansText;
    public Text NumberOfShootersText;

    public void Update()
    {
        UpdateNumberOfPedestrianText();
        UpdateNumberOfShootersText();
    }

    public void UpdateNumberOfPedestrianText()
    {
        NumberOfPedestriansText.text = GetNumberOfPedestrianWithoutGivenTags(new List<string> { EvacuAgentSceneParamaters.SHOOTER_TAG }).ToString();
    }

    public void UpdateNumberOfShootersText()
    {
        NumberOfShootersText.text = GetNumberOfObjectsWithTag(EvacuAgentSceneParamaters.SHOOTER_TAG).ToString();
    }

    public int GetNumberOfPedestrianWithoutGivenTags(List<string> tagsToNotCount)
    {
        int counter = 0;

        foreach (Pedestrian pedestrian in GameObject.FindObjectsOfType<Pedestrian>())
        {
            if (!tagsToNotCount.Contains(pedestrian.tag))
            {
                counter++;
            }
        }
        return counter;
    }

    public int GetNumberOfObjectsWithTag(string tag)
    {
        return GameObject.FindGameObjectsWithTag(tag).Length;
    }
}
