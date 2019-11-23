// A C# Program for Server
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Server {

class Program {

// Main Method
static void Main(string[] args)
{
	ExecuteServer();
}

public static void ExecuteServer()
{
	// Establish the local endpoint
	// for the socket. Dns.GetHostName
	// returns the name of the host
	// running the application.
	IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
	IPAddress ipAddr = ipHost.AddressList[0];
	IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);

	// Creation TCP/IP Socket using
	// Socket Class Costructor
	Socket listener = new Socket(ipAddr.AddressFamily,
				SocketType.Stream, ProtocolType.Tcp);

	try {

		// Using Bind() method we associate a
		// network address to the Server Socket
		// All client that will connect to this
		// Server Socket must know this network
		// Address
		listener.Bind(localEndPoint);

		// Using Listen() method we create
		// the Client list that will want
		// to connect to Server
		listener.Listen(10);

		while (true) {

			Console.WriteLine("Waiting connection ... ");

			// Suspend while waiting for
			// incoming connection Using
			// Accept() method the server
			// will accept connection of client
			Socket clientSocket = listener.Accept();

			// Data buffer
      XmlDocument doc = new XmlDocument();
			byte[] bytes = new Byte[1024];
			string data = null;

			while (true) {

				int numByte = clientSocket.Receive(bytes);

				data += Encoding.UTF8.GetString(bytes,
										0, numByte);

				if (numByte == 0)
					break;
			}

			IDictionary<string, string> parsed_data = new Dictionary<string, string>();


      //Console.WriteLine(data);
      //doc.LoadXml(data);
      //doc.Save("data.xml");

			// IDictionary<string, string> parsed_data = ParseData(doc);
      //
			// foreach(KeyValuePair<string, string> entry in parsed_data) {
			// 		Console.Write(entry.Key);
			// 		Console.Write(" ");
			// 		Console.Write(entry.Value);
			// 		Console.WriteLine("");
			// }

			// Close client Socket using the
			// Close() method. After closing,
			// we can use the closed Socket
			// for a new Client Connection
			clientSocket.Shutdown(SocketShutdown.Both);
			clientSocket.Close();
		}
	}

	catch (Exception e) {
		Console.WriteLine(e.ToString());
	}
}

public static IDictionary<string, string> ParseData(XmlDocument doc){
	XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);

	mgr.AddNamespace("hl7", "urn:hl7-org:v3");

	XmlNodeList vital_values = doc.DocumentElement.SelectNodes("//hl7:POLB_IN224200UV01/hl7:controlActProcess/hl7:subject/hl7:observationBattery/hl7:component1/hl7:observationEvent/hl7:value", mgr);
	XmlNodeList display_names = doc.DocumentElement.SelectNodes("//hl7:POLB_IN224200UV01/hl7:controlActProcess/hl7:subject/hl7:observationBattery/hl7:component1/hl7:observationEvent/hl7:code", mgr);

	IDictionary<string, string> dict = new Dictionary<string, string>();



	for(int i = 0; i < vital_values.Count; ++i) {

			if (display_names[i].Attributes["displayName"].Value == "Body temperature") {
					double celcius = Double.Parse(vital_values[i].Attributes["value"].Value);
					double temp = ((celcius * 9) / 5) + 32;
					string strValue = temp.ToString("N2");

					dict.Add(display_names[i].Attributes["displayName"].Value, strValue);

			}
			else {
					dict.Add(display_names[i].Attributes["displayName"].Value, vital_values[i].Attributes["value"].Value);
			}
	}

	return dict;
}

}
}
