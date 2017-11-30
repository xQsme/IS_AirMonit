using AirMonit_DLog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace AirMonit_Alarm.model
{
    public class XMLController
    {

        private XMLManager xmlManager;
        public string XmlValidationError { get; internal set; }

        public XMLController(string xmlFile, string xsdFile)
        {
            xmlManager = new XMLManager(xmlFile, xsdFile);
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

        public List<string> GetParticlesName()
        {
            List<string> temp = new List<string>();
            foreach(XmlNode node in xmlManager.GetParticlesList()){
                if(node != null)
                {
                    temp.Add(node.Name);
                }
            }
            return temp;
        }

        #endregion

        #region CUD functions

        public void AddParticleToXML(string particle)
        {
            xmlManager.AddNewParticleToSchema(particle);
            xmlManager.AddNewParticleToXML(particle);
            xmlManager.Save();

            //TODO: Parar o servico para poder fazer esta funcao...
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
            
        }

        public void DeleteRule(RuleCondition rule)
        {
            XmlNode nodeToRemove = rule.StoredNode;

            xmlManager.DeleteRule(nodeToRemove);
            
        }

        public void DisableReading()
        {
            xmlManager.DeactivateRules();
        }

        public void EnableReading()
        {
            xmlManager.ActivateRules();
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
        
    }
}
