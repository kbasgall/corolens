using System;
using System.Collections.Generic;
using System.Xml;

namespace org.doublecloud
{
    class XmlParsingDemo
    {
        static void Main(string[] args)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"originalv3Message.xml");

            XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);

            mgr.AddNamespace("hl7", "urn:hl7-org:v3");

            XmlNodeList vital_values = doc.DocumentElement.SelectNodes("//hl7:POLB_IN224200UV01/hl7:controlActProcess/hl7:subject/hl7:observationBattery/hl7:component1/hl7:observationEvent/hl7:value", mgr);
            XmlNodeList display_names = doc.DocumentElement.SelectNodes("//hl7:POLB_IN224200UV01/hl7:controlActProcess/hl7:subject/hl7:observationBattery/hl7:component1/hl7:observationEvent/hl7:code", mgr);

            //Console.WriteLine(vital_values.Count);

            IDictionary<string, string> dict = new Dictionary<string, string>();

            for(int i = 0; i < vital_values.Count; ++i) {
                dict.Add(display_names[i].Attributes["displayName"].Value, vital_values[i].Attributes["value"].Value + " " + vital_values[i].Attributes["unit"].Value);
            
                /*Console.Write(display_names[i].Attributes["displayName"].Value);
                Console.Write(" ");
                Console.Write(vital_values[i].Attributes["value"].Value);
                Console.Write(" ");
                Console.Write(vital_values[i].Attributes["unit"].Value);
                Console.WriteLine("");*/
            }

            foreach(KeyValuePair<string, string> entry in dict) {
                Console.Write(entry.Key);
                Console.Write(" ");
                Console.Write(entry.Value);
                Console.WriteLine("");
            }
        }
    }
}
