using AirMonit_Alarm.model;
using System;
using System.Text;
using System.Web.Script.Serialization;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace AirMonit_Alarm
{

    class MessagingHandler
    {
        private static MqttClient mClient;
        private static string[] sTopics = new[] { "dataUploader", "alarm" };
        private static string ip = Properties.Settings.Default.BrokerIP;

        public MessagingHandler(string[] sTopics, string ip)
        {
            mClient = new MqttClient(ip);
            mClient.Connect(Guid.NewGuid().ToString());
            mClient.MqttMsgPublishReceived += MClient_MqttMsgPublishReceived;
            byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE };
            mClient.Subscribe(sTopics, qosLevels);
        }

        private static void MClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            if (e.Topic == sTopics[0])
            {
                Entry entry;
                String json = Encoding.UTF8.GetString(e.Message);
                Console.WriteLine("Received from DU: " + json);

                try
                {
                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    entry = jss.Deserialize<Entry>(json);
                }
                catch (Exception)
                {

                    throw new Exception("Unable to parse Json");
                }
                

            }
            else if (e.Topic == sTopics[1])
            {
                Console.WriteLine("Received from Alarm" + Encoding.UTF8.GetString(e.Message));
            }
        }

    }
}
