using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLaction11 : MonoBehaviour
{

    public Material material3; //green
    public Material material5; //black

    public Material CM;

    public float timer = 0;   //we donot need the timer for now
    public float timerMax = 0; // we dont use this for now

    void Start()
    {

        CM = GetComponent<Renderer>().material;
    }

    public void materialchangeGREEN1()
    {

        CM = material3;
        Material[] myarr = GetComponent<Renderer>().materials;
        myarr[0] = material3;
        GetComponent<Renderer>().materials = myarr;
    }

    public void materialchangeblack()
    {

        CM = material5;
        Material[] myarr = GetComponent<Renderer>().materials;
        myarr[0] = material5;
        GetComponent<Renderer>().materials = myarr;
    }

    void Update()
    {

    }

    private bool Waited(float seconds)
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
