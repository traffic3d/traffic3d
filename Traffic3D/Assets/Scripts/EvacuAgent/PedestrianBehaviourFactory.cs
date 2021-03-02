using UnityEngine;

public class PedestrianBehaviourFactory : MonoBehaviour
{
    [SerializeField]
    private GameObject fieldOfViewPrefab;

    [SerializeField]
    private GameObject shooterHighlightPrefab;

    private int numberOfShooterAgentsSpawned = 0;

    public void AddEvacuAgentBehaviour(Pedestrian pedestrian)
    {
        if (pedestrian.GetComponentInChildren<FieldOfView>() == null)
        {
            GameObject fieldOfView = Instantiate(fieldOfViewPrefab, pedestrian.transform.position, Quaternion.identity);
            fieldOfView.transform.SetParent(pedestrian.transform);
            fieldOfView.GetComponent<MeshRenderer>().enabled = EvacuAgentSceneParamaters.IS_FOV_VISUAL_ENABLED;
        }

        if(numberOfShooterAgentsSpawned < EvacuAgentSceneParamaters.NUMBER_OF_SHOOTER_AGENTS)
        {
            AddShooterBehaviour(pedestrian);
            numberOfShooterAgentsSpawned++;
        }
    }

    private void AddShooterBehaviour(Pedestrian pedestrian)
    {
        pedestrian.gameObject.AddComponent<PedestrianPathCreator>();
        GameObject highlight = Instantiate(shooterHighlightPrefab, pedestrian.transform.position, Quaternion.identity);
        highlight.transform.SetParent(pedestrian.transform);
        highlight.GetComponent<MeshRenderer>().enabled = EvacuAgentSceneParamaters.IS_SHOOTER_HIGHTLIGHT_VISUAL_ENABLED;
        pedestrian.tag = EvacuAgentSceneParamaters.SHOOTER_TAG;
        pedestrian.isShooterAgent = true;
    }
}
