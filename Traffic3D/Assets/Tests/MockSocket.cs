using System.Text;

class MockSocket : ISocket
{
    public void Connect(string host, int port)
    {
        // Nothing to connect to
    }

    public int Receive(byte[] buffer)
    {
        buffer = Encoding.UTF8.GetBytes("0");
        return buffer.Length;
    }

    public int Send(byte[] buffer)
    {
        buffer = Encoding.UTF8.GetBytes("Screenshot/shot1.png");
        return buffer.Length;
    }
}
