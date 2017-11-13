using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;
using AirMonit_DU.Models;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace AirMonit_DU
{
    class Program
    {
        private static Entry currentEntry;
        private static MqttClient mClient;
        private static String topic = "dataUploader";
        private static String ip = "127.0.0.1";

        static void Main(string[] args)
        {
            mClient = new MqttClient(ip);
            mClient.Connect(Guid.NewGuid().ToString());
            AirSensorNodeDll.AirSensorNodeDll dll = new AirSensorNodeDll.AirSensorNodeDll();
            dll.Initialize(newEntry, 100); 
        }
        public static void newEntry(String arg)
        {
            string[] array = arg.Split(';');
            if (currentEntry == null)
            {
                currentEntry = new Entry();
                currentEntry.no2 = -1;
                currentEntry.co = -1;
                currentEntry.o3 = -1;
                currentEntry.date = Convert.ToDateTime(array[3]);
                switch (array[4])
                {
                    case "Leiria":
                        currentEntry.city = Entry.City.LEIRIA;
                        break;
                    case "Coimbra":
                        currentEntry.city = Entry.City.COIMBRA;
                        break;
                    case "Lisboa":
                        currentEntry.city = Entry.City.LISBOA;
                        break;
                    case "Porto":
                        currentEntry.city = Entry.City.PORTO;
                        break;
                }
            }
            switch (array[1])
            {
                case "NO2":
                currentEntry.no2 = int.Parse(array[2]);
                break;
                case "CO":
                currentEntry.co = int.Parse(array[2]);
                break;
                case "O3":
                currentEntry.o3 = int.Parse(array[2]);
                break;
            }
            if (currentEntry.co != -1 && currentEntry.no2 != -1 && currentEntry.o3 != -1)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                string json = jss.Serialize(currentEntry);
                mClient.Publish(topic, Encoding.UTF8.GetBytes(json));
                Console.WriteLine("Uploaded by DU: " + json);
                currentEntry = null;
                Thread.Sleep(5000);
            }
        }
    }
}
