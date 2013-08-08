using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;
using System.IO;
using SharpPcap;
using PacketDotNet;
//using SharpPcap.WinPcap;
//using MetroFramework.Forms;
namespace rtChartPlotter
{

    public partial class Form1 : Form
    {
        public double timeDelay = 0;

        TextWriter textboxStream = null;
        private Random random1 = new Random();
        private Random random2 = new Random();
        private int pointIndex = 0;
        private int deviceNumber = 0;
        private int readTimeoutMilliseconds = 1000;


        CaptureDeviceList devices = null;

        static ICaptureDevice device0s, device0r;
        static ICaptureDevice device1s, device1r;
        static ICaptureDevice device2s, device2r;
        static ICaptureDevice device3s, device3r;

        static long[] devicesPacketLengthS = new long[4];
        static long[] devicesPacketLengthR = new long[4];

        //private bool isDeviceOpen = false;
        //private static int sumPacketLength = 0;

        private IEnumerable<string[]> lines;
        private IEnumerable<string[][]> csv;

        private double chartArea1MinYAxis = 100;
        //private double chartArea2MinYAxis = 100;
        //private double chartArea3MinYAxis = 100;
        public Form1()
        {
            InitializeComponent();
        }
        private void menuStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
        private static void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            DateTime time = e.Packet.Timeval.Date;
            int len = e.Packet.Data.Length;
            devicesPacketLengthS[0] += len;
            Console.WriteLine("bytes : {0}", len);
            Console.WriteLine("{0}:{1}:{2},{3} Len={4}",
    time.Hour, time.Minute, time.Second, time.Millisecond, len);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            timeDelay = timer1.Interval;

            lines = File.ReadAllLines(@"c:\dinjter.csv").Select(a => a.Split(','));
            csv = from line in lines select (from piece in line select piece.Split(',')).ToArray();

            textboxStream = new TextBoxStreamWriter(textBox1);
            Console.SetOut(textboxStream);

            // Volt meter stub.
            textBox10.Text = "0.00 V";

            // Retrieve the device list
            devices = CaptureDeviceList.New();

            // If no devices were found print an error
            if (devices.Count < 1)
            {
                Console.WriteLine("No devices were found on this machine");
                return;
            }
            //set limit on NumericUpDown box
            this.numericUpDown1.Maximum = devices.Count - 1;

            Console.WriteLine("The following devices are available on this machine:");
            Console.WriteLine("----------------------------------------------------");

            // Print out the available network devices
            foreach (ICaptureDevice dev in devices)
                Console.WriteLine("{0}\n", dev.ToString());

            device0s = devices[0];
            device0r = devices[0];
            device1s = devices[0];
            device1r = devices[0];
            device2s = devices[0];
            device2r = devices[0];
            device3s = devices[0];
            device3r = devices[0];
            
