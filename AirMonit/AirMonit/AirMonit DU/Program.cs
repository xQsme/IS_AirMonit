﻿using System;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using AirMonit_DU.Models;
using uPLibrary.Networking.M2Mqtt;

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
            if (currentEntry == null || currentEntry.city != (Entry.City)Enum.Parse(typeof(Entry.City), array[4].ToUpper()))
            {
                currentEntry = new Entry();
                currentEntry.no2 = -1;
                currentEntry.co = -1;
                currentEntry.o3 = -1;
                currentEntry.date = Convert.ToDateTime(array[3]);
                currentEntry.city= (Entry.City) Enum.Parse(typeof(Entry.City), array[4].ToUpper());
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
