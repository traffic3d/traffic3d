public interface ISocket
{
    void Connect(string host, int port);
    int Receive(byte[] buffer);
    int Send(byte[] buffer);
}
