using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SocketConnection : MonoBehaviour
{

    public const int port = 13004;
    public static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    void Start()
    {

        socket.Connect("localhost", port);
        print("Established tcpSocket Connection with Python");

        ChangeScene1();

    }

    public void ChangeScene1()
    {
        SceneManager.LoadScene("Demo AI car2");
    }

}
