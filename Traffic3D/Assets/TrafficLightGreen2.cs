using UnityEngine;

public class TrafficLightGreen2 : MonoBehaviour
{

    public Material greenMaterial;
    public Material blackMaterial;

    public Material currentMaterial;

    void Start()
    {
        currentMaterial = GetComponent<Renderer>().material;
    }

    public void SetToBlackMaterial()
    {
        currentMaterial = blackMaterial;
        Material[] materials = GetComponent<Renderer>().materials;
        materials[0] = blackMaterial;
        GetComponent<Renderer>().materials = materials;
    }

    public void SetToGreenMaterial()
    {
        currentMaterial = greenMaterial;
        Material[] materials = GetComponent<Renderer>().materials;
        materials[0] = greenMaterial;
        GetComponent<Renderer>().materials = materials;
    }

}
