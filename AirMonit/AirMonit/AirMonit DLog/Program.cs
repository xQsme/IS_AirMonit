using System;
using System.Data.SqlClient;
using System.Text;
using System.Web.Script.Serialization;
using AirMonit_DLog.Properties;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using IAirEntries;
using System.Collections.Generic;
using System.Collections.Specialized;
using AirMonit_DLog.Models;

namespace AirMonit_DLog
{
    class Program
    {
        
        private static MqttClient mClientDU;
        private static MqttClient mClientAlarm;
        private static StringCollection topicsCollection = Settings.Default.Topics;
        private static string[] sTopics = new string[] { "", ""};
        private static string ipDU = Settings.Default.BrokerIPDU;
        private static string ipAlarm = Settings.Default.BrokerIPAlarm;

        private static JavaScriptSerializer jssParticleEntry = new JavaScriptSerializer();
        private static JavaScriptSerializer jssAlarmEntry = new JavaScriptSerializer();

        static void Main(string[] args)
        {
            
            topicsCollection.CopyTo(sTopics, 0);

            //Preparar a tabela para poder receber os dados
            manageIPMqttConfiguration(sTopics, ipDU, ipAlarm);
            
        }

        private static void manageIPMqttConfiguration(string[] sTopics, string ipDU, string ipAlarm)
        {
            byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE };

            //Caso a msm maquina tenha tanto o DU como o Alarm
            if (ipDU.Equals(ipAlarm))
            {
                mClientDU = new MqttClient(ipDU);
                mClientAlarm = mClientDU;

                mClientDU.Connect(Guid.NewGuid().ToString());

                mClientDU.MqttMsgPublishReceived += MClient_MqttMsgPublishReceived;

                mClientDU.Subscribe(sTopics, qosLevels);
                
            }
            else
            {
                //Caso o DU e o Alarm estejam em máquinas diferentes
                mClientDU = new MqttClient(ipDU);
                mClientAlarm = new MqttClient(ipAlarm);

                mClientDU.Connect(Guid.NewGuid().ToString());
                mClientAlarm.Connect(Guid.NewGuid().ToString());

                mClientDU.MqttMsgPublishReceived += MClient_MqttMsgPublishReceived;
                mClientAlarm.MqttMsgPublishReceived += MClient_MqttMsgPublishReceived;

                string[] duTopic = new string[] {sTopics[0]};
                string[] alarmTopic = new string[] { sTopics[1] };
                mClientDU.Subscribe(duTopic, qosLevels);
                mClientAlarm.Subscribe(alarmTopic, qosLevels);

            }
            
        }

        private static void MClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            //NOTA!: agora vem so 1 particula de cada vez!!!
            String json = Encoding.UTF8.GetString(e.Message);
            if (e.Topic == sTopics[0])
            {
                SaveParticleEntry(json);
            }
            else if (e.Topic == sTopics[1])
            {
                SaveAlarmEntry(json);
            }
        }

        private static void SaveAlarmEntry(string json)
        {
            AlarmEntry alarmEntry;
            //Convert the object...
            try
            {
                alarmEntry = jssAlarmEntry.Deserialize<AlarmEntry>(json);

                if (DBManager.WriteToTableAlarm(alarmEntry) > 0)
                {
                    Console.WriteLine("Received from Alarm: " + json);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("[PARSING]: " + ex.Message);
            }
            
        }

        private static void SaveParticleEntry(string json)
        {
            ParticleEntry particleEntry;
            try
            {

                Console.WriteLine(json);

                particleEntry = jssParticleEntry.Deserialize<ParticleEntry>(json);

                try
                {
                    if (DBManager.WriteToTableEntries(particleEntry) > 0)
                    {
                        Console.WriteLine("Entry added to DB: " + Environment.NewLine);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[LOAD DATA]" + ex.Message);
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Parsing JSON went wrong: " + ex.Message);
            }
        }

    }
}
