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
    enum Condition
    {
        EQUALS, BETWEEN, LESS, GREATER
    }

    class RuleCondition
    {
        private XmlNode node { get; set; }
        public bool ApplyRule { get; set; }
        public Condition Condition { get; set; }
        public decimal Value1 { get; set; }
        public decimal Value2 { get; set; }
        public string Message { get; set; }
        public string Particle { get; }

        /// <summary>
        /// Expecting the rule node so i can access the ApplyRule
        /// This method will be called with a node from xml so if xml file is incorrect...
        /// </summary>
        /// <param name="node"><rule></param>
        public RuleCondition(XmlNode node)
        {
            this.node = node;
            XmlNode conditionNode = node.FirstChild;
            try
            {
                this.ApplyRule = bool.Parse(node.Attributes["applyRule"].Value);
            }
            catch (Exception)
            {
                throw new Exception("Unable to convert ApplyRule value: '" + node.Attributes["applyRule"].Value + "' into bool");
            }
            try
            {
                this.Condition = (Condition)Enum.Parse(typeof(Condition), conditionNode.Name, true);
            }
            catch(Exception ex)
            {
                throw new Exception("Unable to convert: '"+ conditionNode.Name + "' into enum of Condition");
            }

            string val1="nothing for value 1", val2 = "nothing for value 2";

            //ALL other conditions
            if (!Condition.Equals(Condition.BETWEEN)){
                try
                {
                    val1 = conditionNode["num"].InnerText;
                    this.Value1 = decimal.Parse(val1);
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to convert: '" + conditionNode["num"].InnerText + "' into integer");
                }
            }
            else
            {
                //THE BETWEEN CONDITION BECAUSE OF ITS 2 values
                try
                {
                    val1 = conditionNode["num1"].InnerText;
                    val2 = conditionNode["num2"].InnerText;
                    this.Value1 = decimal.Parse(val1);
                    this.Value2 = decimal.Parse(val2);
                }
                catch (Exception ex)
                {

                    throw new Exception("Unable to convert: '" + val2 + "' into integer");
                }
            }

            this.Message = conditionNode["msg"].InnerText;

        }

        /// <summary>
        /// User input will be entered here
        /// This is for new rules
        /// that's why the <paramref name="particle"/>
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="applyRule"></param>
        /// <param name="condition"></param>
        /// <param name="values"></param>
        /// <param name="message"></param>
        public RuleCondition(string particle, bool applyRule, string condition, decimal[] values, string message)
        {
            if(particle == null || particle == "")
            {
                throw new NullReferenceException("Particle cannot be empty");
            }

            if (message == null || message == "")
            {
                throw new NullReferenceException("Alert Message cannot be empty");
            }

            try
            {
                this.Particle = particle;
                this.ApplyRule = applyRule;
                    try
                    {
                        this.Condition = (Condition)Enum.Parse(typeof(Condition), condition);
                    }
                    catch(Exception ex)
                    {
                        throw new Exception("'"+condition+"' is not a valid Condition");
                    }
                
                this.Value1 = values[0];
                if (values.Length > 1)
                {
                    this.Value2 = values[1];
                }
                this.Message = message;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public XmlNode GetNode()
        {
            return this.node;
        }
    }

    class XMLHandler
    {
        
        #region XPath strings
        private const string XPATH_PARTICLES = "/rules/*";
        private const string XPATH_ROOT = "/rules";

        //Usage: String.Format(XPATH_PARTICLE_RULES, "CO2")
        private const string XPATH_PARTICLE = "/rules/{0}";
        private const string XPATH_PARTICLE_RULES = "/rules/{0}/rule";
        private const string XPATH_PARTICLE_RULES_STATUS = "/rules/{0}/rule[@applyRule='{1}']";
        private const string XPATH_PARTICLE_RULE_COND = "/rules/{0}/rule/{1}";

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
        public string filepath
        {
            get { return FILEPATHXML; }
            set {
                FILEPATHXML = value;
                doc = null;
                doc = new XmlDocument();
                doc.Load(FILEPATHXML);
            }
        }

        private bool isValid;

        public string ValidationMessage { get; private set; }

        private XmlDocument doc;


        public XMLHandler(string xmlFile, string xsdFile)
        {
            this.FILEPATHXSD = xsdFile;
            this.filepath = xmlFile;

        }

        #region XML FILE GETTERS

        public bool alarmServiceStatus()
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

        public XmlNode GetRoot()
        {
            return doc.SelectSingleNode(XPATH_ROOT);
        }

        public XmlNode GetParticle(string particle)
        {
            return doc.SelectSingleNode(String.Format(XPATH_PARTICLE, particle.ToUpper()));
        }

        /// <summary>
        /// Gets the <particle> node
        /// </summary>
        /// <returns></returns>
        public XmlNodeList GetParticlesList()
        {
            return doc.SelectNodes(XPATH_PARTICLES);
        }

        /// <summary>
        /// Get's the rule node of that particle
        /// </summary>
        /// <param name="particle">NO2</param>
        /// <returns></returns>
        public XmlNodeList GetParticleRules(string particle)
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
        public XmlNodeList GetParticleRulesStatus(string particle, bool status)
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
        public XmlNodeList GetParticleRuleCondition(string particle, string condition)
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
        public void CreateRuleElement(string particle, RuleCondition rule)
        {
            GetParticle(particle).AppendChild(ParseXML(rule));
        }

        /// <summary>
        /// Adicionar novas particulas
        /// </summary>
        /// <param name="particleName"></param>
        public void addNewParticleToSchema(string particleName)
        {
            List<string> txtLines = File.ReadAllLines(this.FILEPATHXSD).ToList();   //Fill a list with the lines from the txt file.

            int line = txtLines.FindIndex(s =>
                s.Replace('\u0009'.ToString(), " ").Trim().Equals(XsdStringForNewParticleElement)
            );

            txtLines.Insert(line, String.Format(xsdNewParticleEle, "Minha-Particula"));  //Insert the line you want to add last under the tag 'item1'.
            File.WriteAllLines(this.FILEPATHXSD, txtLines);
        }

        public void updateNode(RuleCondition rule)
        {
            if(rule.GetNode() != null)
            {
                XmlNode node = rule.GetNode();
                node.Attributes["applyRule"].InnerText = rule.ApplyRule.ToString();

                XmlNode condNode = node[rule.Condition.ToString().ToLower()];

                if(condNode != null)
                {
                    if (rule.Condition.Equals(Condition.BETWEEN))
                    {
                        condNode[XPATH_RULE_UPDATE_NUM1].InnerText = rule.Value1.ToString();
                        condNode[XPATH_RULE_UPDATE_NUM2].InnerText = rule.Value2.ToString();
                    }
                    else
                    {
                        condNode[XPATH_RULE_UPDATE_NUM].InnerText = rule.Value1.ToString();
                    }

                
                    condNode[XPATH_RULE_UPDATE_MSG].InnerText = rule.Message;
                }
                

            }
            else
            {
                throw new Exception("This method is only for RuleConditions who were parsed from XmlNode");
            }
        }

        public void deleteRule(RuleCondition rule)
        {
            XmlNode nodeToRemove = rule.GetNode();
            nodeToRemove.ParentNode.RemoveChild(nodeToRemove);
            Save();
        }
        
        #endregion
        
        public List<RuleCondition> GetRulesConditions(string particle)
        {
            return ParseCondition(GetParticleRules(particle));
        }

        public static List<RuleCondition> ParseCondition(XmlNodeList collection)
        {
            List<RuleCondition> list = new List<RuleCondition>();
            

            foreach (XmlNode node in collection)
            {
                try
                {
                    list.Add(new RuleCondition(node));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("MINHA EXCECAO" + ex.Message);
                }
            }

            
            return list;
        }

        private XmlNode ParseXML(RuleCondition rule)
        {

            XmlElement ruleEle = doc.CreateElement("rule");
            ruleEle.SetAttribute("applyRule", rule.ApplyRule.ToString());

            XmlElement condition = doc.CreateElement(rule.Condition.ToString().ToLower());

            XmlElement val1, val2;
            if (rule.Condition != Condition.BETWEEN)
            {
                val1 = doc.CreateElement("num");
                val1.InnerText = rule.Value1.ToString();
                condition.AppendChild(val1);
            }
            else
            {
                val1 = doc.CreateElement("num1");
                val2 = doc.CreateElement("num2");
                val1.InnerText = rule.Value1.ToString();
                val2.InnerText = rule.Value2.ToString();
                condition.AppendChild(val1);
                condition.AppendChild(val2);
            }

            XmlElement message = doc.CreateElement("msg");
            message.InnerText = rule.Message;
            condition.AppendChild(message);
            ruleEle.AppendChild(condition);
            return ruleEle;

        }

        public void revokeRule(RuleCondition rule)
        {
            rule.ApplyRule = false;
            if (rule.GetNode() == null)
            {
                CreateRuleElement(rule.Particle, rule);
            }
            else
            {
                updateNode(rule);
            }
            Save();
        }

        public bool ValidateXml()
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
                XmlSchemaValidationException shemaEx = (XmlSchemaValidationException)ex;
                isValid = false;
                ValidationMessage = "[Documento inválido]: " + ex.Message;
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

        public void SaveRule(RuleCondition rule)
        {
            XmlNode node = ParseXML(rule);

            //Se o elemento nao tiver pai definido(quando é novo elemento)
            XmlNode nodeToSave = rule.GetNode();
            XmlNode parent;
            //is a new rule! no node yet created...
            if (nodeToSave == null)
            {
                parent = GetParticle(rule.Particle);
                parent.AppendChild(node);
            }
            else
            {
                parent = nodeToSave.ParentNode;
                updateNode(rule);
            }
            
            Save();
        }

        /// <summary>
        /// Saves the changes to the XML Document
        /// </summary>
        public void Save()
        {
            doc.Save(FILEPATHXML);
        }

    }
}
