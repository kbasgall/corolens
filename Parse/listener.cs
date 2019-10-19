using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml;

public class Listener
{
    private static Socket CreateSocket(System.Net.IPAddress ip_addr, int port)
    {
        Socket s = new Socket(System.Net.Sockets.SocketType.Stream, ProtocolType.Tcp);
        s.Connect(ip_addr, port);
        if (!s.Connected)
        {
            Console.WriteLine("Socket didn't connect.");
        }
        return s;
    }

    private static XmlDocument RecvData(Socket s)
    {
        Byte[] bytes = new Byte[256];
        XmlDocument doc = new XmlDocument();
        string data = "";
        int num_bytes_recv;
        do
        {
            num_bytes_recv = s.Receive(bytes);
            data += Encoding.UTF8.GetString(bytes, 0, num_bytes_recv);
        } while (num_bytes_recv > 0);

        doc.LoadXml(data);

        return doc;
    }

    public static void Main(string[] args)
    {
        System.Net.IPAddress ip_addr = IPAddress.Parse("127.0.0.1");
        int port = 8000;
        try
        {
            Socket s = CreateSocket(ip_addr, port);
            XmlDocument resp = RecvData(s);
        }
        catch(SocketException e)
        {
            Console.WriteLine(e);
        }
    }
}
