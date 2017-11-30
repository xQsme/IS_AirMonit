using AirMonit_Alarm.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AirMonit_DLog.Models
{
    public enum Condition
    {
        EQUALS, BETWEEN, LESS, GREATER
    }

    public class RuleCondition
    {
        
        public bool ApplyRule { get; set; }
        internal XmlNode Parent { get; set; }

        /// <summary>
        /// It's the node that was load (doesn't suffer any change as long as Save method isn't called)
        /// </summary>
        internal XmlNode StoredNode { get; set; }
        public string Particle { get; }
        public Condition Condition { get; set; }
        public decimal Value1 { get; set; }
        public decimal Value2 { get; set; }
        public string Message { get; set; }

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
            List<string> erros = new List<string>();
            if (particle == null || particle == "")
            {
                erros.Add("Particle cannot be empty");
            }

            if (message == null || message == "")
            {
                erros.Add("Alert Message cannot be empty");
            }

            try
            {
                this.Particle = particle;
                this.ApplyRule = applyRule;
                try
                {
                    this.Condition = (Condition)Enum.Parse(typeof(Condition), condition.ToUpper());
                }
                catch (Exception ex)
                {
                    erros.Add("'" + condition + "' is not a valid Condition");
                }

                this.Value1 = values[0];
                if (values.Length > 1)
                {
                    this.Value2 = values[1];
                }
                this.Message = message;
            }
            catch (Exception ex)
            {
                erros.Add(ex.Message);
            }
            if(erros.Count > 0)
            {
                throw new FormException(erros);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sibling">Node sibling to this</param>
        internal RuleCondition(XmlNode ruleNode, bool applyRule, string condition, decimal[] values, string message) 
            : this(ruleNode.ParentNode.Name, applyRule, condition, values, message)
        {
            this.Parent = ruleNode.ParentNode;
            this.StoredNode = ruleNode;
        }
    }
}
