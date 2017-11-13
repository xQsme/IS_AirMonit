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
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace AirMonit_DU
{
    class Program
    {
        private static MqttClient mClient;
        private static String topic = "dataUploader";
        private static String ip = "127.0.0.1";
        private static int count = 0;

        static void Main(string[] args)
        {
            mClient = new MqttClient(ip);
            mClient.Connect(Guid.NewGuid().ToString());
            AirSensorNodeDll.AirSensorNodeDll dll = new AirSensorNodeDll.AirSensorNodeDll();
            dll.Initialize(newEntry, 100); 
        }
        public static void newEntry(String arg)
        {
            mClient.Publish(topic, Encoding.UTF8.GetBytes(arg));
            count++;
            Console.WriteLine(arg);
            if (count == 12)
            {
                count = 0;
                //envia a cada minuto dados de todos os parametros para todas as cidades
                Thread.Sleep(60000);
            }
        }
    }
}
