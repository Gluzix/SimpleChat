using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace SimpleChat
{
    public partial class Form1 : Form
    {
        private string wiadomosc;
        private string recv_wiadomosc;
        private IPEndPoint ip;
        private UdpClient udpClient;
        private bool if_server = false;

        Thread thread;

        public Form1()
        {
            InitializeComponent();
        }
        ~Form1()
        {
            udpClient.Close();
        }

        public delegate void insert_to_textbox();

        public void to_textbox()
        {
            richTextBox2.Text += "Guest:" + recv_wiadomosc;
        }

        private void send(string wiadomosc)
        {
            byte[] data = Encoding.Unicode.GetBytes(wiadomosc);
            if(if_server)
            {
                udpClient.Send(data, data.Length, ip);
            }
            else
            {
                udpClient.Send(data, data.Length);
            }
        }

        private void receive()
        {
            while(true)
            {
                recv_wiadomosc = null;
                byte[] data = udpClient.Receive(ref ip);
                recv_wiadomosc = Encoding.Unicode.GetString(data);
                recv_wiadomosc += '\n';
                if(InvokeRequired)
                {
                    Invoke(new insert_to_textbox(to_textbox));
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(richTextBox1.Text!="")
            {
                wiadomosc = richTextBox1.Text + '\n';
                richTextBox2.Text += "Ty: " + wiadomosc;
                richTextBox1.Text = null;
                send(wiadomosc);
            }           
        }
        private void richTextBox1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            { 
                if(Keyboard.IsKeyDown(Key.LeftShift))
                {
                    e.Handled = false;
                }
                else
                {
                    button1_Click(null, null);
                    e.Handled = true;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string port = textBox2.Text;
            int x = int.Parse(port);
            string addr = textBox1.Text;

            if (radioButton1.Checked)
            {
                if_server = true;
                udpClient = new UdpClient(x);
                ip = new IPEndPoint(IPAddress.Any, x);
                thread = new Thread(receive);
                thread.IsBackground = true;
                thread.Start();
            }
            else if(radioButton2.Checked)
            {
                ip = new IPEndPoint(IPAddress.Parse(addr), x);
                udpClient = new UdpClient();
                udpClient.Connect(ip);
                thread = new Thread(receive);
                thread.IsBackground = true;
                thread.Start();
            }
            panel1.Enabled = false;
        }
    }
}
