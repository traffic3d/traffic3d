using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChange : MonoBehaviour
{

    public Material Material1;
    public Material Material2;
    public Material Material3;
    public Material Material4;

    public Material[] mat;
    public Material CM;

    private float timer = 0;
    private float timerMax = 0;

    void Start()
    {

        CM = GetComponent<Renderer>().material;

    }


    void Update()
    {

        Materialchange();

    }

    private void Materialchange()
    {
        if (CM == Material1)
        {
            if (!Waited(10))
            {
                return;
            }
            CM = Material2;
            Material[] myarr = GetComponent<Renderer>().materials;
            myarr[0] = Material2;

            GetComponent<Renderer>().materials = myarr;
        }

        else if (CM == Material2)

        {
            if (!Waited(1))
            {
                return;
            }
            CM = Material3;
            Material[] myarr = GetComponent<Renderer>().materials;
            myarr[0] = Material3;

            GetComponent<Renderer>().materials = myarr;
        }

        else if (CM == Material3)

        {
            if (!Waited(10))
            {
                return;
            }
            CM = Material4;
            Material[] myarr = GetComponent<Renderer>().materials;
            myarr[0] = Material4;

            GetComponent<Renderer>().materials = myarr;
        }

        else if (CM == Material4)
        {
            if (!Waited(1))
            {
                return;
            }

            CM = Material1;
            Material[] myarr = GetComponent<Renderer>().materials;
            myarr[0] = Material1;

            GetComponent<Renderer>().materials = myarr;
        }

        else
        {

            if (!Waited(1))
            {
                return;
            }

            CM = Material1;
            Material[] myarr = GetComponent<Renderer>().materials;
            myarr[0] = Material1;

            GetComponent<Renderer>().materials = myarr;
        }
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
