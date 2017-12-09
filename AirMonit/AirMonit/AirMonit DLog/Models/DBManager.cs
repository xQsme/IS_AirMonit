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

        private const string SELECT_ALL_CITIES = "SELECT id, name, longitude, latitude FROM CITIES";
        private const string SELECT_CITY_BY_NAME = "SELECT id FROM Cities WHERE LOWER(name) = @cityName";
        private const string SELECT_ID_CITY_FROM_NAME = "SELECT id FROM CITIES WHERE name=@city";
        private const string SELECT_ALL_PARTICLES = "SELECT DISTINCT name FROM ENTRIES;";

        private const string INSERT_NEWDISTRICT = "INSERT INTO CITIES VALUES (@name, @latitude, @longitude)";
        private const string INSERT_ENTRY = "INSERT INTO ENTRIES (name, value, date, cityId) VALUES (@particleName, @value, @date, (" + SELECT_CITY_BY_NAME + "))";
        private const string INSERT_ALARM = "INSERT INTO ALARMS (particle, condition, conditionValue1, conditionValue2, EntryValue, Message, Date, CityId) VALUES (@particleName, @condition, @conditionValue1, @conditionValue2, @entryValue, @message,  @date, ("+SELECT_CITY_BY_NAME+"))";
        private const string INSERT_CITY_AVERAGE = "INSERT INTO CityAverage (particle, cityId) VALUES (@particleName, @cityFk);";

        #endregion

        //A connection string esta nos resources porq assim o user nao pode mudar a sua ligação
        private static string CONNSTR = Resources.connStr;

        public static int WriteToTableAlarm(AlarmEntry alarmEntry)
        {
            SqlConnection conn = new SqlConnection(CONNSTR);

             //tenho a cidade string e quero pedir o id dela mas sem ter que iniciar várias coneccoes..

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
                cmd.Parameters.AddWithValue("@date", alarmEntry.Date);
                cmd.Parameters.AddWithValue("@cityName", alarmEntry.City.ToLower());

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
        
        public static int WriteToTableEntries(ParticleEntry particleEntry)
        {
            SqlConnection conn = new SqlConnection(CONNSTR);

            try
            {
                //binding dos valores
                string particle = particleEntry.Name;
                decimal value = particleEntry.Value;
                DateTime date = particleEntry.Date;
                string city = particleEntry.City.ToLower();

                //Comando sql
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = INSERT_ENTRY;

                cmd.Parameters.AddWithValue("@particleName", particle);
                cmd.Parameters.AddWithValue("@value", value);
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@cityName", city);

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

        /// <summary>
        /// Carrega as cidades que estao na BD para poder saber as PK delas e usar no inser das entries para relacionar a cidade com o record
        /// </summary>
        public static List<City> GetAllCities()
        {

            SqlConnection conn = new SqlConnection(CONNSTR);
            List<City> citiesListInDB = new List<City>();
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = SELECT_ALL_CITIES;

                cmd.Connection = conn;
                SqlDataReader reader = cmd.ExecuteReader();
                
                while (reader.Read())
                {
                    City city = new City()
                    {
                        ID = (int)reader["Id"],
                        Name = (string)reader["name"]
                    };
                    citiesListInDB.Add(city);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                Console.WriteLine("FATAL ERROR: Loading Cities When: " + ex.Message);
                return null;
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return citiesListInDB;

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
            }
            catch (Exception ex)
            {
                conn.Close();
                Console.WriteLine("FATAL ERROR: Loading Particles When: " + ex.Message);
                return null;
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


        /*@Deprecated
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

        public static int GetCityId(string city)
        {
            SqlConnection conn = new SqlConnection(CONNSTR);
            int cityId = -1;

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = SELECT_ID_CITY_FROM_NAME;

                cmd.Parameters.AddWithValue("city", city);

                cmd.Connection = conn;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cityId = (int)reader["id"];
                    break; //So quero 1 valor
                }
                reader.Close();
                return cityId;
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
            return cityId;
        }
        */
    }
}
