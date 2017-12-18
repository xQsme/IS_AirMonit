
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
using IAirEntries.Models;

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
            try
            {
                return DBManager.GetAllParticles();
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        [Route("api/particles/{particle}")]
        public IEnumerable<ParticleEntry> GetParticle(string particle)
        {
            try
            {
                return DBManager.GetParticle(particle);
            }
            catch (Exception)
            {
                return null;
            }
            
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
            try
            {
                string[] dates = days.Split('_');
                if (dates.Length <= 1)
                {
                    return null;
                }

                DateTime start = DateTime.Parse(dates[0]);
                DateTime end = DateTime.Parse(dates[1]);
                return DBManager.GetParticleBetweenDays(particle, start, end);
            }
            catch (Exception)
            {
                return null;
            }
            


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
            try
            {
                DateTime date = DateTime.Parse(day);
                return DBManager.GetSummarizeCityEntriesInDay(particle, city, date).ToList();
            }
            catch (Exception)
            {
                return null;
            }
                
            

        }

        #endregion

        #region Summarize Days By DAY

        //OK
        [Route("api/particles/{particle}/summarize/days/{days}/")]
        public IEnumerable<SummarizeEntries> GetParticleSummarizeInDays(string particle, string days)
        {
            try
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
            catch (Exception)
            {
                return null;
            }
            
            
        }

        //OK
        [Route("api/particles/{particle}/summarize/city/{city}/days/{days}")]
        public IEnumerable<SummarizeEntries> GetParticleSummarizeInDaysInCity(string particle, string days, string city)
        {
            try
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
            catch (Exception)
            {
                return null;
            }
            
        }

        #endregion

        #region Alarms

        [Route("api/particles/alarms/days/{days}")]
        public IEnumerable<AlarmEntry> GetAlarmsBetweenDates(string days)
        {
            try
            {
                string[] dates = days.Split('_');
                if (dates.Length <= 1)
                {
                    return null;
                }


                DateTime start = DateTime.Parse(dates[0]);
                DateTime end = DateTime.Parse(dates[1]);

                return DBManager.GetAlarmsBetweenDates(start, end);
            }
            catch (Exception)
            {
                return null;
            }
            
        }


        [Route("api/particles/alarms/day/{day}")]
        public IEnumerable<AlarmEntry> GetAlarmsInDay(string day)
        {
            try
            {
                DateTime date = DateTime.Parse(day);
                return DBManager.GetAlarmsInDay(day);
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        [Route("api/particles/alarms/city/{city}/day/{day}")]
        public IEnumerable<AlarmEntry> GetAlarmsInCityInDay(string city, string day)
        {
            try
            {
                DateTime date = DateTime.Parse(day);
                return DBManager.GetAlarmsinCityInDay(city, day);
            }
            catch (Exception)
            {
                return null;
            }
            
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