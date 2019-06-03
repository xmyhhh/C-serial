    using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.DirectX.DirectSound;
namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {

        Microsoft.DirectX.DirectSound.Device dev =
        new Microsoft.DirectX.DirectSound.Device();
        bool isPlaying = false;
        StringBuilder data = new StringBuilder("");
        StringBuilder datalable = new StringBuilder("");
        private string dataRead = "";
        private string TempVal = "0";
        private string HumidityVal = "0";
        private string LightVal = "0";
        StringBuilder Box1 = new StringBuilder("");
        StringBuilder Box2 = new StringBuilder("");
        StringBuilder Box3 = new StringBuilder("");
        SerialPort port =new SerialPort("COM4");

        bool mood_1 = false, mood_2 = false, mood_3 = true,mood_4=false;
        public delegate void MyInvoke(string str1,int i);

        public Form1()
        {
            InitializeComponent();
            dev.SetCooperativeLevel(this,
            Microsoft.DirectX.DirectSound.CooperativeLevel.Normal);
            this.FormClosed += new FormClosedEventHandler(Form1_FormClosed);
            port.ReceivedBytesThreshold = 1;
            port.BaudRate = 115200;
            port.DtrEnable = true;
            port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            port.Open();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Post();
        }
        private void port_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            dataRead = port.ReadLine();
            addText(dataRead, 1);
            addText("\n", 1);
            DataProcess();
        }


        public static void SetHeaderValue(WebHeaderCollection header, string name, string value)// HTTP协议报文头加入

        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection", BindingFlags.Instance | BindingFlags.NonPublic);
            if (property != null)
            {
                var collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = value;
            }
        }

        public void Post()
        {
            
            string url = "http://api.heclouds.com/devices/528166030/datapoints?";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            SetHeaderValue(request.Headers, "api-key", "JWrRnmOzLbxsGazvRKq6qv=43vI=");//设备API地址和 首部参数
            request.Host = "api.heclouds.com";
            request.ProtocolVersion = new Version(1, 1);
            string Cod = "{\"datastreams\":" +
                "[{\"id\":\"Temp\",\"datapoints\":[{\"value\":\"" + TempVal + "\"}]}," +
                "{\"id\":\"Humidity\",\"datapoints\":[{\"value\":\"" + HumidityVal + "\"}]}," +
                "{\"id\":\"Light\",\"datapoints\":[{\"value\":\"" + LightVal + "\"}]}]}";
            byte[] data = Encoding.UTF8.GetBytes(Cod);
            request.ContentLength = data.Length;
            addText("send over \n", 3);
            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
                request.Abort();

            }
            
           
        }//面向OneNet的发送

        private void DataProcess()
        {

            try
            {

                if (dataRead.Contains("Temp"))
                {


                    TempVal = dataRead.Replace("Temp=","");
                    //addText(TempVal, 2);
                    Post();
                }
                else if (dataRead.Contains("Light"))
                {
                    LightVal = dataRead.Replace("Light=", "");
                    //addText(LightVal, 3);
                    Post();
                }
                else if (dataRead.Contains("Humidity"))
                {
                    HumidityVal = dataRead.Replace("Humidity=", "");
                    //addText(HumidityVal, 2);
                    Post();
                }
                else { }

                
            }
            catch (Exception e) { }
        }
        private void updateText(String updateData,int ID) {
            
            MyInvoke mi = new MyInvoke(updateTextMethod);
            this.Invoke(mi, updateData, ID);
        }
        private void addText(String addData, int ID)
        {
            MyInvoke mi = new MyInvoke(addTextMethod);
            this.Invoke(mi,addData,ID);
        }
        private void playSound(String url)
        {
            try
            {
                Microsoft.DirectX.DirectSound.SecondaryBuffer snd =
                new Microsoft.DirectX.DirectSound.SecondaryBuffer(
                  url, dev);
                snd.Play(0, Microsoft.DirectX.DirectSound.BufferPlayFlags.Default);
            }
            catch (Exception ex)
            {
                ;
            }
        }
       
        public static void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)
            {
                Application.DoEvents();
            }
        }
        private void updateTextMethod(string text,int ID)
        {
            switch (ID) {
                case 1:{richTextBox1.Text = text;break; }
                case 2: { richTextBox2.Text = text; break; }
                case 3: { richTextBox3.Text = text; break; }
                case 4: { label1.Text = text; break; }
            }
        }
        private void addTextMethod(string text, int ID)
        {
            switch (ID)
            {
                case 1: { Box1.Append(text); Box1.Append("\n"); richTextBox1.Text = Box1.ToString(); break; }
                case 2: { Box2.Append(text); Box2.Append("\n"); richTextBox2.Text = Box2.ToString(); break; }
                case 3: { Box3.Append(text); Box3.Append("\n"); richTextBox3.Text = Box3.ToString(); break; }
                case 4: { label1.Text = text; break; }
            }
        }

        void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (port != null && port.IsOpen)
            {
                port.Close();
              
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            mood_1 = !mood_1;
        }
        private void mood2_CheckedChanged(object sender, EventArgs e)
        {
            mood_2 = !mood_2;
            if (mood_2 == true) port.Write("a");
            else port.Write("b");
        }
        private void mood3_CheckedChanged(object sender, EventArgs e)
        {
            mood_3 = !mood_3;
        }
        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            mood_4 = !mood_4;
            if (mood_4 == true) port.Write("c");
            else port.Write("d");
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {


            switch (comboBox1.SelectedItem.ToString()) {
                case "Rhythm1": {
                        port.Write("f");
                        addText("Rhythm1 \n", 1);
                        break; }
                case "Rhythm2":
                    {   
                        port.Write("g");
                        addText("Rhythm2 \n", 1);
                        break;
                    }
                case "Rhythm3":
                    {
                        port.Write("h");
                        addText("Rhythm3 \n", 1);
                        break;
                    }
                case "Rhythm4":
                    {
                        port.Write("i");
                        addText("Rhythm4 \n", 1);
                        break;
                    }
                default: break;
            }

        }
        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }


}
