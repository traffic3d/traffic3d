using System;
using System.Net.Sockets;
using System.Text;
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

    /// <value>The port of the socket.</value>
    public const int PORT = 13000;

    private ISocket socket;

    private SocketManager()
    {
        socket = new RealSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    /// <summary>
    /// Connects the socket to the specified port. This is used to connect the python script to Unity.
    /// </summary>
    /// <returns>true if connected.</returns>
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

    /// <summary>
    /// Get the socket.
    /// </summary>
    /// <returns>Returns the socket.</returns>
    public ISocket GetSocket()
    {
        return socket;
    }

    /// <summary>
    /// Sets the main socket.
    /// Used to make mock sockets if needed for tests.
    /// </summary>
    /// <param name="socket">The socket to set.</param>
    public void SetSocket(ISocket socket)
    {
        this.socket = socket;
    }

    /// <summary>
    /// Receives data from the Socket into a receive buffer.
    /// </summary>
    /// <param name="buffer">An array of type Byte that is the storage location for the received data.</param>
    /// <returns>The number of bytes received.</returns>
    public int Receive(byte[] buffer)
    {
        return socket.Receive(buffer);
    }

    /// <summary>
    /// Receives the data directly into a string.
    /// </summary>
    /// <returns>The string that has been converted from the byte array.</returns>
    public string ReceiveString()
    {
        byte[] bytes = new byte[512];
        Receive(bytes);
        return Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// Receives the data directly into an int.
    /// </summary>
    /// <returns>An int that has been converted from the byte array.</returns>
    public int ReceiveInt()
    {
        return int.Parse(ReceiveString());
    }

    /// <summary>
    /// Sends data to the connected Socket.
    /// </summary>
    /// <param name="buffer">An array of type Byte that contains the data to be sent.</param>
    /// <returns>The number of bytes sent to the Socket.</returns>
    public int Send(byte[] buffer)
    {
        return socket.Send(buffer);
    }

    /// <summary>
    /// Sends a string to the socket.
    /// </summary>
    /// <param name="str">The string to send.</param>
    /// <returns>The number of bytes sent to the Socket.</returns>
    public int Send(string str)
    {
        return Send(Encoding.UTF8.GetBytes(str));
    }
}
