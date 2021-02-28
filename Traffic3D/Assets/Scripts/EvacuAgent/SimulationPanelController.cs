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
}
