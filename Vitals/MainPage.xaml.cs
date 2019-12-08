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

        // Upper and lower alert ranges for temperature, diastolic blood pressure, systolic blood pressure, heart rate, Sp02
        public static double temp_alert_lower_val = 90;
        public static double temp_alert_upper_val = 105;
        public static int bp_diast_alert_lower_val = 60;
        public static int bp_diast_alert_upper_val = 110;
        public static int bp_syst_alert_lower_val = 80;
        public static int bp_syst_alert_upper_val = 160;
        public static int hr_alert_lower_val = 40;
        public static int hr_alert_upper_val = 150;
        public static int sp02_alert_lower_val = 90;

        // Current vital readings
        private String _systolic_blood_pressure_val;
        private String _diastolic_blood_pressure_val;
        private String _heartrate_val;
        private String _oxygen_val;
        private String _temperature_val;
        private String _blood_pressure_val;
        private String _current_time_val;

        // Colors for UI display
        public static String alert_color = "Yellow";
        private String _heartrate_color = "HotPink";
        private String _oxygen_color = "#FF2ED813";
        private String _temperature_color = "#FF0CA5DE";
        private String _blood_pressure_color = "#FFF39320";

        // Settings
        private bool toggle_alerts = false;
        private double transparency_value = 0.1;

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

            Thread t = new Thread(SimulateServer);
            Thread t2 = new Thread(Clock);
            t2.Start();
            t.Start();
        }

        public void UpdateUI()
        {
            blood_pressure_val = systolic_blood_pressure_val + "/" + diastolic_blood_pressure_val;
            //DataContext = vitalValues;
        }

        public void Clock()
        {
            while (true)
            {
                current_time_val = DateTime.Now.ToString("h:mm:ss tt");
            }
        }

        private void TransparencySlider(object sender, RoutedEventArgs e)
        {
            Slider slider = sender as Slider;
            if(slider != null)
            {
                transparency_value = slider.Value/100;
                Debug.WriteLine(transparency_value);
                OnPropertyChanged("transparency_value");
            }
        }

        private void SubmitThresholds(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button == null) return;

            if (button.Name == "bpm_alt")
            {
                if (bpm_min.Text != "")
                {
                    int.TryParse(bpm_min.Text, out hr_alert_lower_val);
                }
                if (bpm_max.Text != "")
                {
                    int.TryParse(bpm_max.Text, out hr_alert_upper_val);
                }
            }

            if (button.Name == "diastolic_bp")
            {
                if (diastolic_bp_min.Text != "") {
                    int.TryParse(diastolic_bp_min.Text, out bp_diast_alert_lower_val);
                }
                if (diastolic_bp_max.Text != "")
                {
                    int.TryParse(diastolic_bp_max.Text, out bp_diast_alert_upper_val);
                }
            }

            else if(button.Name == "systolic_bp")
            {
                if (systolic_bp_min.Text != "")
                {
                    int.TryParse(systolic_bp_min.Text, out bp_syst_alert_lower_val);
                    bp_syst_alert_lower_val = Convert.ToInt32(systolic_bp_min.Text);
                }
                if (systolic_bp_max.Text != "")
                {
                    int.TryParse(systolic_bp_max.Text, out bp_syst_alert_upper_val);
                }
            }

            else if (button.Name == "oxygen_alt")
            {
                if (oxygen_min.Text != "")
                {
                    int.TryParse(oxygen_min.Text, out sp02_alert_lower_val);
                }
            }

            else if (button.Name == "temp_alt")
            {
                if (temp_min.Text != "")
                {
                    double.TryParse(temp_min.Text, out temp_alert_lower_val);
                    temp_alert_lower_val = Convert.ToDouble(temp_min.Text);
                }
                if (temp_max.Text != "")
                {
                    double.TryParse(temp_max.Text, out temp_alert_upper_val);
                }
            }
        }
        // Handles the Click event on the Button inside the Popup control and 
        // closes the Popup. 
        private void ClosePopupClicked(object sender, RoutedEventArgs e)
        {
            // if the Popup is open, then close it 
            if (StandardPopup.IsOpen) { StandardPopup.IsOpen = false; }
        }

        private void ToggleAlerts(object sender, RoutedEventArgs e)
        {
            toggle_alerts = !toggle_alerts;
            Debug.WriteLine("Toggled! toggle_alerts is: ", toggle_alerts.ToString());
        }

        // Handles the Click event on the Button on the page and opens the Popup. 
        private void ShowPopupOffsetClicked(object sender, RoutedEventArgs e)
        {
            // open the Popup if it isn't open already 
            if (!StandardPopup.IsOpen) { StandardPopup.IsOpen = true; }
        }

        void ClickRevert(object sender, RoutedEventArgs e)
        {
            heartrate_button.Height = double.NaN;
            heartrate_button.Width = double.NaN;

            blood_pressure_button.Height = double.NaN;
            blood_pressure_button.Width = double.NaN;

            oxygen_button.Height = double.NaN;
            oxygen_button.Width = double.NaN;

            temperature_button.Height = double.NaN;
            temperature_button.Width = double.NaN;
        }
        void OnClickHR(object sender, RoutedEventArgs e)
        {
            heartrate_button.Foreground = new SolidColorBrush(Windows.UI.Colors.Green);
            heartrate_button.Height = 0;
            heartrate_button.Width = 0;
        }
        void OnClickBP(object sender, RoutedEventArgs e)
        {
            blood_pressure_button.Foreground = new SolidColorBrush(Windows.UI.Colors.Green);
            blood_pressure_button.Height = 0;
            blood_pressure_button.Width = 0;
        }
        void OnClickO2(object sender, RoutedEventArgs e)
        {
            oxygen_button.Foreground = new SolidColorBrush(Windows.UI.Colors.Green);
            oxygen_button.Height = 0;
            oxygen_button.Width = 0;
        }
        void OnClickT(object sender, RoutedEventArgs e)
        {
            temperature_button.Foreground = new SolidColorBrush(Windows.UI.Colors.Green);
            temperature_button.Height = 0;
            temperature_button.Width = 0;
        }
        public void SimulateServer()
        {
            String[] docs = { "test1.xml", "test2.xml", "test3.xml", "test4.xml", "testv2_1.txt", "testv2_2.txt", "testv2_3.txt", "testv2_4.txt" };
            string data = "";
            int i = 0;
            int version = 3;
            while (true)
            {
                if (i == 4) {
                    version = 2;
                }
                Debug.WriteLine("Next file" + i);
                ParseDataFromSocket(docs[i], version);

                Thread.Sleep(3000);

                if (i == 7) {
                    i = 0;
                    version = 3;
                }
                else {
                    i++;
                }
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
                    }

                    Console.WriteLine(data);
                    if (data[0] == 'M' && data[1] == 'S' && data[2] == 'H')
                        data_version = 2;

                    ParseDataFromSocket(data, data_version);

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


        public void ParseDataFromSocket(string data, int version){
              if(version == 2){
                string contents = System.IO.File.ReadAllText(data);
                List<string> piped = new List<string>();
                string[] tokens = contents.Split("OBX");

                foreach (var word in tokens){
                  if(word[0] == '|')
                    piped.Add(word);
                }

                foreach (var pipe in piped){
                  string[] fields = pipe.Split('|');
                  if(fields[2] == "NM"){
                    if(fields[3] == "3446"){
                      double fahrenheit = ((Double.Parse(fields[5]) * 9) / 5) + 32;
                      String result = string.Format("{0:0.0}", Math.Truncate(fahrenheit * 10) / 10);
                      temperature_val = result;
                      if(toggle_alerts && (Convert.ToDouble(temperature_val) <= temp_alert_lower_val || Convert.ToDouble(temperature_val) >= temp_alert_upper_val))
                      {
                          temperature_color = alert_color;
                      }
                      else
                      {
                          temperature_color = "#FF0CA5DE";
                      }
                    }
                    else if(fields[3] == "19"){
                      heartrate_val = fields[5];
                      if(toggle_alerts && Convert.ToInt32(heartrate_val) <= hr_alert_lower_val || Convert.ToInt32(heartrate_val) >= hr_alert_upper_val)
                      {
                          heartrate_color = alert_color;
                      }
                      else
                      {
                          heartrate_color = "HotPink";
                      }
                    }
                    else if(fields[3] == "2"){
                      systolic_blood_pressure_val = fields[5];
                      if (toggle_alerts && (Convert.ToInt32(diastolic_blood_pressure_val) >= bp_diast_alert_upper_val || Convert.ToInt32(diastolic_blood_pressure_val) <= bp_diast_alert_lower_val ||
                          Convert.ToInt32(systolic_blood_pressure_val) >= bp_syst_alert_upper_val || Convert.ToInt32(systolic_blood_pressure_val) <= bp_syst_alert_lower_val))
                      {
                          blood_pressure_color = alert_color;
                      }
                      else
                      {
                          blood_pressure_color = "#FFF39320";
                      }
                    }
                    else if(fields[3] == "3"){
                      diastolic_blood_pressure_val = fields[5];
                      if(toggle_alerts && (Convert.ToInt32(diastolic_blood_pressure_val) >= bp_diast_alert_upper_val || Convert.ToInt32(diastolic_blood_pressure_val) <= bp_diast_alert_lower_val ||
                          Convert.ToInt32(systolic_blood_pressure_val) >= bp_syst_alert_upper_val || Convert.ToInt32(systolic_blood_pressure_val) <= bp_syst_alert_lower_val))
                      {
                          blood_pressure_color = alert_color;
                      }
                      else
                      {
                          blood_pressure_color = "#FFF39320";
                      }
                    }
                    else if(fields[3] == "4"){
                      Console.WriteLine("mean blood pressure");
                    }
                    else if(fields[3] == "14"){
                      oxygen_val = fields[5];
                      if(toggle_alerts && Convert.ToInt32(oxygen_val) <= sp02_alert_lower_val)
                      {
                          oxygen_color = alert_color;
                      }
                      else
                      {
                          oxygen_color = "#FF2ED813";
                      }
                    }
                  }
                  UpdateUI();
                }
              }
              else{
                XmlDocument doc = new XmlDocument();
                doc.Load(data);

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
                        if(toggle_alerts && (Convert.ToDouble(temperature_val) <= temp_alert_lower_val || Convert.ToDouble(temperature_val) >= temp_alert_upper_val))
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
                        if (toggle_alerts && (Convert.ToInt32(diastolic_blood_pressure_val) >= bp_diast_alert_upper_val || Convert.ToInt32(diastolic_blood_pressure_val) <= bp_diast_alert_lower_val ||
                            Convert.ToInt32(systolic_blood_pressure_val) >= bp_syst_alert_upper_val || Convert.ToInt32(systolic_blood_pressure_val) <= bp_syst_alert_lower_val))
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
                        if(toggle_alerts && (Convert.ToInt32(diastolic_blood_pressure_val) >= bp_diast_alert_upper_val || Convert.ToInt32(diastolic_blood_pressure_val) <= bp_diast_alert_lower_val ||
                            Convert.ToInt32(systolic_blood_pressure_val) >= bp_syst_alert_upper_val || Convert.ToInt32(systolic_blood_pressure_val) <= bp_syst_alert_lower_val))
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
                        if(toggle_alerts && (Convert.ToInt32(heartrate_val) <= hr_alert_lower_val || Convert.ToInt32(heartrate_val) >= hr_alert_upper_val))
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
                        if(toggle_alerts && Convert.ToInt32(oxygen_val) <= sp02_alert_lower_val)
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
