
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

        //OK!
        /// <summary>
        /// Gets the values of that particle in that day
        /// </summary>
        /// <param name="particle">CO</param>
        /// <param name="day">30-11-2017</param>
        /// <returns>IEnumerable<ParticleEntry></returns>
        [Route("api/particles/{particle}/day/{day}")]
        //api/particles/CO/day/30-11-2017
        public IEnumerable<ParticleEntry> GetParticleByDate(string particle, string day)
        {
            
                DateTime date = DateTime.Parse(day);
                return DBManager.GetParticleInDay(particle, date);


        }

        //OK //api/particles/NO2/days/27-11-2017_30-11-2017
        /// <summary>
        /// Gets the values of that particle between 2 dates
        /// </summary>
        /// <param name="particle">CO</param>
        /// <param name="day">30-11-2017</param>
        /// <returns>IEnumerable<ParticleEntry></returns>
        [Route("api/particles/{particle}/days/{days}")]
        //api/particles/CO/days/27-11-2017_30-11-2017
        public IEnumerable<ParticleEntry> GetParticleBetweenDates(string particle, string days)
        {
            
            string[] dates = days.Split('_');
            if(dates.Length <= 1)
            {
                return null;
            }
            
                DateTime start = DateTime.Parse(dates[0]);
                DateTime end = DateTime.Parse(dates[1]);
                return DBManager.GetParticleBetweenDays(particle, start, end);


        }

        //OK
        /// <summary>
        /// Gets the values of that particle in that city in that day
        /// </summary>
        /// <param name="particle">CO</param>
        /// <param name="city">Leiria</param>
        /// <param name="day">30-11-2017</param>
        /// <returns>IEnumerable<ParticleEntry></returns>
        [Route("api/particles/{particle}/city/{city}/day/{day}")]
        public IEnumerable<decimal> GetParticleInDay(string particle, string day, string city)
        {
                DateTime date = DateTime.Parse(day);
                return DBManager.GetCityEntriesInDay(particle, city, date);
            

        }

        #region Summarize Day By HOUR

        //OK
        /// <summary>
        /// Gets the MAX MIN AVERAGE of that particle in that day
        /// </summary>
        /// <param name="particle">CO</param>
        /// <param name="day">30-11-2017</param>
        /// <returns>IEnumerable<SummarizeEntries></returns>
        [Route("api/particles/{particle}/summarize/day/{day}/hour")]
        public IEnumerable<SummarizeEntries> GetParticleSummarizeInDay(string particle, string day)
        {
                DateTime date = DateTime.Parse(day);
                return DBManager.GetParticleSummarizeInDay(particle, date);
            

        }

        //OK!
        /// <summary>
        /// Gets the MAX MIN AVERAGE of that particle in that day in that city
        /// </summary>
        /// <param name="particle">CO</param>
        /// /// <param name="city">Leiria</param>
        /// <param name="day">30-11-2017</param>
        /// <returns>IEnumerable<SummarizeEntries></returns>
        [Route("api/particles/{particle}/summarize/city/{city}/day/{day}/hour")]
        public IEnumerable<SummarizeEntries> GetParticleSummarizeInDayInCity(string particle, string day, string city)
        {
            
                DateTime date = DateTime.Parse(day);
                return DBManager.GetSummarizeCityEntriesInDay(particle, city, date);
            

        }

        #endregion

        #region Summarize Days By DAY

        //OK
        [Route("api/particles/{particle}/summarize/days/{days}/")]
        public IEnumerable<SummarizeEntries> GetParticleSummarizeInDays(string particle, string days)
        {
            string[] dates = days.Split('_');
            if (dates.Length <= 1)
            {
                return null;
            }
            

                DateTime start = DateTime.Parse(dates[0]);
                DateTime end = DateTime.Parse(dates[1]);
                return DBManager.GetParticleSummarizeInDay(particle, start, end);
            
        }

        //OK
        [Route("api/particles/{particle}/summarize/city/{city}/days/{days}")]
        public IEnumerable<SummarizeEntries> GetParticleSummarizeInDaysInCity(string particle, string days, string city)
        {
            string[] dates = days.Split('_');
            if (dates.Length <= 1)
            {
                return null;
            }


            DateTime start = DateTime.Parse(dates[0]);
            DateTime end = DateTime.Parse(dates[1]);
            return DBManager.GetParticlesSummarizeInDaysInCity(particle, city, start, end);
        }

        #endregion

    }
    /*
            try
            {
                
            }
            catch (Exception)
            {
                return null;
            }
     */
}