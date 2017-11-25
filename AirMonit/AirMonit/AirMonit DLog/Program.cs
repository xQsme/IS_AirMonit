using System;
using System.Data.SqlClient;
using System.Text;
using System.Web.Script.Serialization;
using AirMonit_DLog.Properties;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using IAirEntries;
using System.Collections.Generic;
using AirMonit_DLog.Models;

namespace AirMonit_DLog
{
    class Program
    {

        #region QUERIES

        private const string SELECT_ALLDISTRICTS_ID_NAME = "SELECT id, name FROM CITIES";
        private const string SELECT_ALL_PARTICLES = "SELECT name FROM ENTRIES GROUP BY name;";
        private const string INSERT_NEWDISTRICT = "INSERT INTO CITIES VALUES (@name, @latitude, @longitude)";
        private const string INSERT_ENTRY = "INSERT INTO ENTRIES (name, value, date, cityId) VALUES (@particleName, @value, @date, @cityForeignKey)";
        private const string INSERT_ALARM = "";
        private const string UPDATE_GLOBAL_AVERAGE = "UPDATE CITYAVERAGE SET SUM = (SUM + @value), count = (count+1), average = ROUND(((SUM + @value) / (count+1)),2) WHERE particle = @particleName AND cityId = @cityId;";
        
        #endregion

        private static string CONNSTR = Settings.Default.connStr;
        private static MqttClient mClient;
        private static string[] sTopics = new[] { "dataUploader", "alarm" };
        private static string ip = "127.0.0.1";
        
        private static List<City> citiesListInDB = new List<City>();
        private static List<string> particlesList = new List<string>();

        static void Main(string[] args)
        {
            LoadCities();
            LoadParticles();

            mClient = new MqttClient(ip);
            mClient.Connect(Guid.NewGuid().ToString());
            mClient.MqttMsgPublishReceived += MClient_MqttMsgPublishReceived;
            byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE };
            mClient.Subscribe(sTopics, qosLevels);
        }

        private static void MClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            //NOTA!: agora vem so 1 particula de cada vez!!!
            if (e.Topic == sTopics[0])
            {
                Entry newEntry;
                try
                {
                    
                    String json = Encoding.UTF8.GetString(e.Message);
                    Console.WriteLine(json);
                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    newEntry = jss.Deserialize<Entry>(json);



                    if (WriteToDBNewParticleRecord(newEntry))
                    {
                        Console.WriteLine("Entry added to DB: " + Environment.NewLine);
                    }

                    if (newEntry.message != "")
                    {
                        //Write to other table that has all records of alerts!
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Parsing JSON went wrong: " + ex.Message);
                }

            }
            else if (e.Topic == sTopics[1])
            {
                Console.WriteLine("Received from Alarm: " + Encoding.UTF8.GetString(e.Message));
            }
        }

        //So adiciona o novo record na tabela Entries as mensagens estao na tabela Alerts
        private static Boolean WriteToDBNewParticleRecord(Entry entry)
        {
            SqlConnection conn = new SqlConnection(CONNSTR);

            try
            {                
                //binding dos valores
                string particle = entry.name;
                decimal value = entry.val;
                DateTime date = entry.date;
                string city = entry.city.ToLower();

                //Sem ter que ir sempre á BD procurar a FK pelo name buscar da lista
                City c = citiesListInDB.Find(ele => ele.Name.ToLower().Equals(city));

                if (c == null)
                {
                    //NEW CITY FOUND! This class is not responsible for inserting new Cities due to not knowing more then the city name (missing long, lang)
                    LoadCities();
                    Console.WriteLine("New city found if you wish to start recording the data please insert in table 'Cities'");
                    UpdateCityAverage();
                    c = citiesListInDB.Find(ele => ele.Name.ToLower().Equals(city));
                }
                int cityFK = c.ID;
                
                //Comando sql
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = INSERT_ENTRY;

                //CONVERT(SMALLDATETIME, @, 108)

                cmd.Parameters.AddWithValue("@particleName", particle);
                cmd.Parameters.AddWithValue("@value", value);
                cmd.Parameters.AddWithValue("@date", date);//.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@cityForeignKey", cityFK);

                cmd.Connection = conn;
                int nRows = cmd.ExecuteNonQuery();

                conn.Close();
                if (nRows > 0)
                {
                    UpdateParticleAverage(particle, value, cityFK);
                    //Ja inseriu valores ja pode popular a lista e ja pode adicionar os sensores na average de cada cidade
                    if (particlesList.Count == 0)
                    {
                        LoadParticles();
                        UpdateCityAverage();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
                Console.WriteLine("SQL: "+ ex.Message);
                return false;
            }
            return false;
        }

        //private static void UpdateCityAverage()
        //{
        //    foreach (City c in citiesListInDB)
        //    {
        //        "MERGE CITYAVERAGE as target" +
        //        "USING (SELECT name, cityId FROM CITYAVERAGE) as source" +
        //        "ON source.name = target.name AND source.cityId = target.cityID" +
        //        "WHEN NOT MATCHED THEN" +
        //        "INSERT (source.name, source.cityId)" +
        //        "VALUES"
        //    }
        //}

        //Atualiza os valores da média das particulas e das cidades
        private static void UpdateParticleAverage(string particle, decimal value, int cityId)
        {

            SqlConnection conn = new SqlConnection(CONNSTR);

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = UPDATE_GLOBAL_AVERAGE;
                
                cmd.Parameters.AddWithValue("@particleName", particle.ToUpper());
                cmd.Parameters.AddWithValue("@value", value);
                cmd.Parameters.AddWithValue("@cityId", cityId);
                cmd.Connection = conn;

                int nRows = cmd.ExecuteNonQuery();

                conn.Close();
                if (nRows <= 0)
                {
                    Console.WriteLine("Unable to update table ParticlesAverage: ");
                }
                
            }
            catch (Exception ex)
            {
                conn.Close();
                Console.WriteLine("FATAL ERROR: Updating Average table: " + ex.Message);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// Carrega as cidades que estao na BD para poder saber as PK delas e usar no inser das entries para relacionar a cidade com o record
        /// </summary>
        private static void LoadCities()
        {

            SqlConnection conn = new SqlConnection(CONNSTR);
            citiesListInDB.Clear();
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = SELECT_ALLDISTRICTS_ID_NAME;

                cmd.Connection = conn;
                SqlDataReader reader = cmd.ExecuteReader();

                City city;
                while (reader.Read())
                {
                    city = new City
                    {
                        ID = (int)reader["Id"],
                        Name = ((string)reader["name"]).Trim()
                    };
                    citiesListInDB.Add(city);
                }
                reader.Close();
                if(citiesListInDB.Count <= 0)
                {
                    Console.WriteLine("FATAL ERROR: TABLE CITIES IS EMPTY"+ Environment.NewLine+ "Unable to continue press any key to continue...");
                    Console.ReadLine();
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                Console.WriteLine("FATAL ERROR: Loading Cities When: " + ex.Message);
                Console.ReadLine();
                Environment.Exit(1);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }

        }

        private static void LoadParticles()
        {
            SqlConnection conn = new SqlConnection(CONNSTR);
            citiesListInDB.Clear();
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = SELECT_ALL_PARTICLES;

                cmd.Connection = conn;
                SqlDataReader reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    particlesList.Add((string)reader["name"]);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                Console.WriteLine("FATAL ERROR: Loading Particles When: " + ex.Message);
                Console.ReadLine();
                Environment.Exit(1);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

    }
}
