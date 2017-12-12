using AirMonit_DLog.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace AirMonit_Alarm
{
    internal class XMLManager
    {
        
        #region XPath strings
        private const string XPATH_PARTICLES = "/rules/*";
        private const string XPATH_ROOT = "/rules";

        //Usage: String.Format(XPATH_PARTICLE_RULES, "CO2")
        private const string XPATH_PARTICLE = "/rules/{0}";
        private const string XPATH_PARTICLE_RULES = "/rules/{0}/*";
        private const string XPATH_PARTICLE_RULES_STATUS = "/rules/{0}[@applyRule='{1}']";
        private const string XPATH_PARTICLE_RULE_COND = "/rules/{0}/{1}";

        //UPDATE NODE
        private const string XPATH_RULE_UPDATE_NUM = "num";
        private const string XPATH_RULE_UPDATE_NUM1 = "num1";
        private const string XPATH_RULE_UPDATE_NUM2 = "num2";
        private const string XPATH_RULE_UPDATE_MSG = "msg";
        #endregion

        #region New Particles in Schema
        private string XsdStringForNewParticleElement = "<!--@NextElement-->";
        private string xsdNewParticleEle = "<xs:element maxOccurs=\"1\" name=\"{0}\" type=\"particleRuleType\" />";

        #endregion

        private string FILEPATHXSD;
        private string FILEPATHXML;
        internal string XmlFilePath
        {
            get { return FILEPATHXML; }
            set
            {
                FILEPATHXML = value;
                doc = null;
                doc = new XmlDocument();
                doc.Load(FILEPATHXML);
            }
        }

        internal string XsdFilePath
        {
            get { return FILEPATHXSD; }
            set
            {
                FILEPATHXSD = value;
            }
        }

        private bool isValid;

        public string ValidationMessage { get; private set; }

        private XmlDocument doc;

        internal XMLManager(string xmlFile, string xsdFile)
        {
            this.FILEPATHXSD = xsdFile;
            this.XmlFilePath = xmlFile;

        }

        #region XML FILE GETTERS

        internal bool alarmServiceStatus()
        {
            XmlNode xmlRoot = doc.SelectSingleNode(XPATH_ROOT);

            string rootAttribute = xmlRoot.Attributes["applyRule"].Value;
            try
            {
                bool ruleFileStatus = Convert.ToBoolean(rootAttribute);
                return ruleFileStatus;
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Valor do atributo applyRule do root 'Rules' do ficheiro xml nao é convertivel para booleano" + ex.GetBaseException().Message);
                throw new Exception("Erro no ficheiro trigger-rules.xml elemento root '<Rules>' no atributo 'applyRule' não possui um valor string convertivel para booleano: {'" + rootAttribute + "'} \n input valido: 'true'");
            }
        }

        internal XmlNode GetRoot()
        {
            return doc.SelectSingleNode(XPATH_ROOT);
        }

        internal XmlNode GetParticle(string particle)
        {
            return doc.SelectSingleNode(String.Format(XPATH_PARTICLE, particle.ToUpper()));
        }

        /// <summary>
        /// Gets the <particle> node
        /// </summary>
        /// <returns></returns>
        internal XmlNodeList GetParticlesList()
        {
            return doc.SelectNodes(XPATH_PARTICLES);
        }

        /// <summary>
        /// Get's the conditions of that particle
        /// </summary>
        /// <param name="particle">NO2</param>
        /// <returns></returns>
        internal XmlNodeList GetParticleRules(string particle)
        {
            return doc.SelectNodes(
                                    String.Format(XPATH_PARTICLE_RULES, particle)
                                    );
        }

        /// <summary>
        /// Gets the rule nodes wich are enabled(true) or disabled(false)
        /// </summary>
        /// <param name="particle">O3</param>
        /// <param name="status">applyRule='true'</param>
        /// <returns></returns>
        internal XmlNodeList GetParticleRulesStatus(string particle, bool status)
        {
            return doc.SelectNodes(
                                    String.Format(XPATH_PARTICLE_RULES_STATUS, particle, status.ToString())
                                    );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        internal XmlNodeList GetParticleRuleCondition(string particle, string condition)
        {
            return doc.SelectNodes(
                                    String.Format(XPATH_PARTICLE_RULE_COND, particle, condition.ToLower())
                                    );
        }

        #endregion

        #region XML FILE SETTERS

        /// <summary>
        /// Appends to the <paramref name="particle"/> the new <paramref name="rule"/>
        /// </summary>
        /// <param name="particle">NO2</param>
        /// <param name="rule"></param>
        internal void CreateRuleElement(string particle, XmlNode rule)
        {
            GetParticle(particle).AppendChild(rule);
        }

        internal void UpdateParticleStatus(string particle, bool status)
        {
            
            XmlNode particleNode = doc.SelectSingleNode(String.Format(XPATH_PARTICLE, particle));
            particleNode.Attributes["applyRule"].InnerText = status.ToString().ToLower();

        }

        /// <summary>
        /// Adicionar novas particulas
        /// </summary>
        /// <param name="particleName"></param>
        internal void AddNewParticleToSchema(string particleName)
        {
            List<string> txtLines = File.ReadAllLines(this.FILEPATHXSD).ToList();   //Fill a list with the lines from the txt file.

            int line = txtLines.FindIndex(s =>
                s.Replace('\u0009'.ToString(), " ").Trim().Equals(XsdStringForNewParticleElement)
            );

            txtLines.Insert(line, String.Format(xsdNewParticleEle, particleName));  //Insert the line you want to add last under the tag 'item1'.
            File.WriteAllLines(this.FILEPATHXSD, txtLines);
        }

        internal void AddNewParticleToXML(string particle)
        {
            //<NO2 applyRule="true">< rule applyRule = "false" >
            XmlElement particleNode = doc.CreateElement(particle);
            particleNode.SetAttribute("applyRule", false.ToString().ToLower());
            GetRoot().AppendChild(particleNode);
            Save();
        }

        //Does not manage the copy of childNodes from oldRuleNode to ruleNode
        internal void UpdateRule(XmlNode ruleNode, XmlNode oldNode)
        {
            XmlNode parent = oldNode.ParentNode;
            //ERRO AQUI ao fazer revoke ruleNode e oldNode veem com o ParentNode a null...
            parent.ReplaceChild(ruleNode, oldNode);

            Save();
        }

        internal void DeleteRule(XmlNode nodeToRemove)
        {
            nodeToRemove.ParentNode.RemoveChild(nodeToRemove);
            Save();
        }

        //Places a new Rule at the end of all rules from that particle
        internal void AddRule(string particle, XmlNode ruleNode)
        {
            XmlNode parent = GetParticle(particle);

            parent.AppendChild(ruleNode);

            Save();
        }

        /// <summary>
        /// Saves the changes to the XML Document
        /// </summary>
        internal void Save()
        {
            doc.Save(FILEPATHXML);
        }

        #endregion

        #region HelperFunctions

        internal XmlElement GetRuleBetweenExample()
        {

            XmlElement conditionNode = doc.CreateElement("between");
            conditionNode.SetAttribute("applyRule", true.ToString().ToLower());

            XmlElement val1, val2;
            val1 = doc.CreateElement("num1");
            val2 = doc.CreateElement("num2");
            conditionNode.AppendChild(val1);
            conditionNode.AppendChild(val2);

            XmlElement message = doc.CreateElement("msg");

            conditionNode.AppendChild(message);
            return conditionNode;

        }

        internal XmlElement GetRuleDefaultExample(string condition)
        {

            XmlElement conditionNode = doc.CreateElement(condition);
            conditionNode.SetAttribute("applyRule", true.ToString().ToLower());

            XmlElement val;
            val = doc.CreateElement("num");
            conditionNode.AppendChild(val);

            XmlElement message = doc.CreateElement("msg");

            conditionNode.AppendChild(message);
            return conditionNode;

        }

        #endregion  

        internal void ActivateRules()
        {
            GetRoot().Attributes["applyRule"].Value = true.ToString().ToLower();
            Save();
        }

        internal void DeactivateRules()
        {
            GetRoot().Attributes["applyRule"].Value = false.ToString().ToLower();
            Save();
        }

        private void RenameNode(XmlNode oldNode, string newNameNode)
        {
            XmlNode newNode = doc.CreateElement(newNameNode.ToUpper());
            XmlNode parent = oldNode.ParentNode;
            parent.ReplaceChild(newNode, oldNode);

            foreach (XmlNode childNode in oldNode.ChildNodes)
            {
                newNode.AppendChild(childNode.CloneNode(true));
            }
        }
        
        internal bool ValidateXml()
        {
            isValid = true;
            ValidationMessage = "[Documento válido]";
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(FILEPATHXML);
                doc.Schemas.Add(null, FILEPATHXSD);

                ValidationEventHandler myEvent = new ValidationEventHandler(trataEvento);

                doc.Validate(myEvent);
            }
            catch (Exception ex)
            {
                try
                {
                    XmlSchemaValidationException shemaEx = (XmlSchemaValidationException)ex;
                }
                catch (Exception)
                {
                    ValidationMessage = "Could not found trigger-rules-xsd";
                }
                
                isValid = false;
                ValidationMessage += "[Documento inválido]: " + ex.Message;
            }
            return isValid;
        }

        internal bool ValidateXml(string xmlPath)
        {
            isValid = true;
            ValidationMessage = "[Documento válido]";
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(xmlPath);
                doc.Schemas.Add(null, FILEPATHXSD);

                ValidationEventHandler myEvent = new ValidationEventHandler(trataEvento);

                doc.Validate(myEvent);
            }
            catch (Exception ex)
            {
                try
                {
                    XmlSchemaValidationException shemaEx = (XmlSchemaValidationException)ex;
                }
                catch (Exception)
                {
                    ValidationMessage = "Could not found trigger-rules-xsd";
                }

                isValid = false;
                ValidationMessage += "[Documento inválido]: " + ex.Message;
            }
            return isValid;
        }

        private void trataEvento(object sender, ValidationEventArgs e)
        {
            isValid = false;
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    ValidationMessage = "[Documento inválido: ERROR]" + e.Message;
                    break;
                case XmlSeverityType.Warning:
                    ValidationMessage = "[Documento inválido: WARNING]" + e.Message;
                    break;
                default:
                    break;
            }

        }

    }
}
