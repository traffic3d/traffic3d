using UnityEngine;

public class TrafficLightRed1 : MonoBehaviour
{

    public Material redMaterial;
    public Material blackMaterial;

    public Material currentMaterial;

    void Start()
    {
        currentMaterial = GetComponent<Renderer>().material;
    }

    public void SetToRedMaterial()
    {
        currentMaterial = redMaterial;
        Material[] materials = GetComponent<Renderer>().materials;
        materials[0] = redMaterial;
        GetComponent<Renderer>().materials = materials;
    }

    public void SetToBlackMaterial()
    {

        currentMaterial = blackMaterial;
        Material[] materials = GetComponent<Renderer>().materials;
        materials[0] = blackMaterial;
        GetComponent<Renderer>().materials = materials;
    }

}
