using IAirEntries;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.UI.WebControls;

namespace AirMonit_Service.Controllers
{
    public class EntriesController : ApiController
    {
        #region SELECT COLUMNS
        private const string COLUMNS_ENTRIES_FULL = "Entriess.*";
        private const string COLUMNS_ENTRIES_SMALL = "Entriess.name, Entriess.value";
        private const string COLUMNS_ENTRIES_DATE = "Entriess.name, Entriess.value, Entriess.date";
        #endregion

        #region QUERIES
        //Para não ter que efetuar sempre esta query sempre que adiciona um novo dado
        //Caso a BD mude os id's adicione algum distrito ou remova não deteta a alteracao! so se voltar a correr esta query
        private const string GETDISTRICTSID = "SELECT id, name FROM CITIES;";
        private const string GETENTRIES_CITY = "SELECT {0}, Cities.name as \"City\" FROM Entriess, Cities WHERE Entriess.cityId = Cities.Id;";
        private const string GETENTRY_CITY = "SELECT {0}, Cities.name as \"City\" FROM Entriess, Cities WHERE Entriess.cityId = Cities.Id && Entriess.Id = @entryId;";
        #endregion

        //Contains the city names and its ID in BD to help insert the particle records

        private static string CONNSTR = System.Configuration.ConfigurationManager.ConnectionStrings["AirMonit_Service.Properties.Settings.connStr"].ConnectionString;
        // GET api/<controller>
        public IEnumerable<Entry> GetEntries()
        {
            List<Entry> lista = new List<Entry>();
            SqlConnection conn = new SqlConnection(CONNSTR);
            try
            {
                conn.Open();
                
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = String.Format(GETENTRIES_CITY, COLUMNS_ENTRIES_DATE);
                cmd.Connection = conn;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Entry e = new Entry();
                    e.name = (string)reader["name"];
                    e.val = (decimal)reader["value"];
                    e.date = (DateTime)reader["date"];
                    e.city = (string)reader["City"];

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

        // GET api/<controller>/5
        public IHttpActionResult Get(int id)
        {
            Entry e = null;
            SqlConnection conn = new SqlConnection(CONNSTR);

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = String.Format(GETENTRY_CITY, COLUMNS_ENTRIES_DATE);
                cmd.Connection = conn;
                
                cmd.Parameters.AddWithValue("@entryId", id);
                
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    e = new Entry();
                    e.name = (string)reader["name"];
                    e.val = (int)reader["value"];
                    e.date = (DateTime)reader["date"];
                    e.city = (string)reader["City"];
                }
                reader.Close();
                
            }
            catch (Exception ex)
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
                Console.WriteLine(ex);
                return NotFound();
            }
            return Ok(e);
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}