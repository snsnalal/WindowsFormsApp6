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
using System.Threading;
using System.IO.Ports;

namespace WindowsFormsApp6
{
    public partial class Form1 : Form
    {
        public const int port = 32000; // 포트
        Socket socket = null;
        TcpListener server = null;
        Thread th = null;
        TcpClient cl;
        NetworkStream ns;
        StreamReader sr;
        StreamWriter sw;
        String a = null;
        Thread th2;
        Thread th3;
        public void Run() // 서버 실행
        {
            String myaddress = "211.220.184.10"; // ip주소
            IPAddress localAddr = IPAddress.Parse(myaddress);
            try
            {
                server = new TcpListener(localAddr, port);
                server.Start();
                listBox1.Items.Add("연결 대기..");
            } 
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }

           

            cl = server.AcceptTcpClient();

            if (cl.Connected)
            {
                listBox1.Items.Add("연결 완료!!");
                ns = cl.GetStream();
                sr = new StreamReader(ns);
                sw = new StreamWriter(ns);
            }

            th2 = new Thread(new ThreadStart(run2)); // 서버스레드 생성
            th2.Start();
            th3 = new Thread(new ThreadStart(run3));
            th3.Start();
        }
        public void run2()
        {
            while (true)
            {
                try
                {
                    a = serialPort1.ReadLine();

                    String[] b = a.Split(' ');
                    listBox1.Items.Add(b[0].Length);
                    b[1] = b[1].Remove(b[1].Length-1);
                    listBox1.Items.Add(b[1].Length);

                    sw.WriteLine(b[0]);
                    sw.WriteLine(b[1]);

                    sw.Flush();
                }
                catch (Exception e)
                {
                    listBox1.Items.Add("연결 끊기..");
                    MessageBox.Show(e.ToString());
                    break;
                }
            }
        }

        public void run3()
        {
            while(true)
            {
                String b = sr.ReadLine();
                String[] data = b.Split(' ');
               
                if (data[0].Equals("0"))
                {
                    serialPort1.Write(b);
                    listBox1.Items.Add("전송");

                }
                else if (data[0].Equals("1"))
                {
                    serialPort1.Write(b);
                    listBox1.Items.Add("전송");
                }
            }              
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;

            serialPort1.BaudRate = 9600;
            serialPort1.DataBits = 8;
            serialPort1.Parity = Parity.None;
            serialPort1.StopBits = StopBits.One;
            serialPort1.Open();
          
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serialPort1 != null && serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e) // 연결 버튼
        {
            th = new Thread(new ThreadStart(Run)); // 서버스레드 생성
            th.Start();
            button1.Enabled = false;
        }


        private void button3_Click(object sender, EventArgs e) // 종료 버튼
        {
            if (socket != null)
            {
                if (socket.Connected)
                {
                    socket.Close();
                    th.Abort();
                    th2.Abort();
                    th3.Abort();                  
                }
            }
            Application.Exit();
        }
    }
}
