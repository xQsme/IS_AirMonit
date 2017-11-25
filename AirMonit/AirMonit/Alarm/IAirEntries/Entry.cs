using System;
using System.Collections.Generic;

namespace IAirEntries
{
    
        public class ParticleEntry
        {
            public string name { get; set; }
            public int val { get; set; }
            public string message { get; set; }
        }
        public class Entry
        {
            public List<ParticleEntry> particles { get; set; }
            public DateTime date { get; set; }
            public City city { get; set; }

            public enum City
            {
                LEIRIA,
                COIMBRA,
                PORTO,
                LISBOA
            }

            public Entry(List<ParticleEntry> particles, DateTime date, City city)
            {
                this.particles = particles;
                this.date = date;
                this.city = city;
            }

            public ParticleEntry getParticleByName(String particle)
            {
                return particles.Find(ele => ele.Equals(particle));
            }

            public List<ParticleEntry> getAlarmedParticles()
            {
                return particles.FindAll(ele => ele.message == "");
            }

            public List<ParticleEntry> getUnAlarmedParticles()
            {
                return particles.FindAll(ele => ele.message != "");
            }
        }
}
