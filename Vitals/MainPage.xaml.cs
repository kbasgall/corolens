using System;
using Windows.UI.Xaml.Controls;
using System.Xml;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;

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
        public VitalValues vitalValues;

        public class VitalValues
        {
            public String systolic_blood_pressure_val { get; set; }
            public String diastolic_blood_pressure_val { get; set; }
            public String heartrate_val { get; set; }
            public String oxygen_val { get; set; }
            public String temperature_val { get; set; }
            public String blood_pressure_val { get; set; }

            public VitalValues()
            {
                systolic_blood_pressure_val = diastolic_blood_pressure_val = heartrate_val = oxygen_val = temperature_val = blood_pressure_val = "--";
            }

            public VitalValues UpdateValues(String systolic_in, String diastolic_in, String heartrate_in, String oxygen_in, String temperature_in)
            {
                systolic_blood_pressure_val = systolic_in;
                diastolic_blood_pressure_val = diastolic_in;
                heartrate_val = heartrate_in;
                return this;
            }
        }        
        
        public MainPage()
        {
            this.InitializeComponent();
            Debug.WriteLine("Hello");
            vitalValues = new VitalValues();
            DataContext = vitalValues;

            //Task.Run(() => SimulateServer());
            //ExecuteServer();
        }
        void OnClickHandler(object sender, RoutedEventArgs e)
        {
            textBlock1.Text = "Beginning!";
            //SimulateServer();
        }

        public void UpdateUI(VitalValues vitalValues)
        {
            vitalValues.blood_pressure_val = vitalValues.systolic_blood_pressure_val + "/" + vitalValues.diastolic_blood_pressure_val;
            DataContext = vitalValues;
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
                    //ParseDataFromSocket(doc);
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
                    vitalValues.temperature_val = result;
                }
                else if (display_names[i].Attributes["displayName"].Value == "Systolic blood pressure")
                {
                    vitalValues.systolic_blood_pressure_val = curr_val;
                }
                else if (display_names[i].Attributes["displayName"].Value == "Diastolic blood pressure")
                {
                    vitalValues.diastolic_blood_pressure_val = curr_val;
                }
                else if (display_names[i].Attributes["displayName"].Value == "Mean blood pressure")
                {

                }
                else if (display_names[i].Attributes["displayName"].Value == "Pulse rate")
                {
                    vitalValues.heartrate_val = curr_val;
                }
                else if (display_names[i].Attributes["displayName"].Value == "SpO2")
                {
                    vitalValues.oxygen_val = curr_val;
                }

                UpdateUI(vitalValues);
            }
        }
    }
}
