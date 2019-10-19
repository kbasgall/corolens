using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml.Linq;
using System.Xml;

public class Sender {
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

    private static void SendData(Socket s, XDocument xml) {
    	Encoding utf8 = Encoding.UTF8;
    	byte[] message = ConvertXmlToByteArray(xml, utf8);
    	byte[] bytes = new byte[256]; // for recieving

    	try {		
			int bytesSent = s.Send(message);
			s.Close();
    	}
    	catch (SocketException e) {
	        Console.WriteLine("{0} Error code: {1}.", e.Message, e.ErrorCode);
	        System.Environment.Exit(1);
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
        System.Net.IPAddress ip_addr = System.Net.IPAddress.Parse("35.2.222.117");
        int port = 60010;

        XDocument xml = XDocument.Load("originalv3Message.xml");

        try
        {
            Socket s = CreateSocket(ip_addr, port);
            SendData(s, xml);
        }
        catch(SocketException e)
        {
        	Console.WriteLine(e);
            System.Environment.Exit(1);
        }
    }
}
