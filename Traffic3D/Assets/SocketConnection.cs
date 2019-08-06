using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Globalization;
using UnityEngine.SceneManagement;



public class SocketConnection : MonoBehaviour
{

    public const int port = 13004;
    public static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


    void Start()
    {

        socket.Connect("localhost", port);
        print("Established tcpSocket Connection with Python");

        changescene1();
        print("change scene 1 CALLED");

    }

    void Update()
    {

    }

    public void changescene1()
    {
        SceneManager.LoadScene("Demo AI car2");
    }

}
