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
            PushDataIntoBuffer(buffer, System.IO.Path.Combine(Application.dataPath, "Screenshots"));
        }
        else
        {
            int trafficLightAmount = TrafficLightManager.GetInstance().GetTrafficLights().Length;
            int trafficLightToChange = 0;
            if (trafficLightAmount != 0)
            {
                trafficLightToChange = receiveCounter % TrafficLightManager.GetInstance().GetTrafficLights().Length;
            }
            PushDataIntoBuffer(buffer, "" + trafficLightToChange);
        }
        receiveCounter++;
        return buffer.Length;
    }

    public int Send(byte[] buffer)
    {
        sendCounter++;
        return buffer.Length;
    }

    private void PushDataIntoBuffer(byte[] buffer, string dataString)
    {
        byte[] data = Encoding.UTF8.GetBytes(dataString);
        for (int i = 0; i < data.Length; i++)
        {
            if (buffer.Length > i)
            {
                buffer[i] = data[i];
            }
        }
    }

}
