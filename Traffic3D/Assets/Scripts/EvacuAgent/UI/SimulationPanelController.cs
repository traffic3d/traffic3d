using UnityEngine;

public class SimulationPanelController : MonoBehaviour
{
    public void TogglePedestrianFieldOfViewVisuals(bool isTurnedOn)
    {
        EvacuAgentSceneParamaters.IS_FOV_VISUAL_ENABLED = isTurnedOn;

        foreach(FieldOfView fieldOfView in GameObject.FindObjectsOfType<FieldOfView>())
        {
            fieldOfView.GetComponent<MeshRenderer>().enabled = isTurnedOn;
        }
    }

    public void ToggleShooterPedestrianColour(bool isHighlightToggledOn)
    {
        foreach (GameObject shooterHighlight in GameObject.FindGameObjectsWithTag(EvacuAgentSceneParamaters.SHOOTER_HIGHLIGHT_TAG))
        {
            shooterHighlight.GetComponent<MeshRenderer>().enabled =  isHighlightToggledOn;
        }
    }

    public void ToggleWorkerPedestriansColour(bool isHighlightToggledOn)
    {
        foreach (GameObject workerHighlight in GameObject.FindGameObjectsWithTag(EvacuAgentSceneParamaters.WORKER_HIGHLIGHT_TAG))
        {
            workerHighlight.GetComponent<MeshRenderer>().enabled = isHighlightToggledOn;
        }
    }

    public void ToggleFriendGroupPedestriansColour(bool isHighlightToggledOn)
    {
        foreach (GameObject friendGroupHighlight in GameObject.FindGameObjectsWithTag(EvacuAgentSceneParamaters.FRIEND_GROUP_HIGHLIGHT_TAG))
        {
            friendGroupHighlight.GetComponent<MeshRenderer>().enabled = isHighlightToggledOn;
        }
    }
}
