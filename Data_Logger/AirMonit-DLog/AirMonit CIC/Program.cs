using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using AirMonit_CIC.Models;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace AirMonit_CIC
{
    class Program
    {
        private static Entry currentEntry;
        private static MqttClient mClient;
        private static string[] sTopics = new[] {"dataUploader"};
        private static string ip = "127.0.0.1";

        static void Main(string[] args)
        {
            mClient = new MqttClient(ip);
            mClient.Connect(Guid.NewGuid().ToString());
            mClient.MqttMsgPublishReceived += MClient_MqttMsgPublishReceived;
            byte[] qosLevels = {MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE};
            mClient.Subscribe(sTopics, qosLevels);
        }

        private static void MClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Console.WriteLine("Received: " + Encoding.UTF8.GetString(e.Message));
            string[] array = Encoding.UTF8.GetString(e.Message).Split(';');
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
                    currentEntry.no2Alarm = false;
                    break;
                case "CO":
                    currentEntry.co = int.Parse(array[2]);
                    currentEntry.no2Alarm = false;
                    break;
                case "O3":
                    currentEntry.o3 = int.Parse(array[2]);
                    currentEntry.no2Alarm = false;
                    break;

            }
            if (currentEntry.co != -1 && currentEntry.no2 != -1 && currentEntry.o3 != -1)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                string json = jss.Serialize(currentEntry);
                mClient.Publish("alarm", Encoding.UTF8.GetBytes(json));
                Console.WriteLine("Sent to alarm: " + json);
                currentEntry = null;
            }
        }
    }
}
