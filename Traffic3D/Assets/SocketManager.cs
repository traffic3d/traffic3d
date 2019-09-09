using System;
using System.Net.Sockets;
using UnityEngine;

public class SocketManager
{

    private static SocketManager instance;

    public static SocketManager GetInstance()
    {
        if (instance == null)
        {
            instance = new SocketManager();
        }
        return instance;
    }

    public const int PORT = 13000;

    private Socket socket;

    private SocketManager()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    public bool Connect()
    {
        try
        {
            socket.Connect("localhost", PORT);
            Debug.Log("Established tcpSocket Connection with Python");
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        return false;
    }

    public Socket GetSocket()
    {
        return socket;
    }

    public int Receive(byte[] buffer)
    {
        return socket.Receive(buffer);
    }

    public int Send(byte[] buffer)
    {
        return socket.Send(buffer);
    }

}
