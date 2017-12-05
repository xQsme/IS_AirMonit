﻿using AirMonit_Service.Properties;
using IAirEntries;
using IAirEntries.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonit_Service.Models
{
    class DBManager
    {
        private const string MAX_ROWS_TO_RETURN = "TOP ";
        #region QUERIES
        private const string SELECT_CITY_BY_NAME = "SELECT id FROM Cities WHERE LOWER(name) = LOWER(@cityName)";

        private const string SELECT_ALL_CITIES = "SELECT id, name, longitude, latitude FROM CITIES";
        private const string SELECT_ALL_PARTICLES = "SELECT DISTINCT name FROM CityAverage;";
        private const string SELECT_PARTICLE_IN_DAY = "SELECT value FROM Entries WHERE name = @particle AND convert(VARCHAR, @date, 105) = convert(varchar, date, 105)";
        private const string SELECT_PARTICLE_IN_CITY_DAY = "SELECT value FROM Entries WHERE name = @particle AND convert(VARCHAR, @date, 105) = convert(varchar, date, 105) AND cityId = @city";
        private const string SELECT_PARTICLE_BETWEEN_DAYS = "SELECT name, value, date FROM Entries WHERE name = @particle AND convert(varchar, date, 105) >= convert(VARCHAR, @start, 105) AND convert(varchar, date, 105) <= convert(VARCHAR, @end, 105)";
        private const string SELECT_SUMMARIZE_PARTICLE_IN_CITY_DAY = "SELECT MAX(value) as MAX, MIN(value) as MIN, AVG(value) as AVERAGE, DATEPART(HOUR, date) as Hour FROM Entries WHERE name = @particle AND convert(VARCHAR, @date, 105) = convert(varchar, date, 105) AND cityId = @city GROUP BY DATEPART(HOUR, date)";
        private const string SELECT_SUMMARIZE_PARTICLE_IN_DAY = "SELECT MAX(value) as MAX, MIN(value) as MIN, AVG(value) as AVERAGE, DATEPART(HOUR, date) as Hour FROM Entries WHERE name = @particle AND convert(VARCHAR, @date, 105) = convert(varchar, date, 105) GROUP BY DATEPART(HOUR, date)";
        private const string SELECT_EVENTS = "SELECT name FROM EVENTS";
        private const string SELECT_TODAY_CITY_SENSORS = "SELECT TOP 1 name, value, date FROM SENSORS WHERE cityId = "+ SELECT_CITY_BY_NAME + " AND convert(VARCHAR, GETDATE(), 105) = convert(varchar, date, 105)  ORDER BY date DESC";
        private const string SELECT_TODAY_CITY_INCIDENTS = "SELECT TOP 5 message, publisher, date, event, otherEvent FROM INCIDENT WHERE cityId = "+ SELECT_CITY_BY_NAME + " AND convert(VARCHAR, GETDATE(), 105) = convert(varchar, date, 105)  ORDER BY date DESC";
        private const string SELECT_ALL_INCIDENTS = "SELECT message, publisher, date, (SELECT name FROM Events e WHERE e.id = eventId) as eventos, otherEvent FROM INCIDENTS ORDER BY date DESC";
        private const string SELECT_ALL_CITY_SENSORS = "SELECT name, value, date FROM SENSORS WHERE cityId = "+ SELECT_CITY_BY_NAME + " ORDER BY date DESC";

        private const string SELECT_ALL_ENTRIES = "SELECT @TOP name, value, date FROM Entries";
        private const string SELECT_CITY_ENTRIES = "SELECT @TOP name, value, date, cityId FROM Entries WHERE cityId = " + SELECT_CITY_BY_NAME;

        private const string INSERT_ENTRY = "INSERT INTO ENTRIES (name, value, date, cityId) VALUES (@particleName, @value, @date, (" + SELECT_CITY_BY_NAME + "))";
        private const string INSERT_ALARM = "INSERT INTO ALARMS (particle, condition, conditionValue1, conditionValue2, EntryValue, Message, Date, CityId) VALUES (@particleName, @condition, @conditionValue1, @conditionValue2, @entryValue, @message,  @date, (" + SELECT_CITY_BY_NAME + "))";
        private const string INSERT_INCIDENT = "INSERT INTO INCIDENTS (message, publisher, eventId, cityId) VALUES (@message, @publisher, (SELECT id FROM Events WHERE name = @eventName), "+ SELECT_CITY_BY_NAME+")";
        private const string INSERT_OTHER_INCIDENT = "INSERT INTO INCIDENTS (message, publisher, otherEvent, cityId) VALUES (@message, @publisher, @eventName, "+ SELECT_CITY_BY_NAME + ")";
        private const string INSERT_SENSOR = "INSERT INTO SENSORS (name, value, cityId) VALUES (@sensor, @value, ("+ SELECT_CITY_BY_NAME + ") )";
        #endregion

        //A connection string esta nos resources porq assim o user nao pode mudar a sua ligação
        private static string CONNSTR = Resources.DBConnection;

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
                        Latitude = (Double)reader["latitude"],
                        Longitude = (Double)reader["longitude"]
                    };
                    citiesListInDB.Add(city);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                Console.WriteLine("FATAL ERROR: Loading Cities When: " + ex.Message);
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
                    e.name = (string)reader["name"];
                    e.val = (decimal)reader["value"];
                    e.date = (DateTime)reader["date"];

                    lista.Add(e);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
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
                    e.name = (string)reader["name"];
                    e.val = (decimal)reader["value"];
                    e.date = (DateTime)reader["date"];

                    lista.Add(e);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
                    e.val = (decimal)reader["value"];

                    lista.Add(e);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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

        public static List<ParticleEntry> GetParticleBetweenDays(string particle, DateTime start, DateTime end)
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
                    e.name = (string)reader["name"];
                    e.val = (decimal)reader["value"];
                    e.date = (DateTime)reader["date"];

                    lista.Add(e);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

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

        public static List<ParticleEntry> GetCityEntriesInDay(string particle, string city, DateTime date)
        {
            SqlConnection conn = new SqlConnection(CONNSTR);
            List<ParticleEntry> lista = new List<ParticleEntry>();
            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = SELECT_PARTICLE_IN_CITY_DAY;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@particle", particle);
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@city", city);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ParticleEntry e = new ParticleEntry();
                    e.val = (decimal)reader["value"];

                    lista.Add(e);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

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

        public static List<SummarizeEntries> GetParticleSummarizeInDay(string particle, DateTime date)
        {
            SqlConnection conn = new SqlConnection(CONNSTR);
            List<SummarizeEntries> lista = new List<SummarizeEntries>();
            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = SELECT_SUMMARIZE_PARTICLE_IN_DAY;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@particle", particle);
                cmd.Parameters.AddWithValue("@date", date);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    SummarizeEntries e = new SummarizeEntries();
                    e.Hour = (DateTime)reader["Hour"];
                    e.Max = (decimal)reader["MAX"];
                    e.Min = (decimal)reader["MIN"];
                    e.Average = (decimal)reader["AVERAGE"];

                    lista.Add(e);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

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

        public static List<SummarizeEntries> GetSummarizeCityEntriesInDay(string particle, string city, DateTime date)
        {
            SqlConnection conn = new SqlConnection(CONNSTR);
            List<SummarizeEntries> lista = new List<SummarizeEntries>();
            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = SELECT_PARTICLE_IN_CITY_DAY;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@particle", particle);
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@city", city);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    SummarizeEntries e = new SummarizeEntries();
                    e.Hour = (DateTime)reader["Hour"];
                    e.Max = (decimal)reader["MAX"];
                    e.Min = (decimal)reader["MIN"];
                    e.Average = (decimal)reader["AVERAGE"];

                    lista.Add(e);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

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


        #region TAES
        
        public static List<string> GetEvents()
        {

            SqlConnection conn = new SqlConnection(CONNSTR);
            List<string> lista = new List<string>();
            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = SELECT_EVENTS;
                cmd.Connection = conn;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string evento = (string)reader["name"];

                    lista.Add(evento);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

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

        public static List<SensorEntry> GetLastSensorsInCity(string city)
        {

            SqlConnection conn = new SqlConnection(CONNSTR);
            List<SensorEntry> lista = new List<SensorEntry>();
            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = SELECT_TODAY_CITY_SENSORS;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@cityName", city);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    SensorEntry e = new SensorEntry();
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

        public static List<SensorEntry> GetAllSensorsCity(string city)
        {

            SqlConnection conn = new SqlConnection(CONNSTR);
            List<SensorEntry> lista = new List<SensorEntry>();
            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = SELECT_ALL_CITY_SENSORS;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@cityName", city);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    SensorEntry e = new SensorEntry();
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

        public static List<IncidentEntry> GetLastIncidentsInCity(string city)
        {

            SqlConnection conn = new SqlConnection(CONNSTR);
            List<IncidentEntry> lista = new List<IncidentEntry>();
            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = SELECT_TODAY_CITY_INCIDENTS;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@cityName", city);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    IncidentEntry e = new IncidentEntry();
                    //message, publisher, date, event, otherEvent
                    e.Message = (string)reader["message"];
                    e.Publisher = (string)reader["publisher"];
                    e.Event = (((object)reader["eventos"]) == DBNull.Value) ? "" : (string)reader["eventos"];
                    e.OtherEvent = (((object)reader["otherEvent"]) == DBNull.Value) ? "" : (string)reader["otherEvent"];
                    e.Date = (DateTime)reader["date"];

                    lista.Add(e);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

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

        public static List<IncidentEntry> GetAllIncidents()
        {

            SqlConnection conn = new SqlConnection(CONNSTR);
            List<IncidentEntry> lista = new List<IncidentEntry>();
            
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = SELECT_ALL_INCIDENTS;
                cmd.Connection = conn;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    IncidentEntry e = new IncidentEntry();
                    //message, publisher, date, eventos, otherEvent
                    e.Message = (string)reader["message"];
                    e.Publisher = (string)reader["publisher"];
                    e.Event = ( ((object)reader["eventos"]) == DBNull.Value ) ? "" : (string)reader["eventos"];
                    e.OtherEvent = ( ((object)reader["otherEvent"]) == DBNull.Value ) ? "": (string)reader["otherEvent"];
                    e.Date = (DateTime)reader["date"];

                    lista.Add(e);
                }
                reader.Close();
            
            return lista;

        }

        public static int InsertSensor(SensorEntry entry)
        {

            SqlConnection conn = new SqlConnection(CONNSTR);
            try
            {
                string sensor = entry.Name;
                decimal value = entry.Value;
                DateTime date = entry.Date;
                string city = entry.City.ToLower();

                //Comando sql
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = INSERT_SENSOR;
                cmd.Parameters.AddWithValue("@sensor", sensor);
                cmd.Parameters.AddWithValue("@value", value);
                cmd.Parameters.AddWithValue("@date", DateTime.Now);
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

        public static int InsertIncident(IncidentEntry entry)
        {

            SqlConnection conn = new SqlConnection(CONNSTR);
            try
            {
                //binding dos valores
                string evento = entry.Event;
                string otherEvent = entry.OtherEvent;
                string message = entry.Message;
                DateTime date = entry.Date;
                string city = entry.City;
                string publisher = entry.Publisher;

                string query = (evento == "" && otherEvent != "") ? INSERT_OTHER_INCIDENT : INSERT_INCIDENT;
                string eventName = (evento == "" && otherEvent != "") ? otherEvent : evento ;
                //Comando sql
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = query;

                //@message, @publisher, @eventName
                cmd.Parameters.AddWithValue("@message", message);
                cmd.Parameters.AddWithValue("@publisher", publisher);
                cmd.Parameters.AddWithValue("@date", DateTime.Now);
                cmd.Parameters.AddWithValue("@cityName", city);
                cmd.Parameters.AddWithValue("@eventName", eventName);

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
        #endregion

    }
}