            device0s.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);
            device1s.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);
            device1r.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);
            device2s.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);
            device2r.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);
            device3s.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);
            device3r.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);

            device0s.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            device1s.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            device1r.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            device2s.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            device2r.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            device3s.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            device3r.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

            device0s.StartCapture();
            //device1s.StartCapture();
            //device1r.StartCapture();
            //device2s.StartCapture();
            //device2r.StartCapture();
            //device3s.StartCapture();
            //device3r.StartCapture();

            Console.WriteLine("device started");
        }
        private void stubToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Define some variables
            int numberOfPointsInChart = 100;
            int numberOfPointsAfterRemoval = 100;

            // adding new data points
            int numberOfPointsAddedMin = 1;
            int numberOfPointsAddedMax = 1;
            for (int pointNumber = 0; pointNumber < random1.Next(numberOfPointsAddedMin, numberOfPointsAddedMax); pointNumber++)
            {
                //var randSeries1 = random1.Next(26, 30);
                chart1.Series[0].Points.AddXY(DateTime.Now.ToLongTimeString(), csv.ElementAt(pointIndex + 1)[0][0]);
                textBox2.Text = csv.ElementAt(pointIndex + 1)[0][0] + "°C";
                chart1.Series[1].Points.AddXY(DateTime.Now.ToLongTimeString(), csv.ElementAt(pointIndex + 1)[1][0]);
                textBox3.Text = csv.ElementAt(pointIndex + 1)[1][0] + "°C";
                chart1.Series[2].Points.AddXY(DateTime.Now.ToLongTimeString(), csv.ElementAt(pointIndex + 1)[2][0]);
                textBox4.Text = csv.ElementAt(pointIndex + 1)[2][0] + "°C";
                chart1.Series[3].Points.AddXY(DateTime.Now.ToLongTimeString(), csv.ElementAt(pointIndex + 1)[3][0]);
                textBox5.Text = csv.ElementAt(pointIndex + 1)[3][0] + "°C";
                chart1.Series[4].Points.AddXY(DateTime.Now.ToLongTimeString(), csv.ElementAt(pointIndex + 1)[4][0]);
                //textBox6.Text = csv.ElementAt(pointIndex + 1)[4][0];
                chart1.Series[5].Points.AddXY(DateTime.Now.ToLongTimeString(), csv.ElementAt(pointIndex + 1)[5][0]);
                //textBox7.Text = csv.ElementAt(pointIndex + 1)[5][0];
                chart1.Series[6].Points.AddXY(DateTime.Now.ToLongTimeString(), csv.ElementAt(pointIndex + 1)[6][0]);
                textBox8.Text = csv.ElementAt(pointIndex + 1)[6][0] + "km/h";
                ++pointIndex;
            }

            // Adjust Y & X axis scale
            chart1.ResetAutoValues();
            chart1.ChartAreas["ChartArea1"].AxisX.ScaleView.Zoom(0, 100);
            chart1.ChartAreas["ChartArea2"].AxisX.ScaleView.Zoom(0, 100);
            chart1.ChartAreas["ChartArea3"].AxisX.ScaleView.Zoom(0, 100);
            if (pointIndex >= 100)
            {
                chart1.ChartAreas["ChartArea1"].AxisX.ScaleView.ZoomReset();
                chart1.ChartAreas["ChartArea2"].AxisX.ScaleView.ZoomReset();
                chart1.ChartAreas["ChartArea3"].AxisX.ScaleView.ZoomReset();
            }
            if (pointIndex > 0)
            {
                if (Math.Min(Math.Min(double.Parse(csv.ElementAt(pointIndex)[0][0]), double.Parse(csv.ElementAt(pointIndex)[1][0]))
                    , double.Parse(csv.ElementAt(pointIndex)[2][0])) < chartArea1MinYAxis)
                {
                    chartArea1MinYAxis = Math.Min(Math.Min(double.Parse(csv.ElementAt(pointIndex)[0][0]), double.Parse(csv.ElementAt(pointIndex)[1][0]))
                    , double.Parse(csv.ElementAt(pointIndex)[2][0]));
                }

                chart1.ChartAreas["ChartArea1"].AxisY.Minimum = chartArea1MinYAxis - 0.5;

            }
            else
                chartArea1MinYAxis = Math.Min(Math.Min(double.Parse(csv.ElementAt(1)[0][0]), double.Parse(csv.ElementAt(1)[1][0]))
                    , double.Parse(csv.ElementAt(1)[2][0]));
            //chart1.ResetAutoValues();

            // Keep a constant number of points by removing them from the left
            /*while (chart1.Series[0].Points.Count > numberOfPointsInChart)
            {
                // Remove data points on the left side
                while (chart1.Series[0].Points.Count > numberOfPointsAfterRemoval)
                {
                    chart1.Series[0].Points.RemoveAt(0);
                    //chart1.Series[1].Points.RemoveAt(0);
                    //chart1.Series[2].Points.RemoveAt(0);
                }

                // Adjust X axis scale
                chart1.ChartAreas["ChartArea1"].AxisX.Minimum = pointIndex - numberOfPointsAfterRemoval;
                chart1.ChartAreas["ChartArea1"].AxisX.Maximum = chart1.ChartAreas["ChartArea1"].AxisX.Minimum + numberOfPointsInChart;
            }*/
            if (pointIndex >= 100)
            {
                chart1.ChartAreas["ChartArea1"].AxisX.Minimum = pointIndex - numberOfPointsAfterRemoval;
                chart1.ChartAreas["ChartArea1"].AxisX.Maximum = chart1.ChartAreas["ChartArea1"].AxisX.Minimum + numberOfPointsInChart;
                chart1.ChartAreas["ChartArea2"].AxisX.Minimum = pointIndex - numberOfPointsAfterRemoval;
                chart1.ChartAreas["ChartArea2"].AxisX.Maximum = chart1.ChartAreas["ChartArea1"].AxisX.Minimum + numberOfPointsInChart;
                chart1.ChartAreas["ChartArea3"].AxisX.Minimum = pointIndex - numberOfPointsAfterRemoval;
                chart1.ChartAreas["ChartArea3"].AxisX.Maximum = chart1.ChartAreas["ChartArea1"].AxisX.Minimum + numberOfPointsInChart;
            }
            else
            {
                chart1.ChartAreas["ChartArea1"].AxisX.Minimum = 0;
                chart1.ChartAreas["ChartArea1"].AxisX.Maximum = numberOfPointsInChart;
                chart1.ChartAreas["ChartArea2"].AxisX.Minimum = 0;
                chart1.ChartAreas["ChartArea2"].AxisX.Maximum = numberOfPointsInChart;
                chart1.ChartAreas["ChartArea3"].AxisX.Minimum = 0;
                chart1.ChartAreas["ChartArea3"].AxisX.Maximum = numberOfPointsInChart;
            }
            /*while (chart1.Series[3].Points.Count > numberOfPointsInChart)
            {
                // Remove data points on the left side
                while (chart1.Series[3].Points.Count > numberOfPointsAfterRemoval)
                {
                    chart1.Series[3].Points.RemoveAt(0);
                }

                // Adjust X axis scale
                chart1.ChartAreas["ChartArea2"].AxisX.Minimum = pointIndex - numberOfPointsAfterRemoval;
                chart1.ChartAreas["ChartArea2"].AxisX.Maximum = chart1.ChartAreas["ChartArea2"].AxisX.Minimum + numberOfPointsInChart;
            }*/
            // Invalidate chart
            chart1.Invalidate();

            // chart2 : Network statistics

            chart2.Series[0].Points.AddXY(DateTime.Now.ToLongTimeString(), devicesPacketLengthS[0] / ((timeDelay / 1000) * 1024));
            textBox12.Text = Math.Round((devicesPacketLengthS[0] / ((timeDelay/1000) * 1024)),2).ToString() + " kB/s";
            /*chart2.Series[1].Points.AddXY(DateTime.Now.ToLongTimeString(), devicesPacketLengthR[0]);
            chart2.Series[2].Points.AddXY(DateTime.Now.ToLongTimeString(), devicesPacketLengthS[1]);
            chart2.Series[3].Points.AddXY(DateTime.Now.ToLongTimeString(), devicesPacketLengthR[1]);
            chart2.Series[4].Points.AddXY(DateTime.Now.ToLongTimeString(), devicesPacketLengthS[2]);
            chart2.Series[5].Points.AddXY(DateTime.Now.ToLongTimeString(), devicesPacketLengthR[2]);
            chart2.Series[6].Points.AddXY(DateTime.Now.ToLongTimeString(), devicesPacketLengthS[3]);
            chart2.Series[7].Points.AddXY(DateTime.Now.ToLongTimeString(), devicesPacketLengthR[3]);
             */ 
            for (int i = 0; i < 4; i++)
            {
                devicesPacketLengthS[i] = 0;
                devicesPacketLengthR[i] = 0;
            }
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TextBoxUtils.ScrollToBottom(textBox1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            device0s.StopCapture();
            device0s.Close();

            //isDeviceOpen = false;

            device0s = devices[deviceNumber];
            device0s = devices[deviceNumber];

            Console.WriteLine("-- Listening on {0}", device0s.Description);

            // Register our handler function to the
            // 'packet arrival' event
            device0s.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);

            // Open the device for capturing
            device0s.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);


            device0s.StartCapture();

            //isDeviceOpen = true;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            device0s.StopCapture();
            device0s.Close();

            //isDeviceOpen = false;

            deviceNumber = (int)numericUpDown1.Value;

            device0s = devices[deviceNumber];

            Console.WriteLine("-- Set device to listening on {0}", device0s.Description);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Stopping device...");

            device0s.StopCapture();
            device0s.Close();

            //isDeviceOpen = false;

            Console.WriteLine("Device stopped");

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
