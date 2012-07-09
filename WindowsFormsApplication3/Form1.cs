using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;      //for plotting
using MathNet.Numerics.Statistics;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        private int i = 0;
        private DateTime dtStarttime;
        private TimeSpan tsTimespent;

        private List<double> listdTimespent = new List<double>();
        private List<double> listdTemperature = new List<double>();
        private List<double> listdHumidity = new List<double>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Start_Click(object sender, EventArgs e)
        {
            dtStarttime = DateTime.Now;

            serialPort1.PortName = "COM3";
            serialPort1.BaudRate = 9600;
            serialPort1.DtrEnable = true;
            serialPort1.Open();

            serialPort1.DataReceived += serialPort1_DataReceived;
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            string line = serialPort1.ReadLine();
            this.BeginInvoke(new LineReceivedEvent(LineReceived), line);
        }

        private delegate void LineReceivedEvent(string line);

        private void LineReceived(string line)
        {
            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            //sw.Start();

            double dTemperature, dTemperatureRound, dHumidity;
            //What to do with the received line here
            label1.Text = line;

            try
            {
                DateTime dtStart = DateTime.Now;
                Temperature.Text = line.Substring(13, 13);
                Humidity.Text = line.Substring(38, 5);

                dTemperature = double.Parse(line.Substring(13, 13), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);  //Arduino sends text as 123.335 we need to change it to 123,335 to succesfully Parse it to double
                dHumidity = double.Parse(line.Substring(38, 5), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);

                dTemperatureRound = Math.Round(dTemperature, 3);

                //////////////////////////////TEMPERATURE

                chart1.Series[0].BorderWidth = 2;
                chart1.Series[0].ChartType = SeriesChartType.Line;

                //chart1.Titles.Clear();
                //chart1.Titles.Add("TMP102 Temperatur Sensor");
                //chart1.Titles[0].Font = new Font("Arial", 12f);
                //chart1.ChartAreas[0].AxisX.Title = "Zeit [s]";
                //chart1.ChartAreas[0].AxisY.Title = "Temperatur [°C]";
                //chart1.ChartAreas[0].AxisX.TitleFont = new Font("Arial", 12f);
                //chart1.ChartAreas[0].AxisY.TitleFont = new Font("Arial", 12f);

                //chart1.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Arial", 18);
                //this.chart1.PrimaryXAxis.Font = new Font("Verdana", 8f, FontStyle.Bold);

                tsTimespent = DateTime.Now - dtStarttime;
                listdTimespent.Add(i);
                listdTemperature.Add(dTemperatureRound);

                chart1.Series[0].Points.AddXY(tsTimespent.TotalSeconds, dTemperatureRound);
                chart1.ChartAreas["ChartArea1"].AxisX.Minimum = listdTimespent[0];
                chart1.ChartAreas["ChartArea1"].AxisY.Minimum = (listdTemperature.Min() - 0.5);
                chart1.ChartAreas["ChartArea1"].AxisY.Maximum = (listdTemperature.Max() + 0.5);
                //chart1.ChartAreas["ChartArea1"].AxisX = new Axis { LabelStyle = new LabelStyle() { Font = new Font("Verdana", 7.5f) } };
                //chart1.ChartAreas["ChartArea1"].AxisX.TitleFont = "Verdana";

                //chart1.ChartAreas["ChartArea1"].AxisX.Title = "Zeit [s]";
                //chart1.ChartAreas["ChartArea1"].AxisY.Title = "Temparature [C]";

                DateTime time = DateTime.Now;
                //string format = "MMM ddd d HH:mm yyyy";
                string format = "yyyyMMddHHmmssffff";

                label2.Text = time.ToString(format);

                i++;

                // DateTime dtStop = DateTime.Now;
                // DateTime dtDifference;

                // dtStop.Subtract(dtStop);

                /////////////////////////////////HUMIDTY

                chart2.Series[0].BorderWidth = 2;
                chart2.Series[0].ChartType = SeriesChartType.Line;
                chart2.Series[0].Points.AddXY(tsTimespent.TotalSeconds, dHumidity);
                listdHumidity.Add(dHumidity);
                chart2.ChartAreas["ChartArea1"].AxisX.Minimum = listdTimespent[0];
                chart2.ChartAreas["ChartArea1"].AxisY.Minimum = (listdHumidity.Min() - 0.5);
                chart2.ChartAreas["ChartArea1"].AxisY.Maximum = (listdHumidity.Max() + 0.5);
                listdHumidity.StandardDeviation();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            //sw.Stop();

            //TimeSpan timespent = DateTime.Now - starttime;

            //label3.Text = tsTimespent.TotalSeconds.ToString();
            label3.Text = Convert.ToString(listdTemperature.Average());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
        }
    }
}