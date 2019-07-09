using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using UnityEngine.SceneManagement;


public class globalsoc : MonoBehaviour
{

    void Start()
    {

        print("scene1 socket one loaded");

        changeScene2();
    }


    public void changeScene2()

    {
        SceneManager.LoadSceneAsync("Scene2");
    }


    void Update()
    {

    }
}
