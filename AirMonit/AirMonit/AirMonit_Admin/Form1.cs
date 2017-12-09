using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using IAirEntries;
using IAirEntries.Models;

namespace AirMonit_Admin
{
    public partial class Form1 : Form
    {

        private static string BASE_URL = "http://localhost:50077";
        private static string CITIES_ENDPOINT = "/api/cities";
        //                                           api/particles/CO/days/27-11-2017_30-11-2017
        private static string PARTICLES_ENDPOINT = "/api/particles/{0}/days/{1}_{2}";

        public Form1()
        {
            InitializeComponent();
            dateTimePickerInicio.Value = DateTime.Today.AddDays(-7);
            dateTimePickerFim.Value = DateTime.Today;

            dateTimePickerInicio.MaxDate = dateTimePickerFim.Value;
            dateTimePickerFim.MinDate = dateTimePickerInicio.Value;

            getCities();
            getParticles();

            listViewAlarme.Columns.Add("Data");
            listViewAlarme.Columns.Add("Cidade");
            listViewAlarme.Columns.Add("Partícula");
            listViewAlarme.Columns.Add("Valor");
            listViewAlarme.Columns.Add("Mensagem");
        }

        public void getCities()
        {
            string cities_uri = BASE_URL + CITIES_ENDPOINT;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(cities_uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            //Stream e o StreamReader
            string content = String.Empty;
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    content = sr.ReadToEnd();
                }
            }

            List<City> lista = new List<City>();

            JavaScriptSerializer jsonObject = new JavaScriptSerializer();
            lista = jsonObject.Deserialize<List<City>>(content);

            ComboboxItem item = new ComboboxItem();
            item.Text = "Todas as cidades";
            item.Value = 0;
            comboBox1.Items.Add(item);

            foreach (City city in lista)
            {
                item = new ComboboxItem();
                item.Text = city.Name;
                item.Value = city.ID;

                comboBox1.Items.Add(item);
            }
        

        }

        public void getParticles()
        {
            string selectedDate = dateTimePicker1.Value.ToString("dd-MM-yyyy");
            string particles_uri = BASE_URL + string.Format(PARTICLES_ENDPOINT, "CO", selectedDate, selectedDate);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(particles_uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            //Stream e o StreamReader
            string content = String.Empty;
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    content = sr.ReadToEnd();
                }
            }

            List<ParticleEntry> lista = new List<ParticleEntry>();

            JavaScriptSerializer jsonObject = new JavaScriptSerializer();
            lista = jsonObject.Deserialize<List<ParticleEntry>>(content);

            Series serie = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "Coimbra",
                Color = System.Drawing.Color.Green,
                IsVisibleInLegend = false,
                IsXValueIndexed = true,
                ChartType = SeriesChartType.Line
            };

            this.chart1.Series.Add(serie);

            foreach (ParticleEntry entry in lista)
            {
                serie.Points.AddXY(entry.Date.Hour, System.Convert.ToDouble(entry.Value));
            }
            chart1.Invalidate();

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

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged_1(object sender, EventArgs e)
        {
            getParticles();
        }
    }
    public class ComboboxItem
    {
        public string Text { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
