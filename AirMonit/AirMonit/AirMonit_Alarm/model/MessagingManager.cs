using AirMonit_DLog.Models;
using IAirEntries;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace AirMonit_Alarm
{
    public delegate void MyEventNewParticleReceived(object source, MyEventParticle e);

    public class MyEventParticle : EventArgs
    {
        private string Particle;
        public MyEventParticle(string particle)
        {
            Particle = particle;
        }

        public string GetParticle()
        {
            return Particle;
        }
    }

    class MessagingHandler
    {
        public event MyEventNewParticleReceived OnNewParticleReceived;
        private MqttClient mClient;
        private string[] sTopics;
        private JavaScriptSerializer jss;
        private string Ip;
        private Dictionary<string, List<RuleCondition>> rulesDictionary;
        private string jsonResponse;

        public MessagingHandler(string[] sTopics, string ip, Dictionary<string, List<RuleCondition>> rule)
        {
            this.Ip = ip;
            this.rulesDictionary = rule;
            this.sTopics = sTopics;
            jss = new JavaScriptSerializer();
            mClient = new MqttClient(Ip);
            mClient.Connect(Guid.NewGuid().ToString());
            mClient.MqttMsgPublishReceived += MClient_MqttMsgPublishReceived;
            byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE };
            mClient.Subscribe(sTopics, qosLevels);
        }

        private void MClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            if (e.Topic == sTopics[0])
            {
                ParticleEntry entry;
                String json = Encoding.UTF8.GetString(e.Message);

                try
                {
                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    entry = jss.Deserialize<ParticleEntry>(json);
                    validateParticleWithAlarm(entry);
                }
                catch (Exception ex)
                {

                    throw new Exception("[PARSING] Unable to parse Json: "+ ex.Message);
                }

            }
        }

        public void disconnectMqtt()
        {
            if(mClient.IsConnected)
                mClient.Disconnect();
        }

        //Nota: Ja previne caso se altera o Dictionary noutro sitio e aconteça apanhar a null aqui
        private void validateParticleWithAlarm(ParticleEntry entry)
        {
            if (rulesDictionary.ContainsKey(entry.Name))
            {
                List<RuleCondition> rulesConditions = rulesDictionary[entry.Name];
                if(rulesConditions == null )//|| rulesConditions.) //Verificar se a particula tb está a false...
                {
                    //Novas particulas detetadas começam com a lista a null
                    return;
                }
                foreach (RuleCondition rule in rulesConditions)
                {
                    if (!rule.ApplyRule)
                        continue;

                    bool alarmTrigged = false;
                    switch (rule.Condition)
                    {
                        case Condition.LESS:
                            alarmTrigged = LessThan(rule.Value1, entry.Value);
                            break;
                        case Condition.GREATER:
                            alarmTrigged = GreaterThan(rule.Value1, entry.Value);
                            break;
                        case Condition.EQUALS:
                            alarmTrigged = Equals(rule.Value1, entry.Value);
                            break;
                        case Condition.BETWEEN:
                            alarmTrigged = Between(rule.Value1, rule.Value2, entry.Value);
                            break;
                    }
                    if (alarmTrigged)
                    {
                        AlarmEntry alarmEntry = new AlarmEntry
                        {
                            Particle = entry.Name,
                            Condition = rule.Condition.ToString(),
                            ConditionValues = new decimal[] { rule.Value1, rule.Value2 },
                            Date = entry.Date,
                            City = entry.City,
                            EntryValue = entry.Value,
                            Message = rule.Message
                        };
                        //call mqtt to publish!
                        jsonResponse = jss.Serialize(alarmEntry);
                        mClient.Publish(sTopics[1], Encoding.UTF8.GetBytes(jsonResponse));
                    }
                }
            }
            else
            {
                //Mostrar ao utilizador que existe uma nova particula...
                OnNewParticleReceived(this, new MyEventParticle(entry.Name));
                rulesDictionary.Add(entry.Name, null); //Null porq é particula nova logo nao tem regras nenhumas
            }
        }

        #region Rules Comparison Methods

        private bool LessThan(decimal ruleVal, decimal sensorVal)
        {
            return (ruleVal > sensorVal);
        }

        private bool GreaterThan(decimal ruleVal, decimal sensorVal)
        {
            return (ruleVal < sensorVal);
        }

        private bool Equals(decimal ruleVal, decimal sensorVal)
        {
            return (ruleVal == sensorVal);
        }

        private bool Between(decimal ruleVal1, decimal ruleVal2, decimal sensorVal)
        {
            return (ruleVal1 < sensorVal && ruleVal2 > sensorVal);
        }

        #endregion
    }
}
