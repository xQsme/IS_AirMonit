using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.UI.WebControls;
using AirMonit_Service.Models;

namespace AirMonit_Service.Controllers
{
    public class EntriesController : ApiController
    {
        private static string CONNSTR = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\Documentos\\Git\\IS_AirMonit\\AirMonit\\AirMonit\\AirMonit DLog\\App_Data\\DBAirMonit.mdf\";Integrated Security=True";
        // GET api/<controller>
        public IEnumerable<Entry> GetEntries()
        {
            List<Entry> lista = new List<Entry>();
            SqlConnection conn = new SqlConnection(CONNSTR);

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Select * from Entries;";
                cmd.Connection = conn;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Entry e = new Entry();
                    e.no2 = (int)reader["NO2"];
                    e.co = (int)reader["CO"];
                    e.o3 = (int)reader["O3"];
                    e.date = (DateTime)reader["DateTime"];
                    e.city = (Entry.City)Enum.Parse(typeof(Entry.City), value: (string)reader["City"]);

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
                cmd.CommandText = "Select * from Entries WHERE id = @idprod;";
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@idprod", id);


                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    e = new Entry
                    {
                        no2 = (int)reader["NO2"],
                        co = (int)reader["CO"],
                        o3 = (int)reader["O3"],
                        date = (DateTime)reader["DateTime"],
                        city = (Entry.City)Enum.Parse(typeof(Entry.City), value: (string)reader["City"])

                    };

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