using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightRed4 : MonoBehaviour
{

    public Material redMaterial;
    public Material greenMaterial;
    public Material amberMaterial;
    public Material blackMaterial;

    public Material currrentMaterial;

    void Start()
    {
        currrentMaterial = GetComponent<Renderer>().material;
    }

    public void SetToBlackMaterial()
    {
        currrentMaterial = blackMaterial;
        Material[] materials = GetComponent<Renderer>().materials;
        materials[0] = blackMaterial;
        GetComponent<Renderer>().materials = materials;
    }

    public void SetToRedMaterial()
    {
        currrentMaterial = redMaterial;
        Material[] materials = GetComponent<Renderer>().materials;
        materials[0] = redMaterial;
        GetComponent<Renderer>().materials = materials;
    }

    public void SetToGreenMaterial()
    {
        currrentMaterial = greenMaterial;
        Material[] materials = GetComponent<Renderer>().materials;
        materials[0] = greenMaterial;
        GetComponent<Renderer>().materials = materials;
    }

    public void SetToAmberMaterial()
    {
        currrentMaterial = amberMaterial;
        Material[] materials = GetComponent<Renderer>().materials;
        materials[0] = amberMaterial;
        GetComponent<Renderer>().materials = materials;
    }

}
