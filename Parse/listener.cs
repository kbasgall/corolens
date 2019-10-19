using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using System.Xml.Linq;

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
        XmlDocument doc = new XmlDocument();
        string xml;
        do
        {
            int num_bytes_recv = s.Receive(bytes);
            xml += Encoding.UTF8.GetString(bytesReceived, 0, bytes);
        } while (num_bytes_recv > 0);

        doc.LoadXml(xml);
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
