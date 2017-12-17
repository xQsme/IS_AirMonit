
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
            return DBManager.GetAllCities();
        }

        /// <summary>
        /// Gets the city all sensors data from the start
        /// </summary>
        /// <param name="city">Leiria</param>
        /// <returns>IEnumerable<SensorEntry></returns>
        [Route("api/cities/{city}/sensors")]
        public IEnumerable<SensorEntry> GetCitySensors(string city)
        {
            return DBManager.GetAllSensorsCity(city);
        }

        /// <summary>
        /// Gets the last 
        /// </summary>
        /// <param name="city">Lisboa</param>
        /// <returns></returns>
        [Route("api/cities/{city}/sensors/last")]
        public IEnumerable<SensorEntry> GetCityLastSensors(string city)
        {
            return DBManager.GetLastSensorsInCity(city);
        }

        [Route("api/cities/incidents")]
        public IEnumerable<IncidentEntry> GetCityIncidents()
        {
            return DBManager.GetAllIncidents();
        }

        [Route("api/cities/{city}/incidents")]
        public IEnumerable<IncidentEntry> GetCityIncidents(string city)
        {
            return DBManager.GetIncidentsInCity(city);
        }

        [Route("api/cities/{city}/incidents/last")]
        public IEnumerable<IncidentEntry> GetTopLastIncidents(string city)
        {
            return DBManager.GetLastIncidentsInCity(city);
        }

        [Route("api/cities/events")]
        public IEnumerable<string> GetCitiesEvents()
        {
            return DBManager.GetEvents();
        }

        [Route("api/cities/{city}/sensors")]
        public IHttpActionResult PostCitySensor(string city, [FromBody]SensorEntry sensorEntry)
        {
            int rows = DBManager.InsertSensor(sensorEntry);
            if(rows > 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [Route("api/cities/{city}/incidents")]
        public IHttpActionResult PostCityIncident(string city, [FromBody]IncidentEntry incidentEntry)
        {
            int rows = DBManager.InsertIncident(incidentEntry);
            if(rows > 0)
            {
                return Ok();
            }
            else {
                return BadRequest(LogHelper.GetLog());
            }

        }

    }
}
