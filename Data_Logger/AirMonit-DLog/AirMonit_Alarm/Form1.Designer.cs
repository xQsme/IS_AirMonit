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

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.listRuleCRUD = new System.Windows.Forms.ListView();
            this.listAirParticles = new System.Windows.Forms.ListView();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel1.Controls.Add(this.listRuleCRUD);
            this.panel1.Location = new System.Drawing.Point(234, 26);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(505, 292);
            this.panel1.TabIndex = 1;
            // 
            // listRuleCRUD
            // 
            this.listRuleCRUD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listRuleCRUD.Location = new System.Drawing.Point(0, 0);
            this.listRuleCRUD.Name = "listRuleCRUD";
            this.listRuleCRUD.Size = new System.Drawing.Size(505, 292);
            this.listRuleCRUD.TabIndex = 0;
            this.listRuleCRUD.UseCompatibleStateImageBehavior = false;
            // 
            // listAirParticles
            // 
            this.listAirParticles.Location = new System.Drawing.Point(13, 26);
            this.listAirParticles.Name = "listAirParticles";
            this.listAirParticles.Size = new System.Drawing.Size(198, 292);
            this.listAirParticles.TabIndex = 3;
            this.listAirParticles.UseCompatibleStateImageBehavior = false;
            // 
            // airMonit_alarm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(751, 455);
            this.Controls.Add(this.listAirParticles);
            this.Controls.Add(this.panel1);
            this.Name = "airMonit_alarm";
            this.Text = "AirMonit Alarm";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListView listRuleCRUD;
        private System.Windows.Forms.ListView listAirParticles;
    }
}

