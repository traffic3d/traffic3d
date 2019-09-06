using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightGreen1 : MonoBehaviour
{

    public Material greenMaterial;
    public Material blackMaterial;

    public Material currentMaterial;

    public float timer = 0;   //we donot need the timer for now
    public float timerMax = 0; // we dont use this for now

    void Start()
    {
        currentMaterial = GetComponent<Renderer>().material;
    }

    public void SetToGreenMaterial()
    {
        currentMaterial = greenMaterial;
        Material[] materials = GetComponent<Renderer>().materials;
        materials[0] = greenMaterial;
        GetComponent<Renderer>().materials = materials;
    }

    public void SetToBlackMaterial()
    {
        currentMaterial = blackMaterial;
        Material[] materials = GetComponent<Renderer>().materials;
        materials[0] = blackMaterial;
        GetComponent<Renderer>().materials = materials;
    }

    private bool HasWaited(float seconds)
    {
        timerMax = seconds;
        timer += Time.deltaTime;

        if (timer >= timerMax)
        {
            timer = 0;
            return true;
        }
        return false;
    }

}
