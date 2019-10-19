using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Encoding;
using System.Xml.Linq;

public class Sender {
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

    private static void SendData(Socket s, XDocument xml) {
    	Encoding utf8 = Encoding.UTF8;
    	byte[] message = ConvertXmlToByteArray(xml, utf8);
    	byte[] bytes = new byte[256]; // for recieving

    	try {
    		int bytesSent = s.Send(message);
    		s.close();
    	}
    	catch (SocketException e) {
	        Console.WriteLine("{0} Error code: {1}.", e.Message, e.ErrorCode);
	        return (e.ErrorCode);
    	}
    }

    // change to take in entire XML document?
    private static byte[] ConvertXmlToByteArray(XDocument xml, Encoding encoding) {
    	using (MemoryStream stream = new MemoryStream())
    	{
	        XmlWriterSettings settings = new XmlWriterSettings();
	        // Add formatting and other writer options here if desired
	        settings.Encoding = encoding;
	        settings.OmitXmlDeclaration = true; // No prolog
	        using (XmlWriter writer = XmlWriter.Create(stream, settings))
	        {
	            xml.Save(writer);
	        }
	        return stream.ToArray();
    	}
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