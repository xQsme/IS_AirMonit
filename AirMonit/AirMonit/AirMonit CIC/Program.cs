using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace AirMonit_CIC
{
    class Program
    {
        private static MqttClient mClient;
        private static string[] sTopics = new[] {"dataUploader", "alarm"};
        private static string ip = "127.0.0.1";

        static void Main(string[] args)
        {
            mClient = new MqttClient(ip);
            mClient.Connect(Guid.NewGuid().ToString());
            mClient.MqttMsgPublishReceived += MClient_MqttMsgPublishReceived;
            byte[] qosLevels = {MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE};
            mClient.Subscribe(sTopics, qosLevels);
        }

        private static void MClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            if (e.Topic == sTopics[0])
            {
                Console.WriteLine("Received from DU: " + Encoding.UTF8.GetString(e.Message));
            }
            else if (e.Topic == sTopics[1])
            {
                Console.WriteLine("Received from Alarm" + Encoding.UTF8.GetString(e.Message));
            }
        }
    }
}
