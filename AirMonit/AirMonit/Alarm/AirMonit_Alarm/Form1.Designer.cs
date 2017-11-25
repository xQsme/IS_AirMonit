namespace AirMonit_Alarm
{
    partial class airMonit_alarm
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        private System.Windows.Forms.Panel panelRulesInfo;
        private System.Windows.Forms.ListView listAirParticles;
        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelRulesInfo = new System.Windows.Forms.Panel();
            this.listParticleRules = new System.Windows.Forms.ListView();
            this.listAirParticles = new System.Windows.Forms.ListView();
            this.panelCRUD_Rules = new System.Windows.Forms.Panel();
            this.txtNumBetween2 = new System.Windows.Forms.NumericUpDown();
            this.txtNumToCompare = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbParticle = new System.Windows.Forms.Label();
            this.btnRevoke = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.txtAlertMessage = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbConditions = new System.Windows.Forms.ComboBox();
            this.btnStopServ = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.btnNewRule = new System.Windows.Forms.Button();
            this.txtXsdPath = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.FileStatusIcon = new System.Windows.Forms.PictureBox();
            this.panelRulesInfo.SuspendLayout();
            this.panelCRUD_Rules.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumBetween2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumToCompare)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FileStatusIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // panelRulesInfo
            // 
            this.panelRulesInfo.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panelRulesInfo.Controls.Add(this.listParticleRules);
            this.panelRulesInfo.Location = new System.Drawing.Point(234, 65);
            this.panelRulesInfo.Name = "panelRulesInfo";
            this.panelRulesInfo.Size = new System.Drawing.Size(540, 269);
            this.panelRulesInfo.TabIndex = 1;
            // 
            // listParticleRules
            // 
            this.listParticleRules.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listParticleRules.FullRowSelect = true;
            this.listParticleRules.GridLines = true;
            this.listParticleRules.Location = new System.Drawing.Point(3, 3);
            this.listParticleRules.MultiSelect = false;
            this.listParticleRules.Name = "listParticleRules";
            this.listParticleRules.Size = new System.Drawing.Size(534, 263);
            this.listParticleRules.TabIndex = 0;
            this.listParticleRules.UseCompatibleStateImageBehavior = false;
            this.listParticleRules.View = System.Windows.Forms.View.Details;
            this.listParticleRules.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.DisplayRuleCRUD);
            // 
            // listAirParticles
            // 
            this.listAirParticles.Location = new System.Drawing.Point(12, 42);
            this.listAirParticles.Name = "listAirParticles";
            this.listAirParticles.Size = new System.Drawing.Size(198, 292);
            this.listAirParticles.TabIndex = 3;
            this.listAirParticles.UseCompatibleStateImageBehavior = false;
            this.listAirParticles.SelectedIndexChanged += new System.EventHandler(this.DisplayRules);
            // 
            // panelCRUD_Rules
            // 
            this.panelCRUD_Rules.BackColor = System.Drawing.Color.Lavender;
            this.panelCRUD_Rules.Controls.Add(this.txtNumBetween2);
            this.panelCRUD_Rules.Controls.Add(this.txtNumToCompare);
            this.panelCRUD_Rules.Controls.Add(this.label5);
            this.panelCRUD_Rules.Controls.Add(this.label4);
            this.panelCRUD_Rules.Controls.Add(this.lbParticle);
            this.panelCRUD_Rules.Controls.Add(this.btnRevoke);
            this.panelCRUD_Rules.Controls.Add(this.btnApply);
            this.panelCRUD_Rules.Controls.Add(this.btnDelete);
            this.panelCRUD_Rules.Controls.Add(this.txtAlertMessage);
            this.panelCRUD_Rules.Controls.Add(this.label2);
            this.panelCRUD_Rules.Controls.Add(this.label1);
            this.panelCRUD_Rules.Controls.Add(this.cbConditions);
            this.panelCRUD_Rules.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelCRUD_Rules.Location = new System.Drawing.Point(0, 351);
            this.panelCRUD_Rules.Name = "panelCRUD_Rules";
            this.panelCRUD_Rules.Size = new System.Drawing.Size(786, 260);
            this.panelCRUD_Rules.TabIndex = 4;
            // 
            // txtNumBetween2
            // 
            this.txtNumBetween2.Enabled = false;
            this.txtNumBetween2.Location = new System.Drawing.Point(465, 81);
            this.txtNumBetween2.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtNumBetween2.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.txtNumBetween2.Name = "txtNumBetween2";
            this.txtNumBetween2.Size = new System.Drawing.Size(85, 22);
            this.txtNumBetween2.TabIndex = 15;
            this.txtNumBetween2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtNumBetween2.Visible = false;
            // 
            // txtNumToCompare
            // 
            this.txtNumToCompare.DecimalPlaces = 2;
            this.txtNumToCompare.Enabled = false;
            this.txtNumToCompare.Location = new System.Drawing.Point(366, 80);
            this.txtNumToCompare.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtNumToCompare.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.txtNumToCompare.Name = "txtNumToCompare";
            this.txtNumToCompare.Size = new System.Drawing.Size(83, 22);
            this.txtNumToCompare.TabIndex = 14;
            this.txtNumToCompare.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(556, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(28, 39);
            this.label5.TabIndex = 13;
            this.label5.Text = ")";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(146, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 39);
            this.label4.TabIndex = 12;
            this.label4.Text = "(";
            // 
            // lbParticle
            // 
            this.lbParticle.AutoSize = true;
            this.lbParticle.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbParticle.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbParticle.Location = new System.Drawing.Point(285, 20);
            this.lbParticle.Name = "lbParticle";
            this.lbParticle.Size = new System.Drawing.Size(142, 42);
            this.lbParticle.TabIndex = 11;
            this.lbParticle.Text = "Particle";
            // 
            // btnRevoke
            // 
            this.btnRevoke.Enabled = false;
            this.btnRevoke.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Strikeout, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRevoke.Location = new System.Drawing.Point(607, 22);
            this.btnRevoke.Name = "btnRevoke";
            this.btnRevoke.Size = new System.Drawing.Size(167, 55);
            this.btnRevoke.TabIndex = 11;
            this.btnRevoke.Text = "Revoke Rule";
            this.btnRevoke.UseVisualStyleBackColor = true;
            this.btnRevoke.Click += new System.EventHandler(this.btnRevoke_Click);
            // 
            // btnApply
            // 
            this.btnApply.Enabled = false;
            this.btnApply.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnApply.ForeColor = System.Drawing.Color.ForestGreen;
            this.btnApply.Location = new System.Drawing.Point(607, 172);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(167, 53);
            this.btnApply.TabIndex = 9;
            this.btnApply.Text = "Apply Rule";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.ForeColor = System.Drawing.Color.Brown;
            this.btnDelete.Location = new System.Drawing.Point(607, 99);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(167, 54);
            this.btnDelete.TabIndex = 8;
            this.btnDelete.Text = "Delete Rule";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // txtAlertMessage
            // 
            this.txtAlertMessage.Enabled = false;
            this.txtAlertMessage.Location = new System.Drawing.Point(194, 161);
            this.txtAlertMessage.Multiline = true;
            this.txtAlertMessage.Name = "txtAlertMessage";
            this.txtAlertMessage.Size = new System.Drawing.Size(356, 64);
            this.txtAlertMessage.TabIndex = 5;
            this.txtAlertMessage.Text = "Alert Message";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(70, 161);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 39);
            this.label2.TabIndex = 2;
            this.label2.Text = "Then";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(80, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 39);
            this.label1.TabIndex = 1;
            this.label1.Text = "If";
            // 
            // cbConditions
            // 
            this.cbConditions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbConditions.Enabled = false;
            this.cbConditions.FormattingEnabled = true;
            this.cbConditions.Location = new System.Drawing.Point(180, 80);
            this.cbConditions.Name = "cbConditions";
            this.cbConditions.Size = new System.Drawing.Size(165, 24);
            this.cbConditions.TabIndex = 0;
            this.cbConditions.SelectedIndexChanged += new System.EventHandler(this.DisplaySecondNumTxt);
            // 
            // btnStopServ
            // 
            this.btnStopServ.Location = new System.Drawing.Point(13, 3);
            this.btnStopServ.Name = "btnStopServ";
            this.btnStopServ.Size = new System.Drawing.Size(109, 33);
            this.btnStopServ.TabIndex = 5;
            this.btnStopServ.UseVisualStyleBackColor = true;
            this.btnStopServ.Click += new System.EventHandler(this.StartStopService);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(475, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 17);
            this.label3.TabIndex = 7;
            this.label3.Text = "XML FILE:";
            // 
            // txtFilePath
            // 
            this.txtFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilePath.Location = new System.Drawing.Point(559, 8);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(212, 22);
            this.txtFilePath.TabIndex = 8;
            // 
            // btnNewRule
            // 
            this.btnNewRule.BackColor = System.Drawing.Color.LimeGreen;
            this.btnNewRule.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewRule.Location = new System.Drawing.Point(234, 32);
            this.btnNewRule.Name = "btnNewRule";
            this.btnNewRule.Size = new System.Drawing.Size(108, 30);
            this.btnNewRule.TabIndex = 9;
            this.btnNewRule.Text = "New Rule";
            this.btnNewRule.UseVisualStyleBackColor = false;
            this.btnNewRule.Click += new System.EventHandler(this.CreateRule);
            // 
            // txtXsdPath
            // 
            this.txtXsdPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtXsdPath.Location = new System.Drawing.Point(559, 37);
            this.txtXsdPath.Name = "txtXsdPath";
            this.txtXsdPath.Size = new System.Drawing.Size(212, 22);
            this.txtXsdPath.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(475, 37);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 17);
            this.label6.TabIndex = 10;
            this.label6.Text = "XSD FILE:";
            // 
            // FileStatusIcon
            // 
            this.FileStatusIcon.Location = new System.Drawing.Point(409, 4);
            this.FileStatusIcon.Name = "FileStatusIcon";
            this.FileStatusIcon.Size = new System.Drawing.Size(60, 50);
            this.FileStatusIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.FileStatusIcon.TabIndex = 12;
            this.FileStatusIcon.TabStop = false;
            this.FileStatusIcon.Tag = "";
            // 
            // airMonit_alarm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ClientSize = new System.Drawing.Size(786, 611);
            this.Controls.Add(this.FileStatusIcon);
            this.Controls.Add(this.txtXsdPath);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnNewRule);
            this.Controls.Add(this.txtFilePath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnStopServ);
            this.Controls.Add(this.panelCRUD_Rules);
            this.Controls.Add(this.listAirParticles);
            this.Controls.Add(this.panelRulesInfo);
            this.Name = "airMonit_alarm";
            this.Text = "AirMonit Alarm";
            this.panelRulesInfo.ResumeLayout(false);
            this.panelCRUD_Rules.ResumeLayout(false);
            this.panelCRUD_Rules.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumBetween2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumToCompare)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FileStatusIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listParticleRules;
        private System.Windows.Forms.Panel panelCRUD_Rules;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbConditions;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.TextBox txtAlertMessage;
        private System.Windows.Forms.Button btnRevoke;
        private System.Windows.Forms.Label lbParticle;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown txtNumToCompare;
        private System.Windows.Forms.Button btnStopServ;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.NumericUpDown txtNumBetween2;
        private System.Windows.Forms.Button btnNewRule;
        private System.Windows.Forms.TextBox txtXsdPath;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox FileStatusIcon;
    }
}

