using System;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using uPLibrary.Networking.M2Mqtt;
using IAirEntries;
using System.Collections.Generic;

namespace AirMonit_DU
{
    class Program
    {
        private static Entry currentEntry;
        private static MqttClient mClient;
        private static String topic = "dataUploader";
        private static String ip = "127.0.0.1";
        private static JavaScriptSerializer jss;
        private static string json;
        private static string[] array;

        static void Main(string[] args)
        {
            mClient = new MqttClient(ip);
            mClient.Connect(Guid.NewGuid().ToString());
            AirSensorNodeDll.AirSensorNodeDll dll = new AirSensorNodeDll.AirSensorNodeDll();
            currentEntry = new Entry();
            jss = new JavaScriptSerializer();


            //Inicializar variaveis a usar no newEntry antes de chamar o dll!
            dll.Initialize(newEntry, 1000); 
        }
        public static void newEntry(String arg)
        {
            //NO2; 123; 12-4-2017; Leiria
            array = arg.Split(';');

            try
            {
                currentEntry.name = array[1];
                currentEntry.val = int.Parse(array[2]);
                currentEntry.date = Convert.ToDateTime(array[3]);
                currentEntry.city =  array[4];

            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to parse Entry: " + ex.Message);
            }
            
            json = jss.Serialize(currentEntry);
            mClient.Publish(topic, Encoding.UTF8.GetBytes(json));
            Console.WriteLine("Uploaded by DU: " + json);
        }
    }
}
