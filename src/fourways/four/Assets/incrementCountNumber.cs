using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class incrementCountNumber : MonoBehaviour {
    public static int CarC = 0;
	
	void Start () {
		
	}
	
	

	void Update () {
		
	}

    public static int getcarC()
    {
        return CarC;
    }

    public static void incrementcarC()
    {
        CarC++;
    }
}
