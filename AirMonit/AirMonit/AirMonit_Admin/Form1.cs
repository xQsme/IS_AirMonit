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
        private static string PARTICLES_ENDPOINT = "/api/particles/{0}/summarize/city/{1}/day/{2}/hour";
        private static string ALL_INCIDENTS_ENDPOINT = "/api/cities/incidents";

        private List<string> cityList = new List<string>();
        private List<Color> colorList = new List<Color>();

        public Form1()
        {
            InitializeComponent();
            dateTimePickerInicio.Value = DateTime.Today.AddDays(-7);
            dateTimePickerFim.Value = DateTime.Today;

            listViewAlarme.Columns.Add("Data");
            listViewAlarme.Columns.Add("Cidade");
            listViewAlarme.Columns.Add("Partícula");
            listViewAlarme.Columns.Add("Valor");
            listViewAlarme.Columns.Add("Mensagem");

            listViewEventos.Columns.Add("Evento", -2, HorizontalAlignment.Left);
            listViewEventos.Columns.Add("Publisher", -2, HorizontalAlignment.Left);
            listViewEventos.Columns.Add("Data", -2, HorizontalAlignment.Left);

            colorList.Add(System.Drawing.Color.Crimson);
            colorList.Add(System.Drawing.Color.RoyalBlue);
            colorList.Add(System.Drawing.Color.MediumSpringGreen);
            colorList.Add(System.Drawing.Color.Violet);
            colorList.Add(System.Drawing.Color.LightSeaGreen);
            colorList.Add(System.Drawing.Color.Aquamarine);

            dateTimePickerInicio.MaxDate = dateTimePickerFim.Value;
            dateTimePickerFim.MinDate = dateTimePickerInicio.Value;

            populateParticleComboBox();
            getCities();

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;

            getParticles();

            GetIncidents();
        }

        public void populateParticleComboBox()
        {
            ComboboxItem item = new ComboboxItem();
            item.Text = "CO";
            comboBox2.Items.Add(item);

            item = new ComboboxItem();
            item.Text = "NO2";
            comboBox2.Items.Add(item);

            item = new ComboboxItem();
            item.Text = "O3";
            comboBox2.Items.Add(item);
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
            /*
            item.Text = "Todas as cidades";
            item.Value = 0;
            comboBox1.Items.Add(item);
            */

            foreach (City city in lista)
            {
                item = new ComboboxItem();
                item.Text = city.Name.Trim();
                item.Value = city.ID;

                comboBox1.Items.Add(item);
                cityList.Add(city.Name);
            }

            item = new ComboboxItem();
            item.Text = "Todas as cidades";
            item.Value = 0;
            comboBox1.Items.Add(item);

        }

        public void getParticles()
        {

            string selectedDate = dateTimePicker1.Value.ToString("dd-MM-yyyy");
            string selectedCity = comboBox1.Text.Trim();
            string selectedParticle = comboBox2.Text.Trim();
            if (!selectedCity.Equals("") && !selectedParticle.Equals(""))
            {
                chart1.Series.Clear();
                if (selectedCity.Equals("Todas as cidades"))
                {
                    DesenharTodasAsCidades(selectedDate, selectedParticle);
                }
                else
                {
                    DesenharSingleCity(selectedDate, selectedCity, selectedParticle);
                }
                chart1.Refresh();
                chart1.ChartAreas[0].RecalculateAxesScale();
                //chart1.Invalidate();
            }


        }

        public void DesenharTodasAsCidades(string selectedDate,string selectedParticle)
        {
            int i = 0;
            foreach (string city in cityList)
            {
                string particlesUri = BASE_URL + string.Format(PARTICLES_ENDPOINT, selectedParticle, city, selectedDate);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(particlesUri);
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

                List<SummarizeEntries> lista = new List<SummarizeEntries>();

                JavaScriptSerializer jsonObject = new JavaScriptSerializer();
                lista = jsonObject.Deserialize<List<SummarizeEntries>>(content);

                chart1.ChartAreas[0].AxisX.IsMarginVisible = false;

                Series serieMedia = new System.Windows.Forms.DataVisualization.Charting.Series
                {
                    Name = "Média " + city,
                    Color = colorList[i++],
                    IsVisibleInLegend = true,
                    IsXValueIndexed = false,
                    ChartType = SeriesChartType.Line,
                    MarkerStyle = MarkerStyle.Square,
                    MarkerSize = 10
                };
                chart1.Series.Add(serieMedia);
                foreach (SummarizeEntries entry in lista)
                {
                    int hora = entry.Date.Hour;
                    serieMedia.Points.AddXY(hora, System.Convert.ToDouble(entry.Average));
                }
            }
        }

        public void DesenharSingleCity(string selectedDate,string selectedCity,string selectedParticle)
        {
            string particlesUri = BASE_URL + string.Format(PARTICLES_ENDPOINT, selectedParticle, selectedCity, selectedDate);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(particlesUri);
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

            List<SummarizeEntries> lista = new List<SummarizeEntries>();

            JavaScriptSerializer jsonObject = new JavaScriptSerializer();
            lista = jsonObject.Deserialize<List<SummarizeEntries>>(content);

            chart1.ChartAreas[0].AxisX.IsMarginVisible = false;

            Series serieMedia = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "Média " + selectedCity,
                Color = colorList[0],
                IsVisibleInLegend = true,
                IsXValueIndexed = false,
                ChartType = SeriesChartType.Line,
                MarkerStyle = MarkerStyle.Square,
                MarkerSize = 10
            };
            Series serieMaximo = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "Máximo " + selectedCity,
                Color = colorList[1],
                IsVisibleInLegend = true,
                IsXValueIndexed = false,
                ChartType = SeriesChartType.Line,
                MarkerStyle = MarkerStyle.Square,
                MarkerSize = 10
            };
            Series serieMinimo = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "Minimo " + selectedCity,
                Color = colorList[2],
                IsVisibleInLegend = true,
                IsXValueIndexed = false,
                ChartType = SeriesChartType.Line,
                MarkerStyle = MarkerStyle.Square,
                MarkerSize = 10
            };

            chart1.Series.Add(serieMaximo);
            chart1.Series.Add(serieMedia);
            chart1.Series.Add(serieMinimo);
            foreach (SummarizeEntries entry in lista)
            {
                int hora = entry.Date.Hour;
                serieMedia.Points.AddXY(hora, System.Convert.ToDouble(entry.Average));
                serieMaximo.Points.AddXY(hora, System.Convert.ToDouble(entry.Max));
                serieMinimo.Points.AddXY(hora, System.Convert.ToDouble(entry.Min));
            }
        }

        public void GetIncidents()
        {
                //string incidents_uri = BASE_URL + string.Format(INCIDENTS_ENDPOINT, selectedCity);

            string incidentsUri = BASE_URL + ALL_INCIDENTS_ENDPOINT;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(incidentsUri);
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

            List<IncidentEntry> lista = new List<IncidentEntry>();

            JavaScriptSerializer jsonObject = new JavaScriptSerializer();
            lista = jsonObject.Deserialize<List<IncidentEntry>>(content);

            listViewEventos.Items.Clear();

            foreach (IncidentEntry entry in lista)
            {
                string[] arr = new string[4];
                ListViewItem itm;
                //add items to ListView 
                arr[0] = entry.Event.Trim();
                arr[1] = entry.Publisher.Trim();
                arr[2] = entry.Date.ToShortDateString().Trim();
                itm = new ListViewItem(arr);
                listViewEventos.Items.Add(itm);
            }

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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            getParticles();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
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
