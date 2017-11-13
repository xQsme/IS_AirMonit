using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace AirMonit_Alarm
{
    public partial class airMonit_alarm : Form
    {
        string FILEPATH = AppDomain.CurrentDomain.BaseDirectory + @"trigger-rules.xml";
        XmlDocument doc;
        public airMonit_alarm()
        {
            InitializeComponent();
            //1º abrir o xml ler as particulas para uma lista
            openXmlTriggerRules();
            saveChanges();
            populateParticlesList();
        }

        private void saveChanges()
        {
            try
            {
                if(doc != null)
                {
                    //validade xml??
                    doc.Save(FILEPATH);
                }
            }
            catch (Exception)
            {
                //Bunch of alerts about the issue..
            }
        }

        private void openXmlTriggerRules()
        {
            doc = new XmlDocument();
            doc.Load(FILEPATH);
        }

        private void populateParticlesList()
        {
            listAirParticles.Items.Clear();

            //abrir xml e ler todos os nos filhos do elemento root "rules"
            XmlNodeList particles = doc.SelectNodes("/rules/*");

            foreach (XmlNode particle in particles)
            {
                var listViewItem = new ListViewItem(particle.Name);
                if (particle.Attributes["applyRule"].Value.Equals("false"))
                {
                    
                    listViewItem.BackColor = Color.Red;
                    
                }
                listAirParticles.Items.Add(listViewItem);
            }
            listAirParticles.View = View.List;
            

        }
    }
}
