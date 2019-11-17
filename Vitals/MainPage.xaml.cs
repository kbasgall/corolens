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
using Windows.UI.Core;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Vitals
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public class TextboxText
    {
        public string Textdata { get; set; }
    }
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private String _systolic_blood_pressure_val;
        private String _diastolic_blood_pressure_val;
        private String _heartrate_val;
        private String _oxygen_val;
        private String _temperature_val;
        private String _blood_pressure_val;

        public String systolic_blood_pressure_val {
            get { return _systolic_blood_pressure_val; }
            set
            {
                _systolic_blood_pressure_val = value;
                OnPropertyChanged();
            }
        }
        public String diastolic_blood_pressure_val
        {
            get { return _diastolic_blood_pressure_val; }
            set
            {
                _diastolic_blood_pressure_val = value;
                OnPropertyChanged();
            }
        }
        public String heartrate_val
        {
            get { return _heartrate_val; }
            set
            {
                _heartrate_val = value;
                OnPropertyChanged();
            }
        }
        public String oxygen_val
        {
            get { return _oxygen_val; }
            set
            {
                _oxygen_val = value;
                OnPropertyChanged();
            }
        }
        public String temperature_val
        {
            get { return _temperature_val; }
            set
            {
                _temperature_val = value;
                OnPropertyChanged();
            }
        }
        public String blood_pressure_val
        {
            get { return _blood_pressure_val; }
            set
            {
                _blood_pressure_val = value;
                OnPropertyChanged();
            }
        }

        public async Task OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            );
        }


        public MainPage()
        {
            this.InitializeComponent();
            Debug.WriteLine("Hello");
            systolic_blood_pressure_val = diastolic_blood_pressure_val = heartrate_val = oxygen_val = temperature_val = blood_pressure_val = "--";
            //DataContext = vitalValues;

            Thread t = new Thread(new ParameterizedThreadStart(SimulateServer));
            t.Start();
            //ExecuteServer();
        }

        public void UpdateUI()
        {
            blood_pressure_val = systolic_blood_pressure_val + "/" + diastolic_blood_pressure_val;
            //DataContext = vitalValues;
            Debug.WriteLine("Updated UI.....heartrate " + heartrate_val);
        }

        public void SimulateServer(int data_version)
        {
            String[] docs = { "test1.xml", "test2.xml", "test3.xml", "test4.xml" };
            string data = "";
            int i = 0;
            while (true)
            {
                Debug.WriteLine("Next file" + i);
                if(data_version == 2){
                  ParseDataFromSocketv2(data);
                }
                else{
                  XmlDocument doc = new XmlDocument();
                  doc.Load(@docs[i]);
                  ParseDataFromSocketv3(doc);
                }

                Thread.Sleep(5000);

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
            int data_version = 3;
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
                    byte[] bytes = new Byte[1024];
                    string data = null;

                    while (true)
                    {

                        int numByte = clientSocket.Receive(bytes);

                        data += Encoding.UTF8.GetString(bytes,
                                                0, numByte);

                        if (numByte == 0)
                            break;
                        if(data == 'MSH')
                            data_version = 2;
                    }

                    if(data_version == 2){
                      ParseDataFromSocketv2(data);
                    }
                    else{
                      XmlDocument doc = new XmlDocument();
                      doc.LoadXml(data);
                      ParseDataFromSocketv3(doc);
                    }
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

        public void ParseDataFromSocketv2(string data){
          
        }

        public void ParseDataFromSocketv3(XmlDocument doc)
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
                }
                else if (display_names[i].Attributes["displayName"].Value == "Systolic blood pressure")
                {
                    systolic_blood_pressure_val = curr_val;
                }
                else if (display_names[i].Attributes["displayName"].Value == "Diastolic blood pressure")
                {
                    diastolic_blood_pressure_val = curr_val;
                }
                else if (display_names[i].Attributes["displayName"].Value == "Mean blood pressure")
                {

                }
                else if (display_names[i].Attributes["displayName"].Value == "Pulse rate")
                {
                    heartrate_val = curr_val;
                }
                else if (display_names[i].Attributes["displayName"].Value == "SpO2")
                {
                    oxygen_val = curr_val;
                }

                UpdateUI();
            }
        }
    }
}
