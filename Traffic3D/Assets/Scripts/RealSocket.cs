using System.Net.Sockets;

class RealSocket : ISocket
{
    private Socket socket;

    public RealSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
    {
        socket = new Socket(addressFamily, socketType, protocolType);
    }

    public void Connect(string host, int port)
    {
        socket.Connect(host, port);
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
