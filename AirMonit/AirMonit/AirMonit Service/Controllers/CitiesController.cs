
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
        // GET: api/Cities
        public IEnumerable<City> GetCities()
        {
            return DBManager.GetAllCities();
        }

        [Route("api/cities/{city}/sensors")]
        public IEnumerable<SensorEntry> GetCitySensors(string city)
        {
            return DBManager.GetAllSensorsCity(city);
        }

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
            return DBManager.GetLastIncidentsInCity(city);
        }

        [Route("api/cities/events")]
        public IEnumerable<string> GetCitiesEvents()
        {
            return DBManager.GetEvents();
        }

        [Route("api/cities/{city}/sensors")]
        public void PostCitySensor(string city, [FromBody]string json)
        {
            DBManager.InsertSensor(new SensorEntry());
        }

        [Route("api/cities/{city}/incidents")]
        public void PostCityIncident(string city, [FromBody]string json)
        {
            DBManager.InsertIncident(new IncidentEntry());
        }
    }
}
