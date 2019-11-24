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
using System.Collections.Generic;
using Windows.UI.Xaml.Media;

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

        public static String alert_color = "White";

        private String _systolic_blood_pressure_val;
        private String _diastolic_blood_pressure_val;
        private String _heartrate_val;
        private String _oxygen_val;
        private String _temperature_val;
        private String _blood_pressure_val;
        private String _current_time_val;

        private String _heartrate_color = "HotPink";
        private String _oxygen_color = "#FF2ED813";
        private String _temperature_color = "#FF0CA5DE";
        private String _blood_pressure_color = "#FFF39320";
        public String current_time_val
        {
            get { return _current_time_val; }
            set
            {
                _current_time_val = value;
                OnPropertyChanged();
            }
        }
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

        public String heartrate_color
        {
            get { return _heartrate_color; }
            set
            {
                _heartrate_color = value;
                OnPropertyChanged();
            }
        }
        public String oxygen_color
        {
            get { return _oxygen_color; }
            set
            {
                _oxygen_color = value;
                OnPropertyChanged();
            }
        }
        public String temperature_color
        {
            get { return _temperature_color; }
            set
            {
                _temperature_color = value;
                OnPropertyChanged();
            }
        }

        public String blood_pressure_color
        {
            get { return _blood_pressure_color; }
            set
            {
                _blood_pressure_color = value;
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
            systolic_blood_pressure_val = diastolic_blood_pressure_val = heartrate_val = oxygen_val = temperature_val = blood_pressure_val = "0";
            //DataContext = vitalValues;

            Thread t = new Thread(ExecuteServer);
            Thread t2 = new Thread(Clock);
            t2.Start();
            t.Start();
        }

        public void UpdateUI()
        {
            blood_pressure_val = systolic_blood_pressure_val + "/" + diastolic_blood_pressure_val;
            //DataContext = vitalValues;
            Debug.WriteLine("Updated UI.....heartrate " + heartrate_val);
        }

        public void Clock()
        {
            while (true)
            {
                current_time_val = DateTime.Now.ToString("h:mm:ss tt");
            }
        }

        void ClickRevert(object sender, RoutedEventArgs e)
        {
            heartrate_button.Height = 200;
            heartrate_button.Width = 200;
        }
        void OnClick(object sender, RoutedEventArgs e)
        {
            heartrate_button.Foreground = new SolidColorBrush(Windows.UI.Colors.Green);
            heartrate_button.Height = 0;
            heartrate_button.Width = 0;
        }
        public void SimulateServer()
        {
            String[] docs = { "test1.xml", "test2.xml", "test3.xml", "test4.xml" };
            string data = "";
            int i = 0;
            while (true)
            {
                Debug.WriteLine("Next file" + i);
                ParseDataFromSocket(@docs[i], 3);

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
            TcpListener server = null;

            try{
              Int32 port = 11111;
              IPAddress localAddr = IPAddress.Parse("35.2.239.229");

              server = new TcpListener(localAddr, port);

              server.Start();

              Byte[] bytes = new Byte[1024];
              String data = null;

              while(true){
                Console.Write("Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                // You could also user server.AcceptSocket() here.
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");

                data = null;

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                int i;

                // Loop to receive all the data sent by the client.
                while((i = stream.Read(bytes, 0, bytes.Length))!=0){
                  // Translate data bytes to a ASCII string.
                  data += System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                }

                Console.WriteLine(data);
                
                if (data[0] == 'M' && data[1] == 'S' && data[2] == 'H')
                    data_version = 2;

                ParseDataFromSocket(data, data_version);

                // Shutdown and end connection
                client.Close();
              }
            }
            catch(SocketException e){
              Console.WriteLine("SocketException: {0}", e);
            }
            finally{
              // Stop listening for new clients.
              server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }


        public void ParseDataFromSocket(string data, int version){
              if(version == 2){
                IDictionary<string, string> dict = new Dictionary<string, string>();
                List<string> piped = new List<string>();
                string[] tokens = data.Split("OBX");
                foreach (var word in tokens){
                  if(word[0] == '|')
                    piped.Add(word);
                }

                foreach (var pipe in piped){
                  string[] fields = pipe.Split('|');
                  if(fields[2] == "NM"){
                    if(fields[3] == "3446"){
                      dict.Add("Body temperature", fields[5]);
                    }
                    else if(fields[3] == "19"){
                      dict.Add("Pulse rate", fields[5]);
                    }
                    else if(fields[3] == "2"){
                      dict.Add("Systolic blood pressure", fields[5]);
                    }
                    else if(fields[3] == "3"){
                      dict.Add("Diastolic blood pressure", fields[5]);
                    }
                    else if(fields[3] == "4"){
                      dict.Add("Mean blood pressure", fields[5]);
                    }
                    else if(fields[3] == "14"){
                      dict.Add("SpO2", fields[5]);
                    }
                  }
                }
              }
              else{
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(data);

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
                        if(Convert.ToDouble(temperature_val) <= 90 || Convert.ToDouble(temperature_val) >= 105)
                        {
                            temperature_color = alert_color;
                        }
                        else
                        {
                            temperature_color = "#FF0CA5DE";
                        }
                    }
                    else if (display_names[i].Attributes["displayName"].Value == "Systolic blood pressure")
                    {
                        systolic_blood_pressure_val = curr_val;
                        if (Convert.ToInt32(diastolic_blood_pressure_val) >= 110 || Convert.ToInt32(diastolic_blood_pressure_val) <= 60 ||
                            Convert.ToInt32(systolic_blood_pressure_val) >= 160 || Convert.ToInt32(systolic_blood_pressure_val) <= 80)
                        {
                            blood_pressure_color = alert_color;
                        }
                        else
                        {
                            blood_pressure_color = "#FFF39320";
                        }
                    }
                    else if (display_names[i].Attributes["displayName"].Value == "Diastolic blood pressure")
                    {
                        diastolic_blood_pressure_val = curr_val;
                        if(Convert.ToInt32(diastolic_blood_pressure_val) >= 110 || Convert.ToInt32(diastolic_blood_pressure_val) <= 60 ||
                            Convert.ToInt32(systolic_blood_pressure_val) >= 160 || Convert.ToInt32(systolic_blood_pressure_val) <= 80)
                        {
                            blood_pressure_color = alert_color;
                        }
                        else
                        {
                            blood_pressure_color = "#FFF39320";
                        }
                    }
                    else if (display_names[i].Attributes["displayName"].Value == "Mean blood pressure")
                    {

                    }
                    else if (display_names[i].Attributes["displayName"].Value == "Pulse rate")
                    {
                        heartrate_val = curr_val;
                        if(Convert.ToInt32(heartrate_val) <= 40 || Convert.ToInt32(heartrate_val) >= 150)
                        {
                            heartrate_color = alert_color;
                        }
                        else
                        {
                            heartrate_color = "HotPink";
                        }
                    }
                    else if (display_names[i].Attributes["displayName"].Value == "SpO2")
                    {
                        oxygen_val = curr_val;
                        if(Convert.ToInt32(oxygen_val) <= 90)
                        {
                            oxygen_color = alert_color;
                        }
                        else
                        {
                            oxygen_color = "#FF2ED813";
                        }
                    }

                    UpdateUI();
                }
              }
            }
        }
}
