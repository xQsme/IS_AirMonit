using IAirEntries.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAirEntries
{
    class DBManager
    {
        private const string MAX_ROWS_TO_RETURN = "TOP ";
        #region QUERIES
        #region SELECT COLUMNS
        private const string COLUMNS_ENTRIES_FULL = "Entries.*";
        private const string COLUMNS_ENTRIES_SMALL = "Entries.name, Entries.value";
        private const string COLUMNS_ENTRIES_DATE = "Entries.name, Entries.value, Entries.date";
        #endregion

        private const string SELECT_ALL_CITIES = "SELECT id, name, longitude, latitude FROM CITIES";
        private const string SELECT_CITY_BY_NAME = "SELECT id FROM Cities WHERE LOWER(name) = @cityName";
        private const string SELECT_ALL_PARTICLES = "SELECT DISTINCT name FROM CityAverage;";
        private const string SELECT_PARTICLE_IN_DAY = "SELECT value FROM Entries WHERE name = @particle AND convert(VARCHAR, @date, 105) = convert(varchar, date, 105)";
        private const string SELECT_PARTICLE_BETWEEN_DAYS = "SELECT name, value, date FROM Entries WHERE name = @particle AND convert(varchar, date, 105) >= convert(VARCHAR, @start, 105) AND convert(varchar, date, 105) <= convert(VARCHAR, @end, 105)";
        
        private const string SELECT_ALL_ENTRIES = "SELECT @TOP name, value, date FROM Entries";
        private const string SELECT_CITY_ENTRIES = "SELECT @TOP name, value, date, cityId as \"City\" FROM Entries WHERE cityId = (SELECT id FROM CITIES WHERE name = @cityName)";

        private const string INSERT_ENTRY = "INSERT INTO ENTRIES (name, value, date, cityId) VALUES (@particleName, @value, @date, (" + SELECT_CITY_BY_NAME + "))";
        private const string INSERT_ALARM = "INSERT INTO ALARMS (particle, condition, conditionValue1, conditionValue2, EntryValue, Message, Date, CityId) VALUES (@particleName, @condition, @conditionValue1, @conditionValue2, @entryValue, @message,  @date, (" + SELECT_CITY_BY_NAME + "))";

        #endregion

        //A connection string esta nos resources porq assim o user nao pode mudar a sua ligação
        private static string CONNSTR = "";//Settings.DBConnection;

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
                        Name = (string)reader["name"],
                        Latitude = (float)reader["latitude"],
                        Longitude = (float)reader["longitude"]
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

        //Como saber se é admin ou se é user normal para poder usar o TOP?
        public static List<ParticleEntry> GetAllEntries()
        {
            SqlConnection conn = new SqlConnection(CONNSTR);
            List<ParticleEntry> lista = new List<ParticleEntry>();
            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = SELECT_ALL_ENTRIES;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@TOP", "TOP 10");
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ParticleEntry e = new ParticleEntry();
                    e.Name = (string)reader["name"];
                    e.Value = (decimal)reader["value"];
                    e.Date = (DateTime)reader["date"];

                    lista.Add(e);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return lista;
        }
        
        public static List<ParticleEntry> GetCityEntries(string name)
        {

            SqlConnection conn = new SqlConnection(CONNSTR);
            List<ParticleEntry> lista = new List<ParticleEntry>();
            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = SELECT_CITY_ENTRIES;
                cmd.Parameters.AddWithValue("@TOP", MAX_ROWS_TO_RETURN + 10);
                cmd.Parameters.AddWithValue("@cityName", name);
                cmd.Connection = conn;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ParticleEntry e = new ParticleEntry();
                    e.Name = (string)reader["name"];
                    e.Value = (decimal)reader["value"];
                    e.Date = (DateTime)reader["date"];

                    lista.Add(e);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return lista;

        }

        public static List<ParticleEntry> GetParticleInDay(string particle, DateTime dateTime)
        {

            SqlConnection conn = new SqlConnection(CONNSTR);
            List<ParticleEntry> lista = new List<ParticleEntry>();
            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = SELECT_PARTICLE_IN_DAY;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@particle", particle);
                cmd.Parameters.AddWithValue("@date", dateTime);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ParticleEntry e = new ParticleEntry();
                    e.Value = (decimal)reader["value"];

                    lista.Add(e);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return lista;

        }

        internal static List<ParticleEntry> GetParticleBetweenDays(string particle, DateTime start, DateTime end)
        {

            SqlConnection conn = new SqlConnection(CONNSTR);
            List<ParticleEntry> lista = new List<ParticleEntry>();
            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = SELECT_PARTICLE_BETWEEN_DAYS;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@particle", particle);
                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@end", end);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ParticleEntry e = new ParticleEntry();
                    e.Name = (string)reader["name"];
                    e.Value = (decimal)reader["value"];
                    e.Date = (DateTime)reader["date"];

                    lista.Add(e);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return lista;

        }
    }
}
