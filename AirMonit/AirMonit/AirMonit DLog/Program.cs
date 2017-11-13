using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using AirMonit_DLog.Models;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace AirMonit_DLog
{
    class Program
    {
        private static string CONNSTR = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\Documentos\\Git\\IS_AirMonit\\AirMonit\\AirMonit\\AirMonit DLog\\App_Data\\DBAirMonit.mdf\";Integrated Security=True";
        private static MqttClient mClient;
        private static string[] sTopics = new[] { "dataUploader", "alarm" };
        private static string ip = "127.0.0.1";

        static void Main(string[] args)
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
                String json = Encoding.UTF8.GetString(e.Message);
                Console.WriteLine("Received from DU: " + json);
                if (WriteToDB(json))
                {
                    Console.WriteLine("Entry added to DB" + Environment.NewLine);
                }
            }
            else if (e.Topic == sTopics[1])
            {
                Console.WriteLine("Received from Alarm" + Encoding.UTF8.GetString(e.Message));
            }
        }

        private static Boolean WriteToDB(String json)
        {
            SqlConnection conn = new SqlConnection(CONNSTR);
            Entry entry;

            try
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                entry = jss.Deserialize<Entry>(json);
            }
            catch (Exception e)
            {
                return false;
            }

            try
            {
                //binding dos valores
                int no2 = entry.no2;
                int co = entry.co;
                int o3 = entry.o3;
                DateTime date = entry.date;
                Entry.City city = entry.city;

                //Comando sql
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "INSERT INTO Entries (NO2, CO, O3, DateTime, City) VALUES(@no2, @co, @o3, @date, @city);";

                cmd.Parameters.AddWithValue("@no2", no2);
                cmd.Parameters.AddWithValue("@co", co);
                cmd.Parameters.AddWithValue("@o3", o3);
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@city", city.ToString());

                cmd.Connection = conn;
                int nRows = cmd.ExecuteNonQuery();

                conn.Close();
                if (nRows > 0)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
                return false;
            }
            return false;
        }
    }
}
