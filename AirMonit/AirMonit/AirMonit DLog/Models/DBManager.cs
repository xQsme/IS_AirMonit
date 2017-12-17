using AirMonit_DLog.Properties;
using IAirEntries;
using IAirEntries.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonit_DLog.Models
{
    public class DBManager
    {
        private const string MAX_ROWS_TO_RETURN = "TOP ";
        public const string SQL_DATE_FORMAT = "yyyy-mm-dd";

        #region SUB-QUERIES

        private const string SELECT_CITY_BY_NAME = "(SELECT id FROM Cities WHERE LOWER(name) = LOWER(@cityName))";
        private const string SELECT_CITY_NAME_BY_ID = "(SELECT RTRIM(c.name) FROM Cities c WHERE c.id = cityId)";
        private const string SQL_ROW_DATE_105 = "convert(varchar, date, 105)";

        #endregion

        //A connection string esta nos resources porq assim o user nao pode mudar a sua ligação
        private static string CONNSTR = Resources.DBConnection;

        private const string INSERT_ALARM = "INSERT INTO ALARMS (particle, condition, conditionValue1, conditionValue2, EntryValue, Message, Date, CityId) VALUES (@particleName, @condition, @conditionValue1, @conditionValue2, @entryValue, @message,  @date, (" + SELECT_CITY_BY_NAME + "))";

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

        private const string INSERT_ENTRY = "INSERT INTO ENTRIES (name, value, date, cityId) VALUES (@particleName, @value, @date, " + SELECT_CITY_BY_NAME + ")";
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

        private const string SELECT_ALL_CITIES = "SELECT id, RTRIM(name) as name, longitude, latitude FROM CITIES";
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

        private static string SELECT_ALARMS_BETWEEN_DATES = "SELECT Particle, Condition, ConditionValue1, ConditionValue2, EntryValue, Date, Message, " + SELECT_CITY_NAME_BY_ID + " as city FROM ALARMS WHERE " + SQL_ROW_DATE_105 + " >= convert(VARCHAR, @start, 105) AND " + SQL_ROW_DATE_105 + " <= convert(VARCHAR, @end, 105)";
        public static List<AlarmEntry> GetAlarmsBetweenDates(DateTime start, DateTime end)
        {

            SqlConnection conn = new SqlConnection(CONNSTR);
            List<AlarmEntry> lista = new List<AlarmEntry>();
            //try
            //{
            conn.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = SELECT_ALARMS_BETWEEN_DATES;
            cmd.Connection = conn;
            cmd.Parameters.AddWithValue("@start", start);
            cmd.Parameters.AddWithValue("@end", end);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                decimal val1 = (decimal)reader["ConditionValue1"];
                decimal val2 = (decimal)reader["ConditionValue2"];

                AlarmEntry e = new AlarmEntry();
                e.City = (string)reader["city"];
                e.Condition = (string)reader["Condition"];
                e.ConditionValues = new decimal[] { val1, val2 };
                e.Date = (DateTime)reader["Date"];
                e.EntryValue = (decimal)reader["EntryValue"];
                e.Message = (string)reader["Message"]; ;
                e.Particle = (string)reader["Particle"];

                lista.Add(e);
            }
            reader.Close();
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //    throw;
            //}
            //finally
            //{
            //    if (conn.State == System.Data.ConnectionState.Open)
            //    {
            //        conn.Close();
            //    }
            //}
            return lista;

        }

        private const string SELECT_ALL_PARTICLES = "SELECT DISTINCT RTRIM(name) as name FROM CityAverage;";
        public static List<string> GetParticlesName()
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

        private const string SELECT_PARTICLE_VALUES = "SELECT name, value, date, " + SELECT_CITY_NAME_BY_ID + " as city FROM Entries;";
        public static List<ParticleEntry> GetParticle(string particle)
        {

            SqlConnection conn = new SqlConnection(CONNSTR);
            List<ParticleEntry> lista = new List<ParticleEntry>();
            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = SELECT_PARTICLE_VALUES;
                cmd.Connection = conn;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ParticleEntry e = new ParticleEntry();
                    e.City = (string)reader["city"];
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

        //Como saber se é admin ou se é user normal para poder usar o TOP?
        private const string SELECT_ALL_ENTRIES = "SELECT @TOP name, value, date, " + SELECT_CITY_NAME_BY_ID + "as city FROM Entries";
        public static List<ParticleEntry> GetAllParticles()
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
                    e.City = (string)reader["city"];

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


        private const string SELECT_CITY_ENTRIES = "SELECT @TOP name, value, date FROM Entries WHERE cityId = " + SELECT_CITY_BY_NAME;
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
                    e.City = name;

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


        private const string SELECT_PARTICLE_IN_DAY = "SELECT name, value, " + SELECT_CITY_NAME_BY_ID + " as city FROM Entries WHERE name = @particle AND convert(VARCHAR, @date, 105) = " + SQL_ROW_DATE_105;
        public static List<ParticleEntry> GetParticleInDay(string particle, DateTime date)
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
                cmd.Parameters.AddWithValue("@date", date);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ParticleEntry e = new ParticleEntry();
                    e.Name = (string)reader["name"];
                    e.Value = (decimal)reader["value"];
                    e.City = (string)reader["city"];

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


        private const string SELECT_PARTICLE_BETWEEN_DAYS = "SELECT value, date, " + SELECT_CITY_NAME_BY_ID + " as city FROM Entries WHERE name = @particle AND " + SQL_ROW_DATE_105 + " >= convert(VARCHAR, @start, 105) AND " + SQL_ROW_DATE_105 + " <= convert(VARCHAR, @end, 105)";
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
                    e.Value = (decimal)reader["value"];
                    e.Date = (DateTime)reader["date"];
                    e.City = (string)reader["city"];

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


        private const string SELECT_PARTICLE_IN_CITY_DAY = "SELECT value FROM Entries WHERE name = @particle AND convert(VARCHAR, @date, 105) = " + SQL_ROW_DATE_105 + " AND cityId = " + SELECT_CITY_BY_NAME;
        public static List<ParticleEntry> GetCityEntriesInDay(string particle, string city, DateTime date)
        {
            SqlConnection conn = new SqlConnection(CONNSTR);
            List<ParticleEntry> lista = new List<ParticleEntry>();

            conn.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = SELECT_PARTICLE_IN_CITY_DAY;
            cmd.Connection = conn;
            cmd.Parameters.AddWithValue("@particle", particle);
            cmd.Parameters.AddWithValue("@date", date);
            cmd.Parameters.AddWithValue("@cityName", city);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ParticleEntry e = new ParticleEntry();
                e.Value = (decimal)reader["value"];
                lista.Add(e);
            }
            reader.Close();

            return lista;
        }


        private const string SELECT_SUMMARIZE_PARTICLE_IN_DAY = "SELECT MAX(value) as MAX, MIN(value) as MIN, ROUND(AVG(value),2) as AVERAGE, DATEPART(HOUR, date) as hour, " + SELECT_CITY_NAME_BY_ID + " as city FROM Entries WHERE name = @particle AND convert(VARCHAR, @date, 105) = " + SQL_ROW_DATE_105 + " GROUP BY cityId, DATEPART(HOUR, date) ORDER BY DATEPART(HOUR, date)";
        public static List<SummarizeEntries> GetParticleSummarizeInDay(string particle, DateTime date)
        {
            SqlConnection conn = new SqlConnection(CONNSTR);
            List<SummarizeEntries> lista = new List<SummarizeEntries>();
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
                int hour = (int)reader["hour"];
                DateTime temp = new DateTime(date.Year, date.Month, date.Day, hour, 0, 0);
                e.Date = temp;
                e.Max = (decimal)reader["MAX"];
                e.Min = (decimal)reader["MIN"];
                e.Average = (decimal)reader["AVERAGE"];
                e.City = (string)reader["city"];

                lista.Add(e);
            }
            reader.Close();

            return lista;
        }


        private const string SELECT_SUMMARIZE_PARTICLE_IN_CITY_DAY_BY_HOURS = "SELECT MAX(value) as MAX, MIN(value) as MIN, ROUND(AVG(value),2) as AVERAGE, DATEPART(HOUR, date) as hour FROM Entries WHERE name = @particle AND convert(VARCHAR, @date, 105) = " + SQL_ROW_DATE_105 + " AND cityId = " + SELECT_CITY_BY_NAME + " GROUP BY cityId, DATEPART(HOUR, date) ORDER BY DATEPART(HOUR, date)";
        public static List<SummarizeEntries> GetSummarizeCityEntriesInDay(string particle, string city, DateTime date)
        {

            SqlConnection conn = new SqlConnection(CONNSTR);
            List<SummarizeEntries> lista = new List<SummarizeEntries>();

            conn.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = SELECT_SUMMARIZE_PARTICLE_IN_CITY_DAY_BY_HOURS;
            cmd.Connection = conn;
            cmd.Parameters.AddWithValue("@particle", particle);
            cmd.Parameters.AddWithValue("@date", date);
            cmd.Parameters.AddWithValue("@cityName", city);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int hour = (int)reader["hour"];
                SummarizeEntries e = new SummarizeEntries();
                e.Date = new DateTime(date.Year, date.Month, date.Day, hour, 0, 0);
                e.Max = (decimal)reader["MAX"];
                e.Min = (decimal)reader["MIN"];
                e.Average = (decimal)reader["AVERAGE"];

                lista.Add(e);
            }
            reader.Close();

            return lista;
        }


        private const string SELECT_SUMMARIZE_PARTICLE_IN_DAYS_BY_DAY = "SELECT MAX(value) as MAX, MIN(value) as MIN, ROUND(AVG(value),2) as AVERAGE, DATEPART(day, date) as day, " + SELECT_CITY_NAME_BY_ID + " as city FROM Entries WHERE name = @particle AND " + SQL_ROW_DATE_105 + " >= convert(VARCHAR, @start, 105) AND " + SQL_ROW_DATE_105 + " <= convert(VARCHAR, @end, 105) GROUP BY cityId, DATEPART(day, date) ORDER BY DATEPART(day, date)";
        public static IEnumerable<SummarizeEntries> GetParticleSummarizeInDay(string particle, DateTime start, DateTime end)
        {
            SqlConnection conn = new SqlConnection(CONNSTR);
            List<SummarizeEntries> lista = new List<SummarizeEntries>();

            conn.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = SELECT_SUMMARIZE_PARTICLE_IN_DAYS_BY_DAY;
            cmd.Connection = conn;
            cmd.Parameters.AddWithValue("@particle", particle);
            cmd.Parameters.AddWithValue("@start", start);
            cmd.Parameters.AddWithValue("@end", end);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int dia = (int)reader["day"];
                SummarizeEntries e = new SummarizeEntries();
                e.Date = new DateTime(start.Year, start.Month, dia);
                e.Max = (decimal)reader["MAX"];
                e.Min = (decimal)reader["MIN"];
                e.Average = (decimal)reader["AVERAGE"];
                e.City = (string)reader["city"];
                lista.Add(e);
            }
            reader.Close();

            return lista;

        }

        private const string SELECT_SUMMARIZE_PARTICLE_IN_CITY_DAYS_BY_DAY = "SELECT MAX(value) as MAX, MIN(value) as MIN, ROUND(AVG(value),2) as AVERAGE, DATEPART(day, date) as day FROM Entries WHERE name = @particle AND " + SQL_ROW_DATE_105 + " >= convert(VARCHAR, @start, 105) AND " + SQL_ROW_DATE_105 + " <= convert(VARCHAR, @end, 105) AND cityId = " + SELECT_CITY_BY_NAME + " GROUP BY cityId, DATEPART(day, date) ORDER BY DATEPART(day, date)";
        public static IEnumerable<SummarizeEntries> GetParticlesSummarizeInDaysInCity(string particle, string city, DateTime start, DateTime end)
        {
            SqlConnection conn = new SqlConnection(CONNSTR);
            List<SummarizeEntries> lista = new List<SummarizeEntries>();

            conn.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = SELECT_SUMMARIZE_PARTICLE_IN_CITY_DAYS_BY_DAY;
            cmd.Connection = conn;
            cmd.Parameters.AddWithValue("@particle", particle);
            cmd.Parameters.AddWithValue("@start", start);
            cmd.Parameters.AddWithValue("@end", end);
            cmd.Parameters.AddWithValue("@cityName", city);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int dia = (int)reader["day"];
                SummarizeEntries e = new SummarizeEntries();
                e.Date = new DateTime(start.Year, start.Month, dia);
                e.Max = (decimal)reader["MAX"];
                e.Min = (decimal)reader["MIN"];
                e.Average = (decimal)reader["AVERAGE"];
                lista.Add(e);
            }
            reader.Close();

            return lista;

        }


        #region TAES

        private const string SELECT_EVENTS = "SELECT name FROM EVENTS";
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


        private const string SELECT_TODAY_CITY_SENSORS = "SELECT TOP 1 name, value, date FROM SENSORS WHERE cityId = " + SELECT_CITY_BY_NAME + " AND convert(VARCHAR, GETDATE(), 105) = " + SQL_ROW_DATE_105 + "  ORDER BY date DESC";
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


        private const string SELECT_ALL_CITY_SENSORS = "SELECT name, value, date FROM SENSORS WHERE cityId = " + SELECT_CITY_BY_NAME + " ORDER BY date DESC";
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


        private const string SELECT_TODAY_CITY_INCIDENTS = "SELECT message, publisher, date, (SELECT name FROM Events e WHERE e.id = eventId) as evento, otherEvent FROM INCIDENTS WHERE cityId = " + SELECT_CITY_BY_NAME + " AND convert(VARCHAR, GETDATE(), 105) = " + SQL_ROW_DATE_105 + " ORDER BY date DESC";
        public static List<IncidentEntry> GetTodaysIncidentsInCity(string city)
        {

            SqlConnection conn = new SqlConnection(CONNSTR);
            List<IncidentEntry> lista = new List<IncidentEntry>();

            conn.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = SELECT_TODAY_CITY_INCIDENTS;
            cmd.Connection = conn;
            cmd.Parameters.AddWithValue("@cityName", city);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                IncidentEntry e = new IncidentEntry();
                e.Message = (string)reader["message"];
                e.Publisher = (string)reader["publisher"];
                e.Event = (((object)reader["evento"]) == DBNull.Value) ? "" : (string)reader["evento"];
                e.OtherEvent = (((object)reader["otherEvent"]) == DBNull.Value) ? "" : (string)reader["otherEvent"];
                e.Date = (DateTime)reader["date"];

                lista.Add(e);
            }
            reader.Close();

            return lista;

        }


        private const string SELECT_ALL_INCIDENTS = "SELECT RTRIM(message) as message, publisher, date, (SELECT name FROM Events e WHERE e.id = eventId) as eventos, otherEvent, " + SELECT_CITY_NAME_BY_ID + " as city FROM INCIDENTS ORDER BY date DESC";
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
                e.Event = (((object)reader["eventos"]) == DBNull.Value) ? "" : (string)reader["eventos"];
                e.OtherEvent = (((object)reader["otherEvent"]) == DBNull.Value) ? "" : (string)reader["otherEvent"];
                e.Date = (DateTime)reader["date"];
                e.City = (string)reader["city"];

                lista.Add(e);
            }
            reader.Close();

            return lista;

        }


        private const string INSERT_SENSOR = "INSERT INTO SENSORS (name, value, cityId) VALUES (@sensor, @value, " + SELECT_CITY_BY_NAME + " )";
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


        private const string INSERT_INCIDENT = "INSERT INTO INCIDENTS (message, publisher, date, eventId, cityId) VALUES (@message, @publisher, @date, (SELECT id FROM Events WHERE LOWER(name) = LOWER(@eventName)), " + SELECT_CITY_BY_NAME + ")";
        private const string INSERT_OTHER_INCIDENT = "INSERT INTO INCIDENTS (message, publisher, date, otherEvent, cityId) VALUES (@message, @publisher, @date, @eventName, " + SELECT_CITY_BY_NAME + ")";
        public static int InsertIncident(IncidentEntry entry)
        {
            SqlConnection conn = new SqlConnection(CONNSTR);
            string query = "";
            try
            {
                //binding dos valores
                string evento = entry.Event;
                string otherEvent = entry.OtherEvent;
                string message = entry.Message;
                DateTime date = entry.Date;
                string city = entry.City;
                string publisher = entry.Publisher;

                query = (evento == "" && otherEvent != "") ? INSERT_OTHER_INCIDENT : INSERT_INCIDENT;
                string eventName = (evento == "" && otherEvent != "") ? otherEvent : evento;
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
