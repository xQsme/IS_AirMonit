using AirMonit_DLog.Properties;
using IAirEntries;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonit_DLog.Models
{
    class DBManager
    {
        #region QUERIES

        private const string SELECT_ALLDISTRICTS_ID_NAME = "SELECT id, name FROM CITIES";
        private const string SELECT_ALL_PARTICLES = "SELECT DISTINCT name FROM ENTRIES;";
        private const string SELECT_CITIES_NOT_IN_AVERAGE = "SELECT DISTINCT id, name FROM CITIES WHERE id NOT IN (SELECT cityId FROM CityAverage);";
        private const string INSERT_NEWDISTRICT = "INSERT INTO CITIES VALUES (@name, @latitude, @longitude)";
        private const string INSERT_ENTRY = "INSERT INTO ENTRIES (name, value, date, cityId) VALUES (@particleName, @value, @date, @cityForeignKey)";
        private const string INSERT_ALARM = "INSERT INTO ALARMS (particle, condition, conditionValue1, conditionValue2, EntryValue, Message, Date, CityId) VALUES (@particleName, @condition, @conditionValue1, @conditionValue2, @entryValue, @message,  @date, @cityForeignKey)";
        private const string INSERT_CITY_AVERAGE = "INSERT INTO CityAverage (particle, cityId) VALUES (@particleName, @cityFk);";
        private const string UPDATE_GLOBAL_AVERAGE = "UPDATE CITYAVERAGE SET SUM = (SUM + @value), count = (count+1), average = ROUND(((SUM + @value) / (count+1)),2) WHERE particle = @particleName AND cityId = @cityId;";

        #endregion

        private static string CONNSTR = Settings.Default.connStr;

        public static void insertCityInAverage(int cityId)
        {
            //INSERT_CITY_AVERAGE
            foreach (string particle in GetParticles())
            {
                insertCityInAverage(cityId, particle);
            }

        }

        public static int insertCityInAverage(int cityId, string particle)
        {
            //INSERT_CITY_AVERAGE
            SqlConnection conn = new SqlConnection(CONNSTR);

            try
            {
                //Comando sql
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = INSERT_CITY_AVERAGE;


                cmd.Parameters.AddWithValue("@particleName", particle);
                cmd.Parameters.AddWithValue("@cityFk", cityId);

                cmd.Connection = conn;
                int nRows = cmd.ExecuteNonQuery();

                conn.Close();
                return nRows;
            }
            catch (Exception ex)
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
                Console.WriteLine("SQL: " + ex.Message);
                return -1;
            }

        }

        public static int WriteToTableAlarm(AlarmEntry alarmEntry, int cityFK)
        {
            SqlConnection conn = new SqlConnection(CONNSTR);

            try
            {
                //Comando sql
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = INSERT_ALARM;

                //@particleName, @condition, @conditionValue1, @conditionValue2, @entryValue, @message,  @date, @cityForeignKey


                cmd.Parameters.AddWithValue("@particleName", alarmEntry.Particle);
                cmd.Parameters.AddWithValue("@condition", alarmEntry.Condition);
                cmd.Parameters.AddWithValue("@conditionValue1", alarmEntry.ConditionValues[0]);

                if (alarmEntry.ConditionValues.Length > 1)
                    cmd.Parameters.AddWithValue("@conditionValue2", alarmEntry.ConditionValues[1]);
                else
                    cmd.Parameters.AddWithValue("@conditionValue2", null);

                cmd.Parameters.AddWithValue("@entryValue", alarmEntry.EntryValue);
                cmd.Parameters.AddWithValue("@message", alarmEntry.Message);
                cmd.Parameters.AddWithValue("@date", alarmEntry.Date);//.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@cityForeignKey", cityFK);

                cmd.Connection = conn;
                int nRows = cmd.ExecuteNonQuery();

                conn.Close();
                return nRows;
            }
            catch (Exception ex)
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
                Console.WriteLine("SQL: " + ex.Message);
                return -1;
            }
        }

        //Garantir que quando chamar esta funcao exista a row City na tabela Cities com o id = cityFK
        public static int WriteToTableEntries(ParticleEntry particleEntry, int cityFK)
        {
            SqlConnection conn = new SqlConnection(CONNSTR);

            try
            {
                //binding dos valores
                string particle = particleEntry.name;
                decimal value = particleEntry.val;
                DateTime date = particleEntry.date;
                string city = particleEntry.city.ToLower();

                //Comando sql
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = INSERT_ENTRY;

                cmd.Parameters.AddWithValue("@particleName", particle);
                cmd.Parameters.AddWithValue("@value", value);
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@cityForeignKey", cityFK);

                cmd.Connection = conn;
                int nRows = cmd.ExecuteNonQuery();

                conn.Close();
                return nRows;
            }
            catch (Exception ex)
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
                Console.WriteLine("SQL: " + ex.Message);
                return -1;
            }
        }

        //Atualiza os valores da média das particulas e das cidades
        public static void UpdateParticleAverage(string particle, decimal value, int cityId)
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
                    Console.WriteLine("Unable to update table CityAverage: ");
                }

            }
            catch (Exception ex)
            {
                conn.Close();
                Console.WriteLine("FATAL ERROR: Updating CityAverage table: " + ex.Message);
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
        public static List<City> GetCities()
        {

            SqlConnection conn = new SqlConnection(CONNSTR);
            List<City> citiesListInDB = new List<City>();
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
                if (citiesListInDB.Count <= 0)
                {
                    Console.WriteLine("FATAL ERROR: TABLE CITIES IS EMPTY" + Environment.NewLine + "Unable to continue press any key to continue...");
                    Console.ReadLine();
                    Environment.Exit(1);
                }
                return citiesListInDB;
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
            return null;

        }

        public static List<int> GetCitiesNotInAverage()
        {
            SqlConnection conn = new SqlConnection(CONNSTR);
            List<int> citiesList = new List<int>();
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = SELECT_CITIES_NOT_IN_AVERAGE;

                cmd.Connection = conn;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    citiesList.Add((int)reader["Id"]);
                }
                reader.Close();
                return citiesList;
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
            return null;
        }

        public static List<string> GetParticles()
        {
            SqlConnection conn = new SqlConnection(CONNSTR);
            List<string> particlesList = new List<string>();
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
                return particlesList;
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
            return particlesList;
        }
    }
}
