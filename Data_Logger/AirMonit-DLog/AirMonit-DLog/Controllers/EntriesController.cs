using AirMonit_DLog.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static AirMonit_DLog.Models.Entry;

namespace AirMonit_DLog.Controllers
{
    public class EntriesController : ApiController
    {

        private string CONNSTR = System.Configuration.ConfigurationManager.ConnectionStrings["AirMonit_DLog.Properties.Settings.connStr"].ConnectionString;

        public IEnumerable<Entry> GetEntries()
        {
            Debug.WriteLine("teste");
            List<Entry> lista = new List<Entry>();

            SqlConnection conn = new SqlConnection(CONNSTR);

            string debug = "";

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
                    e.id = (int) reader["Id"];
                    debug = "id";
                    e.parameter = (Parameter) Enum.Parse(typeof(Parameter) , (string)reader["Parameter"]);
                    debug = "parameter";
                    e.value = (Decimal) reader["Value"];
                    debug = "value";
                    e.date = (DateTime) reader["DateTime"];
                    debug = "dateTime";
                    e.city = (City) Enum.Parse(typeof(City), value: (string)reader["City"]);

                    lista.Add(e);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("debug = " + debug);
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
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        //[ActionName("Complex")]
        public HttpResponseMessage Post([FromBody]Entry entry)
        {
            Debug.Print(entry.id.ToString());

            if (entry != null)
            {
                SqlConnection conn = new SqlConnection(CONNSTR);


                try
                {
                    //binding dos valores
                    Parameter parameter = entry.parameter;
                    decimal value = entry.value;
                    DateTime date = entry.date;
                    City city = entry.city;

                    //Comando sql
                    conn.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = "INSERT INTO Entries (Parameter, Value, DateTime, City) VALUES(@parameter, @value, @date, @city); ";

                    SqlParameter paramParam = new SqlParameter("@parameter", SqlDbType.NVarChar, 50);
                    SqlParameter valueParam = new SqlParameter("@value", SqlDbType.Decimal, 12);
                    SqlParameter dateParam = new SqlParameter("@date", SqlDbType.Date);
                    SqlParameter cityParam = new SqlParameter("@city", SqlDbType.NVarChar, 50);

                    paramParam.Value = parameter.ToString();
                    valueParam.Value = value;
                    dateParam.Value = date;
                    cityParam.Value = city.ToString();

                    cmd.Parameters.Add(paramParam);
                    cmd.Parameters.Add(valueParam);
                    cmd.Parameters.Add(dateParam);
                    cmd.Parameters.Add(cityParam);

                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();

                    conn.Close();

                    cmd.ExecuteNonQuery();

                    return Request.CreateResponse(HttpStatusCode.Created);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    //throw;
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }

            }
            return Request.CreateResponse(HttpStatusCode.BadRequest);
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