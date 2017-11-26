using System;
using System.Data.SqlClient;
using System.Text;
using System.Web.Script.Serialization;
using AirMonit_DLog.Properties;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using IAirEntries;
using System.Collections.Generic;
using AirMonit_DLog.Models;

namespace AirMonit_DLog
{
    class Program
    {
        
        private static MqttClient mClient;
        private static string[] sTopics = new[] { "dataUploader", "alarm" };
        private static string ip = "127.0.0.1";
        
        private static List<City> citiesListInDB = new List<City>();
        private static List<string> particlesList = new List<string>();
        private static JavaScriptSerializer jssParticleEntry = new JavaScriptSerializer();
        private static JavaScriptSerializer jssAlarmEntry = new JavaScriptSerializer();

        static void Main(string[] args)
        {
            citiesListInDB = DBManager.GetCities();
            particlesList = DBManager.GetParticles();
            
            //Preparar a tabela para poder receber os dados
            UpdateCityAverage();

            mClient = new MqttClient(ip);
            mClient.Connect(Guid.NewGuid().ToString());
            mClient.MqttMsgPublishReceived += MClient_MqttMsgPublishReceived;
            byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE };
            mClient.Subscribe(sTopics, qosLevels);
        }

        private static void MClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            //NOTA!: agora vem so 1 particula de cada vez!!!
            String json = Encoding.UTF8.GetString(e.Message);
            if (e.Topic == sTopics[0])
            {
                SaveParticleEntry(json);
            }
            else if (e.Topic == sTopics[1])
            {
                SaveAlarmEntry(json);
            }
        }

        private static void SaveAlarmEntry(string json)
        {
            AlarmEntry alarmEntry;
            //Convert the object...
            try
            {
                alarmEntry = jssAlarmEntry.Deserialize<AlarmEntry>(json);

                int cityFK = GetCityForeignKey(alarmEntry.City);

                CheckCityAverageStructure();

                //TODO: Criar a tabela pensar no relacionamente se é que vai haver e criar metodo para inserir na BD
                if (DBManager.WriteToTableAlarm(alarmEntry, cityFK) > 0)
                {

                }
                //Inserted in DB
                Console.WriteLine("Received from Alarm: " + json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[PARSING]: " + ex.Message);
            }
            
        }

        private static void SaveParticleEntry(string json)
        {
            ParticleEntry particleEntry;
            try
            {

                Console.WriteLine(json);

                particleEntry = jssParticleEntry.Deserialize<ParticleEntry>(json);

                try
                {
                    int cityFK = GetCityForeignKey(particleEntry.city);
                    if (DBManager.WriteToTableEntries(particleEntry, cityFK) > 0)
                    {
                        Console.WriteLine("Entry added to DB: " + Environment.NewLine);
                        //Acabou de inserir uma linha entao vamos popular a lista de particulas
                        if (particlesList.Count == 0)
                        {
                            particlesList = DBManager.GetParticles();
                            UpdateCityAverage();
                        }
                        //Garante que so é chamado quando tiver linhas para todas as particulas na CityAverage
                        DBManager.UpdateParticleAverage(particleEntry.name, particleEntry.val, cityFK);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[LOAD DATA]" + ex.Message);
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Parsing JSON went wrong: " + ex.Message);
            }
        }

        private static int GetCityForeignKey(string city)
        {
            //Sem ter que ir sempre á BD procurar a FK pelo name buscar da lista
            city = city.ToLower();
            City c = citiesListInDB.Find(ele => ele.Name.ToLower().Equals(city));

            if (c == null)
            {
                //NEW CITY FOUND! This class is not responsible for inserting new Cities due to not knowing more then the city name (missing long, lang)
                List<City> cities = DBManager.GetCities();
                Console.WriteLine("New city found if you wish to start recording the data please insert in table 'Cities'");
                //UpdateCityAverage();
                c = citiesListInDB.Find(ele => ele.Name.ToLower().Equals(city));
                if(c == null)
                {
                    throw new Exception("Tried twice getting Cities from table Cities but not a single city was found");
                }
            }

            //TODO: Avisar o User que a Tabela Cities esta vazia e que precisa de ser preenchida
            //Vai crashar se a BD nao tiver nenhuma cidade...
            return c.ID;
        }


        /// <summary>
        /// Vai buscar todas as cidades que nao tenham rows na CityAverage e vai pedir ao DBManager
        /// Para inserir todas as particulas que existam para essa cidade na tabela CityAverage
        /// </summary>
        private static void UpdateCityAverage()
        {
            foreach (City c in citiesListInDB)
            {
                //Verificar se a cidade está na tabela senao insere todas as particulas 
                //Verificar se as cidades teem todas uma linha para cada particula

                //grab the cities
                List<int> citiesInAverageId = DBManager.GetCitiesNotInAverage();
                foreach (int cityId in citiesInAverageId)
                {
                    foreach (string particle in particlesList)
                    {
                        //para cada particula na lista particlesList insere uma nova linha limpa
                        DBManager.insertCityInAverage(cityId, particle);
                    }
                }
            }
        }

        private static void CheckCityAverageStructure()
        {
            //Verificar se existe a cidade na tabela average
            List<int> citiesMissingInAverage = DBManager.GetCitiesNotInAverage();
            foreach (int missingCity in citiesMissingInAverage)
            {
                DBManager.insertCityInAverage(missingCity);
            }

        }

    }
}
