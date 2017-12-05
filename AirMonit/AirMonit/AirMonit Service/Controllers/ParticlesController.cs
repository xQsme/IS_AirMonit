
using AirMonit_Service.Models;
using AirMonit_Service.Properties;
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
    [RoutePrefix("api/particles")]
    public class ParticlesController : ApiController
    {

        //Contains the city names and its ID in BD to help insert the particle records

        private static string CONNSTR = Resources.DBConnection;

        /// <summary>
        /// Get All Entries From All Cities
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ParticleEntry> GetAllParticles()
        {
            return DBManager.GetAllEntries();
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

        /// <summary>
        /// Gets the values of that particle in that day
        /// </summary>
        /// <param name="particle">CO</param>
        /// <param name="day">30-11-2017</param>
        /// <returns>IEnumerable<ParticleEntry></returns>
        [Route("{particle}/day/{day}")]
        //api/particles/CO/day/30-11-2017
        public IEnumerable<ParticleEntry> GetParticleByDate(string particle, string day)
        {
            try
            {
                DateTime date = DateTime.Parse(day);
                return DBManager.GetParticleInDay(particle, date);
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// Gets the values of that particle between 2 dates
        /// </summary>
        /// <param name="particle">CO</param>
        /// <param name="day">30-11-2017</param>
        /// <returns>IEnumerable<ParticleEntry></returns>
        [Route("{particle}/days/{day}")]
        //api/particles/CO/days/27-11-2017_30-11-2017
        public IEnumerable<ParticleEntry> GetParticleBetweenDates(string particle, string day)
        {
            string[] dates = day.Split('_');
            if(dates.Length <= 1)
            {
                return null;
            }
            try
            {
                DateTime start = DateTime.Parse(dates[0]);
                DateTime end = DateTime.Parse(dates[1]);
                return DBManager.GetParticleBetweenDays(particle, start, end);
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        /// <summary>
        /// Gets the values of that particle in that city in that day
        /// </summary>
        /// <param name="particle">CO</param>
        /// <param name="city">Leiria</param>
        /// <param name="day">30-11-2017</param>
        /// <returns>IEnumerable<ParticleEntry></returns>
        [Route("{particle}/city/{city}/day/{day}")]
        public IEnumerable<ParticleEntry> GetParticleInDay(string particle, string day, string city)
        {
            try
            {
                DateTime date = DateTime.Parse(day);
                return DBManager.GetCityEntriesInDay(particle, city, date);
            }
            catch (Exception)
            {

                return null;
            }

        }

        /// <summary>
        /// Gets the MAX MIN AVERAGE of that particle in that day
        /// </summary>
        /// <param name="particle">CO</param>
        /// <param name="day">30-11-2017</param>
        /// <returns>IEnumerable<SummarizeEntries></returns>
        [Route("{particle}/summarize/day/{day}")]
        public IEnumerable<SummarizeEntries> GetParticleSummarizeInDayInCity(string particle, string day)
        {
            try
            {
                DateTime date = DateTime.Parse(day);
                return DBManager.GetParticleSummarizeInDay(particle, date);
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// Gets the MAX MIN AVERAGE of that particle in that day in that city
        /// </summary>
        /// <param name="particle">CO</param>
        /// /// <param name="city">Leiria</param>
        /// <param name="day">30-11-2017</param>
        /// <returns>IEnumerable<SummarizeEntries></returns>
        [Route("{particle}/summarize/city/{city}/day/{day}")]
        public IEnumerable<SummarizeEntries> GetParticleSummarizeInDayInCity(string particle, string day, string city)
        {
            try
            {
                DateTime date = DateTime.Parse(day);
                return DBManager.GetSummarizeCityEntriesInDay(particle, city, date);
            }
            catch (Exception)
            {

                return null;
            }

        }

    }
}