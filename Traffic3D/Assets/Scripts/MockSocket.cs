using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class MockSocket : ISocket
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
            string screenshotPath = System.IO.Path.Combine(Application.dataPath, "Screenshots");
            if (!Directory.Exists(screenshotPath))
            {
                Directory.CreateDirectory(screenshotPath);
            }
            PushDataIntoBuffer(buffer, screenshotPath);
        }
        else
        {
            List<PythonManager.PythonAction> pythonActionsList = new List<PythonManager.PythonAction>();
            foreach (Junction junction in TrafficLightManager.GetInstance().GetJunctions())
            {
                pythonActionsList.Add(new PythonManager.PythonAction(junction.junctionId, RandomNumberGenerator.GetInstance().Range(0, junction.GetJunctionStates().Length)));
            }
            PythonManager.PythonActions pythonActions = new PythonManager.PythonActions();
            pythonActions.actions = pythonActionsList.ToArray();
            PushDataIntoBuffer(buffer, JsonUtility.ToJson(pythonActions));
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
