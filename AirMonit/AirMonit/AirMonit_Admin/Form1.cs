using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AirMonit_Admin
{
    public partial class Form1 : Form
    {

        private static string URL = "http://localhost:50077/";
        private static string CITIES_ENDPOINT = "api/cities";

        public Form1()
        {
            InitializeComponent();
            dateTimePickerInicio.Value = DateTime.Today.AddDays(-7);
            dateTimePickerFim.Value = DateTime.Today;

            dateTimePickerInicio.MaxDate = dateTimePickerFim.Value;
            dateTimePickerFim.MinDate = dateTimePickerInicio.Value;

            string cities_uri = URL + CITIES_ENDPOINT;

            /*
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(cities_uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            response.GetResponseStream();
            */

            listViewAlarme.Columns.Add("Data");
            listViewAlarme.Columns.Add("Cidade");
            listViewAlarme.Columns.Add("Partícula");
            listViewAlarme.Columns.Add("Valor");
            listViewAlarme.Columns.Add("Mensagem");
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePickerFim.MinDate = dateTimePickerInicio.Value;
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            dateTimePickerInicio.MaxDate = dateTimePickerFim.Value;
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
