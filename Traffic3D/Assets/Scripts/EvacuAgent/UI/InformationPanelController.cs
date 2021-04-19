using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationPanelController : MonoBehaviour
{
    public Text NumberOfPedestriansText;
    public Text NumberOfShootersText;
    public Text NumberOfWorkerText;
    public Text NumberOfFriendsText;

    public Text ShooterPercentText;
    public Text WorkerPercentText;
    public Text FriendPercentText;

    private int totalPedestrians;
    public void Update()
    {
        UpdateNumberOfPedestrianText();
        UpdateNumberOfShootersText();
        UpdateNumberOfWorkersText();
        UpdateNumberOfFriendsText();
    }

    public void UpdateNumberOfPedestrianText()
    {
        totalPedestrians = GameObject.FindObjectsOfType<Pedestrian>().Length;
        NumberOfPedestriansText.text = totalPedestrians.ToString();
    }

    public void UpdateNumberOfShootersText()
    {
        int numberOfType = GetNumberOfObjectsWithTag(EvacuAgentSceneParamaters.SHOOTER_TAG);
        float percent = CalculatePercent(totalPedestrians, numberOfType);

        NumberOfShootersText.text = numberOfType.ToString();
        ShooterPercentText.text = $"{percent.ToString()}%";
    }

    public void UpdateNumberOfWorkersText()
    {
        int numberOfType = GetNumberOfObjectsWithTag(EvacuAgentSceneParamaters.WORKER_TAG);
        float percent = CalculatePercent(totalPedestrians, numberOfType);

        NumberOfWorkerText.text = numberOfType.ToString();
        WorkerPercentText.text = $"{percent.ToString()}%";
    }

    public void UpdateNumberOfFriendsText()
    {
        int numberOfType = GetNumberOfObjectsWithTag(EvacuAgentSceneParamaters.FRIEND_TAG);
        float percent = CalculatePercent(totalPedestrians, numberOfType);

        NumberOfFriendsText.text = numberOfType.ToString();
        FriendPercentText.text = $"{percent.ToString()}%";
    }

    public float CalculatePercent(int totalPedestrians, int numberOfType)
    {
        if (numberOfType == 0)
            return 0;

        float percent = Mathf.Round((float)numberOfType / (float)totalPedestrians * 100f);
        return percent;
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
