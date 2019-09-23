using System.Text;
using UnityEngine;

class MockSocket : ISocket
{
    private int receiveCounter;
    private int sendCounter;

    public MockSocket()
    {
        receiveCounter = 0;
        sendCounter = 0;
    }

    public void Connect(string host, int port)
    {
        // Nothing to connect to
    }

    public int Receive(byte[] buffer)
    {
        if (receiveCounter == 0)
        {
            buffer = Encoding.UTF8.GetBytes(System.IO.Path.Combine(Application.dataPath, "Screenshots"));
        }
        else
        {
            buffer = Encoding.UTF8.GetBytes("0");
        }
        receiveCounter++;
        return buffer.Length;
    }

    public int Send(byte[] buffer)
    {
        sendCounter++;
        buffer = Encoding.UTF8.GetBytes("shot" + sendCounter + ".png");
        return buffer.Length;
    }
}
