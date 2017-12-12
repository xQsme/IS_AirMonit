using AirMonit_DLog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace AirMonit_Alarm.model
{
        #region Lançamento de Eventos

        public delegate void MyEventXmlChange(object source);

        #endregion

    public class XMLController
    {
        public event MyEventXmlChange OnXmlChange;

        private XMLManager xmlManager;
        public string XmlValidationError { get; internal set; }

        public XMLController(string xmlFile, string xsdFile)
        {
            xmlManager = new XMLManager(xmlFile, xsdFile);
        }

        public bool ChangeXmlFile(string xmlFile)
        {

            xmlManager.XmlFilePath = xmlFile;
            OnXmlChange(this);
            if (!xmlManager.ValidateXml(xmlFile))
            {
                XmlValidationError = xmlManager.ValidationMessage;
                return false;
            }
            return true;
        }

        public void ChangeXsdFile(string xsdFile)
        {

            xmlManager.XsdFilePath = xsdFile;

        }

        #region Getters

        public bool GetAlarmStatus()
        {
            return xmlManager.alarmServiceStatus();
        }

        public List<RuleCondition> GetParticleRulesConditions(string particle)
        {
            List<RuleCondition> list = new List<RuleCondition>();
            XmlNodeList collection = xmlManager.GetParticleRules(particle);
            
            foreach (XmlNode node in collection)
            {
                RuleCondition ruleCond = ParseXML(node);
                if (ruleCond == null)
                    continue;

                list.Add(ruleCond);
            }

            return list;

        }

        public List<ParticleTag> GetParticlesName()
        {
            List<ParticleTag> temp = new List<ParticleTag>();
            foreach(XmlNode node in xmlManager.GetParticlesList()){
                if(node != null)
                {
                    ParticleTag ptag = new ParticleTag();
                    ptag.ApplyRule = bool.Parse(node.Attributes["applyRule"].InnerText);
                    ptag.Name = node.Name;
                    temp.Add(ptag);
                }
            }
            return temp;
        }

        #endregion

        #region CUD functions

        public void AddParticleToXML(string particle)
        {
            DisableReading();

            xmlManager.AddNewParticleToSchema(particle);
            xmlManager.AddNewParticleToXML(particle);
            xmlManager.Save();

            EnableReading();

        }
       
        public void SaveRule(RuleCondition rule)
        {
            XmlNode xmlRule = ParseRule(rule);

            //It's a new node because there's no previous XmlNode state checking Parent has the same result
            if (rule.StoredNode == null)
            {
                xmlManager.AddRule(rule.Particle, xmlRule);
            }
            else
            {
                //O parent pode nao ser null mas o previous pode ser... ou seja o 1º elemento da lista
                xmlManager.UpdateRule(xmlRule, rule.StoredNode);
            }

            OnXmlChange(this);
        }

        public void DeleteRule(RuleCondition rule)
        {
            XmlNode nodeToRemove = rule.StoredNode;

            xmlManager.DeleteRule(nodeToRemove);

            OnXmlChange(this);

        }

        public void DisableReading()
        {
            xmlManager.DeactivateRules();
        }

        public void EnableReading()
        {
            xmlManager.ActivateRules();
            OnXmlChange(this);
        }

        #endregion

        #region Parsers

        private XmlNode ParseRule(RuleCondition rule)
        {
            XmlNode ruleNode;
            
            if (rule.Condition == Condition.BETWEEN)
            {
                ruleNode = xmlManager.GetRuleBetweenExample();

                ruleNode["num1"].InnerText = rule.Value1.ToString();
                ruleNode["num2"].InnerText = rule.Value2.ToString();

            }
            else
            {
                ruleNode = xmlManager.GetRuleDefaultExample(rule.Condition.ToString().ToLower());
                ruleNode["num"].InnerText = rule.Value1.ToString();

            }

            ruleNode.Attributes["applyRule"].InnerText = rule.ApplyRule.ToString().ToLower();
            ruleNode["msg"].InnerText = rule.Message;

            return ruleNode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rule"><between> or <equals>...</param>
        /// <returns></returns>
        private RuleCondition ParseXML(XmlNode rule)
        {
            try
            {
                decimal[] values;
                bool applyRule = Convert.ToBoolean(rule.Attributes["applyRule"].Value); //<equals applyRule="true">
                string condition = rule.Name; //<equals>
                if (condition.ToLower().Equals("between"))
                    values = new decimal[] { decimal.Parse(rule["num1"].InnerText), decimal.Parse(rule["num2"].InnerText) };
                else
                    values = new decimal[] { decimal.Parse(rule["num"].InnerText) };
                string message = rule["msg"].InnerText;

                return new RuleCondition(rule, applyRule, condition, values, message);

            }
            catch (Exception ex)
            {
                Console.WriteLine("[PARSING]" + ex.Message);
            }
            return null;
        }

        #endregion

        public bool IsXmlValidated()
        {
            bool isValid = xmlManager.ValidateXml();

            if (!isValid)
            {
                XmlValidationError = xmlManager.ValidationMessage;
            }
            return isValid;
        }

        public void UpdateParticleStatus(string particle, bool status)
        {
            xmlManager.UpdateParticleStatus(particle, status);
            xmlManager.Save();
            OnXmlChange(this);
        }
    }
}
