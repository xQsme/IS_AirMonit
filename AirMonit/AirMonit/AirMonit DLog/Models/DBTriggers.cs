using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirMonit_DLog.Models
{
    /// <summary>
    /// None of this class methods is responsible for creating or closing DB connections! make sure you do that dirty work
    /// </summary>
    class DBTriggers
    {
        #region Queries
        private const string SELECT_CITY_BY_NAME = "SELECT id FROM Cities WHERE LOWER(name) = @cityName";
        private const string SELECT_CITIES_NOT_IN_AVERAGE = "SELECT DISTINCT id, name FROM CITIES WHERE id NOT IN (SELECT cityId FROM CityAverage);";
        private const string SELECT_PARTICLES_NOT_IN_AVERAGE = "SELECT DISTINCT name FROM ENTRIES WHERE name NOT IN (SELECT DISTINCT particle FROM CityAverage)";

        private const string UPDATE_GLOBAL_AVERAGE = "UPDATE CITYAVERAGE SET SUM = (SUM + @value), " +
                                                                             "count = (count+1), " +
                                                                             "average = ROUND(((SUM + @value) / (count+1)),2) " +
                                                            "WHERE particle = @particleName AND cityId = (" + SELECT_CITY_BY_NAME + ");";

        /// <summary>
        /// *SELECT* Devolve todas as particulas (Tabela Entries) que nao existam na tabela CityAverage Ou as Cidades que nao contenham (1 ou mais) particula(Da Entrie)
        /// Ou seja se nao existir aquela particula na CityAverage ou se uma cidade nao tiver essa particula devolve cria 1 row
        /// </summary>
        private const string SetUp_Average_Missing_Rows =   "INSERT INTO CityAverage (particle, cityId)" +
                                                            "SELECT DISTINCT name, cityId " +
                                                            "FROM Entries e"+
                                                            "WHERE name NOT IN"+
                                                                    "(SELECT particle FROM CityAverage)"+
                                                                "OR cityId NOT IN"+
                                                                    "(SELECT cityId FROM CityAverage WHERE name = particle);";

        #endregion

        public static void NewEntry()
        {
            //check if the new Entry has a new Particle

        }

        private static void NewParticleFound(SqlConnection conn)
        {
            //INSERT INTO CityAverage (particle, cityId)
            //SELECT name, cityId FROM ENTRIES GROUP BY City
            //Insert Into all cities in average a new row
            if (conn.State != ConnectionState.Open)
            {
                throw new Exception("[CONN Failed]: connection to SQL DB was lost before executing trigger [NewParticleFound]");
            }
        }

        public static void CalculateAverage(SqlConnection conn, string particle, decimal value, string cityName)
        {
            //Update row 
            if (conn.State != ConnectionState.Open)
            {
                throw new Exception("[CONN Failed]: connection to SQL DB was lost before executing trigger [CalculateAverage]");
            }

            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = UPDATE_GLOBAL_AVERAGE;

                cmd.Parameters.AddWithValue("@particleName", particle.ToUpper());
                cmd.Parameters.AddWithValue("@value", value);
                cmd.Parameters.AddWithValue("@cityName", cityName);
                cmd.Connection = conn;

                int nRows = cmd.ExecuteNonQuery();

                if (nRows <= 0)
                {
                    Console.WriteLine("Unable to update table CityAverage: ");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("FATAL ERROR: Updating CityAverage table: " + ex.Message);
            }

        }

        #region Helper Functions

        private static List<int> GetCitiesNotInAverage(SqlConnection conn)
        {
            List<int> citiesList = new List<int>();
            try
            {
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
                throw new Exception("FATAL ERROR: Loading Particles When: " + ex.Message);
            }
        }

        private static List<string> GetParticleNotInAverage(SqlConnection conn)
        {
            List<string> particlesMissingList = new List<string>();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = SELECT_PARTICLES_NOT_IN_AVERAGE;

                cmd.Connection = conn;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    particlesMissingList.Add((string)reader["particle"]);
                }
                reader.Close();
                return particlesMissingList;
            }
            catch (Exception ex)
            {
                throw new Exception("FATAL ERROR: Loading Particles When: " + ex.Message);
            }
        }

        #endregion
    }
}
