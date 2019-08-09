﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightGreen2 : MonoBehaviour
{

    public Material defaultMaterial;
    public Material redMaterial;
    public Material greenMaterial;
    public Material amberMaterial;
    public Material blackMaterial;

    public Material currentMaterial;

    public float timer = 0;
    private float timerMax = 0;

    void Start()
    {
        currentMaterial = GetComponent<Renderer>().material;
    }

    public void SetToDefaultMaterial()
    {
        currentMaterial = defaultMaterial;
        Material[] materials = GetComponent<Renderer>().materials;
        materials[0] = defaultMaterial;
        GetComponent<Renderer>().materials = materials;
    }

    public void SetToBlackMaterial()
    {
        currentMaterial = blackMaterial;
        Material[] materials = GetComponent<Renderer>().materials;
        materials[0] = blackMaterial;
        GetComponent<Renderer>().materials = materials;
    }

    public void SetToRedMaterial()
    {
        currentMaterial = redMaterial;
        Material[] materials = GetComponent<Renderer>().materials;
        materials[0] = redMaterial;
        GetComponent<Renderer>().materials = materials;
    }

    public void SetToGreenMaterial()
    {
        currentMaterial = greenMaterial;
        Material[] materials = GetComponent<Renderer>().materials;
        materials[0] = greenMaterial;
        GetComponent<Renderer>().materials = materials;
    }

    public void SetToAmberMaterial()
    {
        currentMaterial = amberMaterial;
        Material[] materials = GetComponent<Renderer>().materials;
        materials[0] = amberMaterial;
        GetComponent<Renderer>().materials = materials;
    }

}
