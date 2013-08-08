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
        TextWriter textboxStream = null;
        private Random random1 = new Random();
        private Random random2 = new Random();
        private int pointIndex = 0;
        private int deviceNumber = 2;
        private int readTimeoutMilliseconds = 1000;


        CaptureDeviceList devices = null;

        ICaptureDevice device;
        ICaptureDevice deviceMasterSent = null;
        ICaptureDevice deviceMasterRecv = null;
        static ICaptureDevice device1Sent = null;
        static ICaptureDevice device1Recv = null;
        static ICaptureDevice device2Sent = null;
        static ICaptureDevice device2Recv = null;
        static ICaptureDevice device3Sent = null;
        static ICaptureDevice device3Recv = null;

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
            Console.WriteLine("bytes : {0}", len);
            Console.WriteLine("{0}:{1}:{2},{3} Len={4}",
    time.Hour, time.Minute, time.Second, time.Millisecond, len);
            /*if (sender.Equals(deviceMasterSent))
                devicesPacketLengthS[0] += len;
            else if (sender.Equals(deviceMasterRecv))
                devicesPacketLengthR[0] += len;*/

            if (sender.Equals(device1Sent))
                devicesPacketLengthS[1] += len;
            else if (sender.Equals(device1Recv))
                devicesPacketLengthR[1] += len;

            else if (sender.Equals(device2Sent))
                devicesPacketLengthS[2] += len;
            else if (sender.Equals(device2Recv))
                devicesPacketLengthR[2] += len;

            else if (sender.Equals(device3Sent))
                devicesPacketLengthS[3] += len;
            else if (sender.Equals(device3Recv))
                devicesPacketLengthR[3] += len;
        }
        /*private static void device_OnPcapStatistics(object sender, SharpPcap.WinPcap.StatisticsModeEventArgs e)
        {
            Console.WriteLine("bytes : {0}", e.Statistics.RecievedBytes);
            /*if (sender.Equals(rtChartPlotter.Form1.deviceMasterSent))
                devicesPacketLengthS[0] += e.Statistics.RecievedBytes;
            else if (sender.Equals(deviceMasterRecv))
                devicesPacketLengthR[0] += e.Statistics.RecievedBytes;

            if (sender.Equals(device1Sent))
                devicesPacketLengthS[1] += e.Statistics.RecievedBytes;
            else if (sender.Equals(device1Recv))
                devicesPacketLengthR[1] += e.Statistics.RecievedBytes;

            else if (sender.Equals(device2Sent))
                devicesPacketLengthS[2] += e.Statistics.RecievedBytes;
            else if (sender.Equals(device2Recv))
                devicesPacketLengthR[2] += e.Statistics.RecievedBytes;

            else if (sender.Equals(device3Sent))
                devicesPacketLengthS[3] += e.Statistics.RecievedBytes;
            else if (sender.Equals(device3Recv))
                devicesPacketLengthR[3] += e.Statistics.RecievedBytes;
        }*/
        private void Form1_Load(object sender, EventArgs e)
        {
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
            this.numericUpDown1.Maximum = devices.Count-1;

            Console.WriteLine("The following devices are available on this machine:");
            Console.WriteLine("----------------------------------------------------");

            // Print out the available network devices
            foreach (ICaptureDevice dev in devices)
                Console.WriteLine("{0}\n", dev.ToString());

            device = devices[2];
            device.OnPacketArrival +=
    new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            device.StartCapture();
            Console.WriteLine("device started");
            // Assign selected device to each members
            
            deviceMasterSent = devices[deviceNumber];
            device1Sent = devices[deviceNumber];
            device2Sent = devices[deviceNumber];
            device3Sent = devices[deviceNumber];
            deviceMasterRecv = devices[deviceNumber];
            device1Recv = devices[deviceNumber];
            device2Recv = devices[deviceNumber];
            device3Recv = devices[deviceNumber];
            

            // Register our handler function to the
            // 'packet statistics' event
            deviceMasterSent.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);
            deviceMasterRecv.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);
            device1Sent.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);
            device1Recv.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);
            device2Sent.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);
            device2Recv.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);
            device3Sent.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);
            device3Recv.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);

            // Open the device for capturing
            deviceMasterSent.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            deviceMasterRecv.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            device1Sent.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            device1Recv.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            device2Sent.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            device2Recv.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            device3Sent.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            device3Recv.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);


            // Set device filter policy
            //deviceMasterSent.Filter = "src net 161.246.73.99";
            //deviceMasterRecv.Filter = "dst net 161.246.73.99";
            device1Sent.Filter = "src net 192.168.137.21";
            device1Recv.Filter = "dst net 192.168.137.21";
            device2Sent.Filter = "src net 192.168.137.22";
            device2Recv.Filter = "dst net 192.168.137.22";
            device3Sent.Filter = "src net 192.168.137.23";
            device3Recv.Filter = "dst net 192.168.137.23";

            Console.WriteLine("-- Listening on {0}, [Initial listening device]",
                deviceMasterSent.Description);

            // Start the capturing process
            deviceMasterSent.StartCapture();
            deviceMasterRecv.StartCapture();
            device1Sent.StartCapture();
            device1Recv.StartCapture();
            device2Sent.StartCapture();
            device2Recv.StartCapture();
            device3Sent.StartCapture();
            device3Recv.StartCapture();

            //isDeviceOpen = true;
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
                chart1.Series[0].Points.AddXY(DateTime.Now.ToLongTimeString(),csv.ElementAt(pointIndex + 1)[0][0]);
                textBox2.Text = csv.ElementAt(pointIndex + 1)[0][0];
                chart1.Series[1].Points.AddXY(DateTime.Now.ToLongTimeString(), csv.ElementAt(pointIndex + 1)[1][0]);
                textBox3.Text = csv.ElementAt(pointIndex + 1)[1][0];
                chart1.Series[2].Points.AddXY(DateTime.Now.ToLongTimeString(), csv.ElementAt(pointIndex + 1)[2][0]);
                textBox4.Text = csv.ElementAt(pointIndex + 1)[2][0];
                chart1.Series[3].Points.AddXY(DateTime.Now.ToLongTimeString(), csv.ElementAt(pointIndex + 1)[3][0]);
                textBox5.Text = csv.ElementAt(pointIndex + 1)[3][0];
                chart1.Series[4].Points.AddXY(DateTime.Now.ToLongTimeString(), csv.ElementAt(pointIndex + 1)[4][0]);
                textBox6.Text = csv.ElementAt(pointIndex + 1)[4][0];
                chart1.Series[5].Points.AddXY(DateTime.Now.ToLongTimeString(), csv.ElementAt(pointIndex + 1)[5][0]);
                textBox7.Text = csv.ElementAt(pointIndex + 1)[5][0];
                chart1.Series[6].Points.AddXY(DateTime.Now.ToLongTimeString(), csv.ElementAt(pointIndex + 1)[6][0]);
                textBox8.Text = csv.ElementAt(pointIndex + 1)[6][0];
                ++pointIndex;
            }

            // Adjust Y & X axis scale
            chart1.ResetAutoValues();
            chart1.ChartAreas["ChartArea1"].AxisX.ScaleView.Zoom(0, 100);
            chart1.ChartAreas["ChartArea2"].AxisX.ScaleView.Zoom(0, 100);
            if (pointIndex >= 100)
            {
                chart1.ChartAreas["ChartArea1"].AxisX.ScaleView.ZoomReset();
                chart1.ChartAreas["ChartArea2"].AxisX.ScaleView.ZoomReset();
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
                chartArea1MinYAxis = Math.Min(Math.Min(double.Parse(csv.ElementAt(1)[0][0]),double.Parse(csv.ElementAt(1)[1][0]))
                    , double.Parse(csv.ElementAt(1)[2][0]));
            //chart1.ResetAutoValues();

            // Keep a constant number of points by removing them from the left
            while (chart1.Series[0].Points.Count > numberOfPointsInChart)
            {
                // Remove data points on the left side
                while (chart1.Series[0].Points.Count > numberOfPointsAfterRemoval)
                {
                    chart1.Series[0].Points.RemoveAt(0);
                }

                // Adjust X axis scale
                chart1.ChartAreas["ChartArea1"].AxisX.Minimum = pointIndex - numberOfPointsAfterRemoval;
                chart1.ChartAreas["ChartArea1"].AxisX.Maximum = chart1.ChartAreas["ChartArea1"].AxisX.Minimum + numberOfPointsInChart;
            }
            while (chart1.Series[1].Points.Count > numberOfPointsInChart)
            {
                // Remove data points on the left side
                while (chart1.Series[1].Points.Count > numberOfPointsAfterRemoval)
                {
                    chart1.Series[1].Points.RemoveAt(0);
                }

                // Adjust X axis scale
                chart1.ChartAreas["ChartArea2"].AxisX.Minimum = pointIndex - numberOfPointsAfterRemoval;
                chart1.ChartAreas["ChartArea2"].AxisX.Maximum = chart1.ChartAreas["ChartArea2"].AxisX.Minimum + numberOfPointsInChart;
            }
            // Invalidate chart
            chart1.Invalidate();
            
            // chart2 : Network statistics
            /*
            chart2.Series[0].Points.AddXY(DateTime.Now.ToLongTimeString(), devicesPacketLengthS[0]);
            chart2.Series[1].Points.AddXY(DateTime.Now.ToLongTimeString(), devicesPacketLengthR[0]);
            chart2.Series[2].Points.AddXY(DateTime.Now.ToLongTimeString(), devicesPacketLengthS[1]);
            chart2.Series[3].Points.AddXY(DateTime.Now.ToLongTimeString(), devicesPacketLengthR[1]);
            chart2.Series[4].Points.AddXY(DateTime.Now.ToLongTimeString(), devicesPacketLengthS[2]);
            chart2.Series[5].Points.AddXY(DateTime.Now.ToLongTimeString(), devicesPacketLengthR[2]);
            chart2.Series[6].Points.AddXY(DateTime.Now.ToLongTimeString(), devicesPacketLengthS[3]);
            chart2.Series[7].Points.AddXY(DateTime.Now.ToLongTimeString(), devicesPacketLengthR[3]);
            for (int i = 0; i < 4; i++)
            {
                devicesPacketLengthS[i] = 0;
                devicesPacketLengthR[i] = 0;
            }
            */
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TextBoxUtils.ScrollToBottom(textBox1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            deviceMasterSent.StopCapture();
            deviceMasterSent.Close();
            deviceMasterRecv.StopCapture();
            deviceMasterRecv.Close();
            device1Sent.StopCapture();
            device1Sent.Close();
            device2Sent.StopCapture();
            device2Sent.Close();
            device3Sent.StopCapture();
            device3Sent.Close();

            //isDeviceOpen = false;

            deviceMasterSent = devices[deviceNumber];
            deviceMasterRecv = devices[deviceNumber];
            device1Sent = devices[deviceNumber];
            device1Recv = devices[deviceNumber];
            device2Sent = devices[deviceNumber];
            device2Recv = devices[deviceNumber];
            device3Sent = devices[deviceNumber];
            device3Recv = devices[deviceNumber];

            Console.WriteLine("-- Listening on {0}", deviceMasterSent.Description);

            // Register our handler function to the
            // 'packet arrival' event
            deviceMasterSent.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);
            deviceMasterRecv.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);
            device1Sent.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);
            device1Recv.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);
            device2Sent.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);
            device2Recv.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);
            device3Sent.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);
            device3Recv.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);

            // Open the device for capturing
            deviceMasterSent.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            deviceMasterRecv.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            device1Sent.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            device1Recv.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            device2Sent.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            device2Recv.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            device3Sent.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            device3Recv.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);



            /*// Set device to statistics mode
            deviceMasterSent.Mode = CaptureMode.Statistics;
            deviceMasterRecv.Mode = CaptureMode.Statistics;
            device1Sent.Mode = CaptureMode.Statistics;
            device1Recv.Mode = CaptureMode.Statistics;
            device2Sent.Mode = CaptureMode.Statistics;
            device2Recv.Mode = CaptureMode.Statistics;
            device3Sent.Mode = CaptureMode.Statistics;
            device3Recv.Mode = CaptureMode.Statistics;*/

            // Set device filter policy
            //deviceMasterSent.Filter = "src net 161.246.73.99";
            //deviceMasterRecv.Filter = "dst net 161.246.73.99";
            device1Sent.Filter = "src net 192.168.137.21";
            device1Recv.Filter = "dst net 192.168.137.21";
            device2Sent.Filter = "src net 192.168.137.22";
            device2Recv.Filter = "dst net 192.168.137.22";
            device3Sent.Filter = "src net 192.168.137.23";
            device3Recv.Filter = "dst net 192.168.137.23";

            deviceMasterSent.StartCapture();
            deviceMasterRecv.StartCapture();
            device1Sent.StartCapture();
            device1Recv.StartCapture();
            device2Sent.StartCapture();
            device2Recv.StartCapture();
            device3Sent.StartCapture();

            //isDeviceOpen = true;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            deviceMasterSent.StopCapture();
            deviceMasterSent.Close();
            deviceMasterRecv.StopCapture();
            deviceMasterRecv.Close();
            device1Sent.StopCapture();
            device1Sent.Close();
            device2Sent.StopCapture();
            device2Sent.Close();
            device3Sent.StopCapture();
            device3Sent.Close();

            //isDeviceOpen = false;

            deviceNumber = (int)numericUpDown1.Value;

            deviceMasterSent = devices[deviceNumber] ;
            device1Sent = devices[deviceNumber] ;
            device2Sent = devices[deviceNumber] ;
            device3Sent = devices[deviceNumber] ;
            deviceMasterRecv = devices[deviceNumber] ;
            device1Recv = devices[deviceNumber] ;
            device2Recv = devices[deviceNumber] ;
            device3Recv = devices[deviceNumber] ;

            Console.WriteLine("-- Set device to listening on {0}", deviceMasterSent.Description);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Stopping device...");

            deviceMasterSent.StopCapture();
            deviceMasterSent.Close();
            deviceMasterRecv.StopCapture();
            deviceMasterRecv.Close();
            device1Sent.StopCapture();
            device1Sent.Close();
            device2Sent.StopCapture();
            device2Sent.Close();
            device3Sent.StopCapture();
            device3Sent.Close();

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
    }
}
