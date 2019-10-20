using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Xml;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;

namespace Vitals
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public class TextboxText
    {
        public string Textdata { get; set; }
    }
    public sealed partial class MainPage : Page
    {
        String systolic_blood_pressure_val = "--";
        String diastolic_blood_pressure_val = "--";
        String heartrate_val = "--";
        String oxygen_val = "--";
        String temperature_val = "--";

        public MainPage()
        {
            this.InitializeComponent();
            Debug.WriteLine("Hello");
            blood_pressure.DataContext = new TextboxText() { Textdata = systolic_blood_pressure_val + "/" + diastolic_blood_pressure_val };
            heartrate.DataContext = new TextboxText() { Textdata = heartrate_val };
            oxygen_level.DataContext = new TextboxText() { Textdata = oxygen_val };
            temperature.DataContext = new TextboxText() { Textdata = temperature_val };


            //SimulateServer();
            //ExecuteServer();
        }

        public void SimulateServer()
        {
            String[] docs = { "test1.xml", "test2.xml", "test3.xml", "test4.xml" };
            int i = 0;
            while (true)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(@docs[i]);
                ParseDataFromSocket(doc);

                Thread.Sleep(500);

                if (i == 3) i = 0;
                else i++;
            }
        }

        public void ExecuteServer()
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

            try
            {

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

                while (true)
                {

                    Debug.WriteLine("Waiting connection ... ");

                    // Suspend while waiting for
                    // incoming connection Using
                    // Accept() method the server
                    // will accept connection of client
                    Socket clientSocket = listener.Accept();

                    // Data buffer
                    XmlDocument doc = new XmlDocument();
                    byte[] bytes = new Byte[1024];
                    string data = null;

                    while (true)
                    {

                        int numByte = clientSocket.Receive(bytes);

                        data += Encoding.UTF8.GetString(bytes,
                                                0, numByte);

                        if (numByte == 0)
                            break;
                    }

                    doc.LoadXml(data);
                    ParseDataFromSocket(doc);
                    Debug.WriteLine("Parsed data");

                    // Close client Socket using the
                    // Close() method. After closing,
                    // we can use the closed Socket
                    // for a new Client Connection
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void ParseDataFromSocket(XmlDocument doc)
        {

            XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);

            mgr.AddNamespace("hl7", "urn:hl7-org:v3");

            XmlNodeList vital_values = doc.DocumentElement.SelectNodes("//hl7:POLB_IN224200UV01/hl7:controlActProcess/hl7:subject/hl7:observationBattery/hl7:component1/hl7:observationEvent/hl7:value", mgr);
            XmlNodeList display_names = doc.DocumentElement.SelectNodes("//hl7:POLB_IN224200UV01/hl7:controlActProcess/hl7:subject/hl7:observationBattery/hl7:component1/hl7:observationEvent/hl7:code", mgr);

            for (int i = 0; i < vital_values.Count; ++i)
            {
                String curr_val = vital_values[i].Attributes["value"].Value;
                // Update individual values
                if (display_names[i].Attributes["displayName"].Value == "Body temperature")
                {
                    double fahrenheit = ((Double.Parse(curr_val) * 9) / 5) + 32;
                    String result = string.Format("{0:0.0}", Math.Truncate(fahrenheit * 10) / 10);
                    temperature_val = result;
                    temperature.DataContext = new TextboxText() { Textdata = temperature_val };
                }
                else if (display_names[i].Attributes["displayName"].Value == "Systolic blood pressure")
                {
                    systolic_blood_pressure_val = curr_val;
                    blood_pressure.DataContext = new TextboxText() { Textdata = systolic_blood_pressure_val + "/" + diastolic_blood_pressure_val };
                }
                else if(display_names[i].Attributes["displayName"].Value == "Diastolic blood pressure")
                {
                    diastolic_blood_pressure_val = curr_val;
                    blood_pressure.DataContext = new TextboxText() { Textdata = systolic_blood_pressure_val + "/" + diastolic_blood_pressure_val };
                }
                else if (display_names[i].Attributes["displayName"].Value == "Mean blood pressure")
                {

                }
                else if (display_names[i].Attributes["displayName"].Value == "Pulse rate")
                {
                    heartrate_val = curr_val;
                    heartrate.DataContext = new TextboxText() { Textdata = heartrate_val };
                }
                else if (display_names[i].Attributes["displayName"].Value == "SpO2")
                {
                    oxygen_val = curr_val;
                    oxygen_level.DataContext = new TextboxText() { Textdata = oxygen_val };
                }
            }
        }
    }
}
