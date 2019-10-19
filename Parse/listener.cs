using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;

public class Listener
{
    private static Socket CreateSocket(int ip_addr, int port)
    {
        Socket s = Socket(System.Net.Sockets.SocketType.Stream, ProtocolType.Tcp);
        s.Connect(System.Net.IPAddress(ip_addr), port);
        if (!s.Connected)
        {
            Console.WriteLine("Socket didn't connect.");
            throw SocketError();
        }
        return s;
    }

    private static RecvData(Socket s)
    {
        Byte[] bytes = new Byte[256];
        string data;
        do
        {
            int num_bytes_recv = s.Receive(bytes);
            data += Encoding.ASCII.GetString(bytesReceived, 0, bytes);
        } while (num_bytes_recv > 0);

    }

    public static void Main(string[] args)
    {
        System.Net.IPAddress ip_addr;
        int port = 0;
        try
        {
            Socket s = CreateSocket(ip_addr, port);
            RecvData(s);
        }
        catch(SocketError)
        {
            exit(1);
        }
    }
}