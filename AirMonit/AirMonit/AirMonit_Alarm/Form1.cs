using AirMonit_Alarm.model;
using AirMonit_Alarm.Properties;
using AirMonit_DLog.Models;
using IAirEntries;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace AirMonit_Alarm
{
    
    public partial class AirMonit_Alarm : Form
    {
        public enum CRUD
        {
            CREATE, UPDATE, NONE
        }

        #region ComponentText
        string buttonTextStop = "Stop Service", buttonTextStart = "Start Service";
        #endregion

        private string SelectedParticle;
        public CRUD OPERATION = CRUD.NONE;
        private RuleCondition SelectedRule;

        string ICONFILEOK = AppDomain.CurrentDomain.BaseDirectory + @"file_ok.png";
        string ICONFILENOTOK = AppDomain.CurrentDomain.BaseDirectory + @"file_notok.png";

        XMLController xmlController;
        MessagingHandler mqttHandler;
        private static StringCollection topicsCollection = Settings.Default.Topics;
        private static string[] sTopics = new string[] {"", ""};
        private static string ip = Properties.Settings.Default.BrokerIP;

        private Dictionary<string, List<RuleCondition>> particlesRulesDictionary;

        string FILEPATHXML = AppDomain.CurrentDomain.BaseDirectory + @"trigger-rules.xml";
        string FILEPATHXSD = AppDomain.CurrentDomain.BaseDirectory + @"trigger-rules.xsd";

        public List<string> newParticlesList { get; set; }

        public AirMonit_Alarm()
        {
            InitializeComponent();

            try
            {
                topicsCollection.CopyTo(sTopics, 0);
                xmlController = new XMLController(FILEPATHXML, FILEPATHXSD);
                particlesRulesDictionary = new Dictionary<string, List<RuleCondition>>();

                newParticlesList = new List<string>();

                LoadXmlRulesDictionary();

            }
            catch (Exception ex)
            {
                MessageBox.Show("[FAIL LOAD] Loading alarm rules when: " + ex.Message);
                return;
            }

            StartMqttListener();


            #region Layout Setup
            btnStopServ.Text = (xmlController.GetAlarmStatus()) ? buttonTextStop : buttonTextStart;

            panelRulesInfo.Visible = false;
            RulesPanelSetUp();
            cbConditions.Items.Add("EQUALS");
            cbConditions.Items.Add("LESS");
            cbConditions.Items.Add("GREATER");
            cbConditions.Items.Add("BETWEEN");
            cbConditions.SelectedIndex = 0;


            txtFilePath.Text = FILEPATHXML;
            txtXsdPath.Text = FILEPATHXSD;
            PopulateParticlesList();

            if (xmlController.IsXmlValidated())
            {
                FileStatusIcon.ImageLocation = ICONFILEOK;
            }
            else
            {
                FileStatusIcon.ImageLocation = ICONFILENOTOK;
                MessageBox.Show(xmlController.XmlValidationError);
            }

            #endregion
            
        }

        #region Mqtt Start and Stop
        private void StartMqttListener()
        {
            try
            {
                if (xmlController.GetAlarmStatus())
                {
                    mqttHandler = new MessagingHandler(sTopics, ip, particlesRulesDictionary);
                    mqttHandler.OnNewParticleReceived += new MyEventNewParticleReceived(NewParticleFound);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("[FAIL CONNECTION] Connecting to mqtt when... " + ex.Message);
                return;
            }
        }

        private void StopMqttListener()
        {
            if(mqttHandler != null)
                mqttHandler.disconnectMqtt();
        }

        /// <summary>
        /// Swapps the <Rules> attribute value 'applyRule' in order disable the alert service
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartStopService(object sender, EventArgs e)
        {

            if (xmlController.GetAlarmStatus())
            {
                StopMqttListener();
                xmlController.DisableReading();
                UpdateXmlStatusButton(false);
            }
            else
            {
                xmlController.EnableReading();
                StartMqttListener();
                UpdateXmlStatusButton(true);
                
            }
            
        }

        #endregion

        #region Layout Status Methods

        private void SetAppLayout(CRUD state)
        {
            OPERATION = state;
            if(OPERATION == CRUD.CREATE || OPERATION == CRUD.UPDATE)
            {
                string applyBtnText= "Apply Rule", revokeBtnText= "Revoke Rule";
                EnableCRUDPanel(true);
                
                if (OPERATION == CRUD.CREATE)
                {
                    lbParticle.Text = SelectedParticle;
                    applyBtnText = "Create and Apply";
                    revokeBtnText = "Revoke";
                }

                btnApply.Text = applyBtnText;
                btnRevoke.Text = revokeBtnText;
                
            }
            else
            {
                cbConditions.SelectedIndex = 0;
                lbParticle.Text = SelectedParticle;
                EnableCRUDPanel(false);
            }
        }

        private void UpdateXmlStatusButton(bool status)
        {
            btnStopServ.Text = (status) ? buttonTextStop : buttonTextStart;
        }

        private void PopulateRulesList(string particle)
        {
            listParticleRules.Items.Clear();

            //Buscar dados
            List<RuleCondition> rules = xmlController.GetParticleRulesConditions(particle);

            foreach (RuleCondition data in rules)
            {
                ListViewItem listViewItem = new ListViewItem(data.Condition.ToString());
                listViewItem.Tag = data;

                string valueData = data.Value1.ToString();
                valueData += (data.Condition == Condition.BETWEEN) ? " : " + data.Value2 : "";
                listViewItem.SubItems.Add(valueData);
                listViewItem.SubItems.Add(data.Message);

                if (!data.ApplyRule)
                    listViewItem.Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Strikeout);
                listParticleRules.Items.Add(listViewItem);
            }
            SelectedRule = null;

        }

        private void DisplayRuleCRUD(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            SelectedRule = null;
            if (listParticleRules.SelectedItems.Count > 0)
            {
                RuleCondition rule = (RuleCondition)e.Item.Tag;
                SelectedRule = rule;
                SetAppLayout(CRUD.UPDATE);
                string condition = rule.Condition.ToString();
                cbConditions.Text = condition;

                decimal number;
                if (!Condition.BETWEEN.ToString().Equals(condition))
                {
                    number = rule.Value1;
                    txtNumBetween2.Visible = false;
                }
                else
                {
                    number = rule.Value1;
                    txtNumBetween2.Value = rule.Value2;
                    txtNumBetween2.Visible = true;
                }

                string message = rule.Message;
                lbParticle.Text = SelectedParticle;

                txtNumToCompare.Value = number;
                txtAlertMessage.Text = message;
            }
            else
            {
                SetAppLayout(CRUD.NONE);
            }
        }

        private void EnableCRUDPanel(bool status)
        {
            //Disabled
            cbConditions.Enabled = status;
            txtNumToCompare.Enabled = status;
            txtNumBetween2.Enabled = status;
            txtAlertMessage.Enabled = status;
            btnApply.Enabled = status;
            btnDelete.Enabled = status;
            btnRevoke.Enabled = status;

            //Clear prev values
            cbConditions.Text = "";
            txtNumToCompare.Value = 0;
            txtNumBetween2.Value = 0;
            txtAlertMessage.Text = "";
        }

        private void RulesPanelSetUp()
        {
            listParticleRules.Columns.Add("Condition", 100, HorizontalAlignment.Center);
            listParticleRules.Columns.Add("Value", 100, HorizontalAlignment.Center);
            listParticleRules.Columns.Add("Message", 300, HorizontalAlignment.Left);
        }

        private void PopulateParticlesList()
        {
            listAirParticles.Items.Clear();

            //abrir xml e ler todos os nos filhos do elemento root "rules"
            List<ParticleTag> particles = xmlController.GetParticlesName();
            Font fontApplyRule = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold);
            foreach (ParticleTag particle in particles)
            {
                var listViewItem = new ListViewItem(particle.Name);
                if (!particle.ApplyRule)
                {
                    listViewItem.Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Strikeout);
                }
                listAirParticles.Items.Add(listViewItem);
            }
            listAirParticles.View = View.List;
        }

        //Alter the right panel of rules to be visible and calls methods to populate it
        private void DisplayRules(object sender, EventArgs e)
        {
            if (listAirParticles.SelectedItems.Count > 0)
            {
                string particle = listAirParticles.SelectedItems[0].Text;
                SelectedParticle = particle;
                PopulateRulesList(particle);
            }
            panelRulesInfo.Visible = true;
            SetAppLayout(CRUD.NONE);
        }

        private void DisplaySecondNumTxt(object sender, EventArgs e)
        {
            txtNumBetween2.Visible = (cbConditions.Text.Equals("BETWEEN")) ? true : false;
        }

        private void ClearCRUDFields(object sender, EventArgs e)
        {
            cbConditions.Text = "";
            txtNumToCompare.Value = 0;
            txtNumBetween2.Value = 0;
            txtAlertMessage.Text = "";
        }

        private void CreateRule(object sender, EventArgs e)
        {
            SelectedRule = null;
            SetAppLayout(CRUD.CREATE);
        }

        #endregion

        #region Btn Apply Delete Revoke Events

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (xmlController.GetAlarmStatus())
            {
                MessageBox.Show("Stop the Service in order to update or create Rules");
            }
            else
            {
                RuleCondition rule;
                if (OPERATION == CRUD.UPDATE)
                {
                    if(SelectedRule == null)
                    {
                        MessageBox.Show("Please Select a rule to Update");
                        return;
                    }
                    rule = SelectedRule;
                    updateRule(rule);
                }
                else
                {
                    try
                    {
                        rule = CreateRule();
                    }
                    catch(FormException ex)
                    {
                        string reportErrors = "";
                        foreach (string error in ex.Errors)
                        {
                            reportErrors += Environment.NewLine +"->"+ error;
                        }
                        MessageBox.Show("Unable to Create Rule: "+ Environment.NewLine + reportErrors);
                        return;
                    }
                    
                }
                ClearCRUDFields(sender, e);
                xmlController.SaveRule(rule);

                LoadXmlRulesDictionary();
                PopulateRulesList(SelectedParticle);
            }
        }

        private void updateRule(RuleCondition rule)
        {
            rule.ApplyRule = true;
            rule.Condition = (Condition)Enum.Parse(typeof(Condition), cbConditions.Text);
            rule.Value1 = txtNumToCompare.Value;
            if (rule.Condition == Condition.BETWEEN)
            {
                rule.Value1 = txtNumBetween2.Value;
            }
            rule.Message = txtAlertMessage.Text;
        }

        private RuleCondition CreateRule()
        {
            RuleCondition rule;
            decimal[] values;
            if (txtNumBetween2.Enabled)
                values = new decimal[] { txtNumToCompare.Value, txtNumBetween2.Value };
            else
                values = new decimal[] { txtNumToCompare.Value };
            
                rule = new RuleCondition(
                                        SelectedParticle,
                                        true,
                                        cbConditions.Text,
                                        values,
                                        txtAlertMessage.Text);
            
            return rule;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //To avoid the delete new rule to show the msg since it will only clear the fields
            if (xmlController.GetAlarmStatus() && OPERATION == CRUD.UPDATE)
            {
                MessageBox.Show("Stop the Service in order to alter or create Rules");
            }
            else
            {
                if(OPERATION == CRUD.UPDATE)
                {
                    //xmlHandler.
                    DeleteRule(sender, e);
                    DisplayRules(sender, e);
                }
                ClearCRUDFields(sender, e);
                //Delete in the xml too

            }
        }

        private void btnRevoke_Click(object sender, EventArgs e)
        {
            if (xmlController.GetAlarmStatus())
            {
                MessageBox.Show("Stop the Service in order to alter or create Rules");
            }
            else
            {
                if (SelectedRule == null)
                {
                    MessageBox.Show("Please Select a rule to Revoke");
                    return;
                }
                RuleCondition rule = SelectedRule;
                if(OPERATION != CRUD.UPDATE)
                {
                    try
                    {
                        rule = CreateRule();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Unable to Create Rule: " + ex.Message);
                        return;
                    }
                    
                }
                ClearCRUDFields(sender, e);
                rule.ApplyRule = false;
                xmlController.SaveRule(rule);

                PopulateRulesList(SelectedParticle);
            }
        }

        private void DeleteRule(object sender, EventArgs e)
        {
            if (SelectedRule == null)
            {
                MessageBox.Show("Please Select a rule to Delete");
                return;
            }
            //Delete here the Rule from XMLFILE
            xmlController.DeleteRule(SelectedRule);
        }

        #endregion

        #region Form Events

        private void AirMonit_Alarm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopMqttListener();
        }

        private void AddNewParticles(object sender, MouseEventArgs e)
        {

            xmlController.DisableReading();

            foreach (string particle in newParticlesList)
            {
                xmlController.AddParticleToXML(particle);
            }

            xmlController.EnableReading();

            LoadXmlRulesDictionary();

            newParticlesList.Clear();
            PopulateParticlesList();
            btnAddParticle.Visible = false;
        }

        #endregion

        private void LoadXmlRulesDictionary()
        {
            if (particlesRulesDictionary != null)
            {
                particlesRulesDictionary.Clear();
                List<ParticleTag> particles = xmlController.GetParticlesName();
                foreach (ParticleTag particle in particles)
                {
                    if (particle.ApplyRule)
                    {
                        List<RuleCondition> particleRules = xmlController.GetParticleRulesConditions(particle.Name);

                        particlesRulesDictionary.Add(particle.Name, particleRules);
                    }
                    
                }
            }
        }

        private void showOptionsMenu(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right && SelectedParticle != null)
            {
                popUpMenuOptions.Show(Cursor.Position);
            }
        }

        private void applyParticle(object sender, EventArgs e)
        {
            UpdateParticleStatus(SelectedParticle, true);
        }

        private void revokeParticle(object sender, EventArgs e)
        {
            UpdateParticleStatus(SelectedParticle, false);
        }

        private void UpdateParticleStatus(string particle, bool status)
        {
            xmlController.DisableReading();

            xmlController.UpdateParticleStatus(particle, status);

            xmlController.EnableReading();

            LoadXmlRulesDictionary();

            PopulateParticlesList();
        }


        /// <summary>
        /// Is triggered when a new particle is added and it's thrown into mqtt dataUploader Topic
        /// Automaticly adds the particle name to the list of particles and updates the particles list in the UI form
        /// Adds the particle node to xml and xsd for validation
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void NewParticleFound(object source, MyEventParticle e)
        {
            
            this.BeginInvoke((MethodInvoker)delegate
            {
                Console.WriteLine(e.GetParticle());

                newParticlesList.Add(e.GetParticle());

                btnAddParticle.Visible = true;
                
            });
            
        }

    }
}
