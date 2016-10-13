using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;
using System.IO;
using ServerClient = ZobowiazanieBitowe;

namespace ServerClient
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        public Boolean firstSend = false;
        public StreamReader STR;
        public StreamWriter STW;
        public string R1;
        public string S;
        public string R2;
        public string biteAlicestring;
        public string receive;
        public string text_to_send;
        public Form1()
        {
            InitializeComponent();

            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());      //get my own IP
            foreach (IPAddress address in localIP)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    textBox4.Text = address.ToString();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)      //start server
        {
            TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(textBox3.Text));
            listener.Start();
            textBox2.AppendText("The server is running at port:" + textBox3.Text + "\n");
            client = listener.AcceptTcpClient();
            STR = new StreamReader(client.GetStream());
            STW = new StreamWriter(client.GetStream());
            STW.AutoFlush = true;

            backgroundWorker1.RunWorkerAsync(); //start receiving data
            backgroundWorker2.WorkerSupportsCancellation = true; //Ability to cancel this thread
        }

        private void button3_Click(object sender, EventArgs e)
        {
            client = new TcpClient();
            IPEndPoint IP_End = new IPEndPoint(IPAddress.Parse(textBox5.Text), int.Parse(textBox6.Text));

            try
            {
                client.Connect(IP_End);
                if(client.Connected)
                {
                    textBox2.AppendText("Connected to server" + "\n");
                    STW = new StreamWriter(client.GetStream());
                    STR = new StreamReader(client.GetStream());
                    STW.AutoFlush = true;

                    backgroundWorker1.RunWorkerAsync(); //start receiving data
                    backgroundWorker2.WorkerSupportsCancellation = true; //Ability to cancel this thread
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)  //Send button
        {
            if(textBox1.Text != "" || firstSend == true)
            {
                if(firstSend == false)//protokół podjęcia decyzji
                {
                    byte[] stringbite = ZobowiazanieBitowe.HashString(textBox1.Text);
                    byte[] R1B = ZobowiazanieBitowe.LosowanieCiagow();
                    byte[] R2B = ZobowiazanieBitowe.LosowanieCiagow();
                    byte biteAlice = stringbite[stringbite.Length - 1];
                    R1 = BitConverter.ToString(R1B);
                    R2 = BitConverter.ToString(R2B);
                    biteAlicestring = biteAlice.ToString();
                    string S = ZobowiazanieBitowe.GetHashString(R1 + R2 + biteAlicestring);
                    text_to_send = ZobowiazanieBitowe.PodjecieDecyzjiSend(R1,S);
                    firstSend = true;
                }
                else
                {
                    text_to_send = ZobowiazanieBitowe.OdkryciaDecyzjiSend(R1, R2, biteAlicestring);
                    firstSend = false;
                }
                backgroundWorker2.RunWorkerAsync();
            }           
            textBox1.Text = "";
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) //receive data
        {
            while (client.Connected)
            {
                try
                {
                    if(firstSend == false)
                    {
                        receive = STR.ReadLine();
                        R1 = ZobowiazanieBitowe.PodjecieDecyzjiGetR1(receive);
                        S = ZobowiazanieBitowe.PodjecieDecyzjiGetS(receive);
                        this.textBox2.Invoke(new MethodInvoker(delegate () { textBox2.AppendText("i'm get R1 and S\n"); }));
                        firstSend = true;
                    }
                    else
                    {
                        receive = STR.ReadLine();
                        receive = ZobowiazanieBitowe.OdkryciaDecyzji(receive, R1, S);
                        this.textBox2.Invoke(new MethodInvoker(delegate () { textBox2.AppendText(receive + "\n"); }));
                        firstSend = false;
                    }

                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message.ToString());
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e) //send data
        {
            if (client.Connected)
            {
                STW.WriteLine(text_to_send);
                if(firstSend == true)
                    this.textBox2.Invoke(new MethodInvoker(delegate () { textBox2.AppendText("Click again send to check Bob accept\n"); }));
                else
                    this.textBox2.Invoke(new MethodInvoker(delegate () { textBox2.AppendText("You do next protokol zobowiazania bitowe\n"); }));
            }
            else
            {
                MessageBox.Show("Send failed!");
            }
            backgroundWorker2.CancelAsync();
        }
    }
}