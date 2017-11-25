using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace AirMonit_Alarm
{
    
    public partial class airMonit_alarm : Form
    {
        public class FakeRecord
        {
            public string condition { get; set; }
            public int number { get; set; }
            public string message { get; set; }
            public Boolean applyRule { get; set; }
            public FakeRecord(string cond, int num, string msg, Boolean apply)
            {
                condition = cond;
                number = num;
                message = msg;
                applyRule = apply;
            }
        }
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

        XMLHandler xmlHandler;
        string FILEPATHXML = AppDomain.CurrentDomain.BaseDirectory + @"trigger-rules.xml";
        string FILEPATHXSD = AppDomain.CurrentDomain.BaseDirectory + @"trigger-rules.xsd";

        public airMonit_alarm()
        {
            InitializeComponent();

            xmlHandler = new XMLHandler(FILEPATHXML, FILEPATHXSD);

            #region Layout Setup
            btnStopServ.Text = (xmlHandler.alarmServiceStatus()) ? buttonTextStop : buttonTextStart;

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

            if (xmlHandler.ValidateXml())
            {
                FileStatusIcon.ImageLocation = ICONFILEOK;
            }
            else
            {
                FileStatusIcon.ImageLocation = ICONFILENOTOK;
            }

            #endregion
            
        }
        
        #region XmlHandler Calls

        /// <summary>
        /// Swapps the <Rules> attribute value 'applyRule' in order disable the alert service
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartStopService(object sender, EventArgs e)
        {
            //Aqui pode haver erro na label do botao!
            //Toggle the text in the button that stops/starts the service

            XmlNode xmlRoot = xmlHandler.GetRoot();

            try
            {
                xmlRoot.Attributes["applyRule"].Value = (xmlHandler.alarmServiceStatus()) ? false.ToString() : true.ToString();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().Message);
                Process.Start(FILEPATHXML);
            }
            
            xmlHandler.Save();
            btnStopServ.Text = (xmlHandler.alarmServiceStatus()) ? buttonTextStop : buttonTextStart;


            //Start Service!
        }

        private void CreateRule(object sender, EventArgs e)
        {
            SelectedRule = null;
            SetAppLayout(CRUD.CREATE);
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
                    revokeBtnText = "Create and Revoke";
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

        private void PopulateRulesList(string particle)
        {
            listParticleRules.Items.Clear();

            //Buscar dados
            List<RuleCondition> rules = xmlHandler.GetRulesConditions(particle);

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
            XmlNodeList particles = xmlHandler.GetParticlesList();
            Font fontNotApplyRule = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Strikeout);
            Font fontApplyRule = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold);
            foreach (XmlNode particle in particles)
            {
                var listViewItem = new ListViewItem(particle.Name);

                if (particle.Attributes["applyRule"].Value.Equals("false"))
                {
                    listViewItem.BackColor = Color.Red;
                    listViewItem.Font = fontNotApplyRule;
                }
                else
                {
                    listViewItem.Font = fontApplyRule;
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

        #endregion

        #region Btn Apply Delete Revoke Events

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (xmlHandler.alarmServiceStatus())
            {
                MessageBox.Show("Stop the Service in order to update or create Rules");
            }
            else
            {
                RuleCondition rule;
                if (OPERATION == CRUD.UPDATE)
                {
                    rule = SelectedRule;
                    updateRule(rule);
                }
                else
                {
                    try
                    {
                        rule = CreateRule();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Unable to Create Rule: "+ ex.Message);
                        return;
                    }
                    ClearCRUDFields(sender, e);
                }
                
                //rule pode vir null senao escolher campos ou a particula..
                xmlHandler.SaveRule(rule);
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

            try
            {
            rule = new RuleCondition(
                                        SelectedParticle,
                                        true,
                                        cbConditions.Text,
                                        values,
                                        txtAlertMessage.Text);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return rule;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //To avoid the delete new rule to show the msg since it will only clear the fields
            if (xmlHandler.alarmServiceStatus() && OPERATION == CRUD.UPDATE)
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
            if (xmlHandler.alarmServiceStatus())
            {
                MessageBox.Show("Stop the Service in order to alter or create Rules");
            }
            else
            {
                RuleCondition rule;
                if (OPERATION == CRUD.UPDATE)
                {
                    xmlHandler.revokeRule(SelectedRule);
                }
                else
                {
                    try
                    {
                        rule = CreateRule();
                        rule.ApplyRule = false;
                        xmlHandler.SaveRule(rule);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Unable to Create Rule: " + ex.Message);
                        return;
                    }
                    ClearCRUDFields(sender, e);
                }

                PopulateRulesList(SelectedParticle);
            }
        }

        private void DeleteRule(object sender, EventArgs e)
        {
            //Delete here the Rule from XMLFILE
            xmlHandler.deleteRule(SelectedRule);
        }

        #endregion

    }
}
