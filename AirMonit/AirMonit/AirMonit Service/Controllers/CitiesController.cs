
using AirMonit_Service.Models;
using IAirEntries;
using IAirEntries.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AirMonit_Service.Controllers
{
    public class CitiesController : ApiController
    {
        /// <summary>
        /// Get a list of all cities availabe in the DB
        /// </summary>
        /// <returns>List of cities</returns>
        public IEnumerable<City> GetCities()
        {
            try
            {
                return DBManager.GetAllCities();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the city all sensors data from the start
        /// </summary>
        /// <param name="city">Leiria</param>
        /// <returns>IEnumerable<SensorEntry></returns>
        [Route("api/cities/{city}/sensors")]
        public IEnumerable<SensorEntry> GetCitySensors(string city)
        {
            try
            {
               return DBManager.GetAllSensorsCity(city);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the last 
        /// </summary>
        /// <param name="city">Lisboa</param>
        /// <returns></returns>
        [Route("api/cities/{city}/sensors/last")]
        public IEnumerable<SensorEntry> GetCityLastSensors(string city)
        {
            try
            {
                return DBManager.GetLastSensorsInCity(city);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [Route("api/cities/incidents")]
        public IEnumerable<IncidentEntry> GetCityIncidents()
        {
            try
            {
                return DBManager.GetAllIncidents();
            }
            catch (Exception)
            {
                return null;
            }
        }

        [Route("api/cities/{city}/incidents")]
        public IEnumerable<IncidentEntry> GetCityIncidents(string city)
        {
            try
            {
                return DBManager.GetIncidentsInCity(city);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [Route("api/cities/{city}/incidents/last")]
        public IEnumerable<IncidentEntry> GetTopLastIncidents(string city)
        {
            try
            {
                return DBManager.GetLastIncidentsInCity(city);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [Route("api/cities/events")]
        public IEnumerable<string> GetCitiesEvents()
        {
            try
            {
                return DBManager.GetEvents();
            }
            catch (Exception)
            {
                return null;
            }    
        }

        [Route("api/cities/{city}/sensors")]
        public IHttpActionResult PostCitySensor(string city, [FromBody]SensorEntry sensorEntry)
        {
            try
            {
                int rows = DBManager.InsertSensor(sensorEntry);
                if (rows > 0)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        [Route("api/cities/{city}/incidents")]
        public IHttpActionResult PostCityIncident(string city, [FromBody]IncidentEntry incidentEntry)
        {
            try
            {
                int rows = DBManager.InsertIncident(incidentEntry);
                if (rows > 0)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(LogHelper.GetLog());
                }
            }
            catch (Exception)
            {
                return null;
            }
            

        }

    }
}
