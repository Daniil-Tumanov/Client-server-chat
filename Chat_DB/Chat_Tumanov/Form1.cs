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
using System.Threading;
using System.IO;
using System.Data.SqlClient;

namespace Chat_Tumanov
{

    public partial class Form1 : Form
    {
        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\AppData\Registration.mdf;Integrated Security=True";
        IPAddress IpServer;
        int port;
        public string Login;
        public TcpClient client;
        static NetworkStream SendMsg;

        public Form1()
        {
            InitializeComponent();

        }
        //Нажатие кнопки соединения
        public void Connect_Click(object sender, EventArgs e)
        {
            using (SqlConnection connect = new SqlConnection(connectionString))
            {
                connect.Open();
                SqlCommand Auth = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Login = N'" + textBox3.Text + "' AND Password = N'" + textBox4.Text + "'", connect);
                int temp = Convert.ToInt16(Auth.ExecuteScalar());
                if (temp == 1)
                {
                    IpServer = IPAddress.Parse(textBox1.Text.ToString());
                    port = Convert.ToInt32(textBox2.Text);
                    Login = textBox3.Text;
                    ConnectMsg();
                }
                else
                {
                    MessageBox.Show("Неправильный логин или пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            //  MessageBox.Show(DateTime.Now.ToString("[HH:mm:ss]"));
        }
        //Нажатие кнопки отправки
        private void Send_Click(object sender, EventArgs e)
        {
            SendMessage();

        }
        //Соединение
        private void ConnectMsg()
        {
            Socket socket = new Socket(IpServer.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(IpServer, port);
                label4.Text = "Подключение с сервером установлено";
                Send.Enabled = true;
                string message = DateTime.Now.ToString("[HH:mm:ss]") + Login + " подключился к чату";
                int SendMsg = socket.Send(Encoding.UTF8.GetBytes(message));
                byte[] buffer = new byte[1024];
                int bytesRec = socket.Receive(buffer);
                richTextBox2.Text = richTextBox2.Text + message + "\r\n";
            }
            catch
            {
                label4.Text = "Ошибка подключения к серверу";
                socket.Close();
            }
        }
        //Отправка сообщений
        private void SendMessage()
        {
            Socket socket = new Socket(IpServer.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(IpServer, port);
                label4.Text = "Подключение с сервером установлено";
                Send.Enabled = true;
                byte[] key = Encoding.UTF8.GetBytes("Key");
                RC4 encoder = new RC4(key);
                string message = DateTime.Now.ToString("[HH:mm:ss]") + Login + ": " + richTextBox1.Text;
                byte[] encryptBytes = Encoding.UTF8.GetBytes(message);
                byte[] result = encoder.Encode(encryptBytes, encryptBytes.Length);
                int SendMsg = socket.Send(result);
                byte[] buffer = new byte[1024];
                int bytesRec = socket.Receive(buffer);
                RC4 decoder = new RC4(key);
                byte[] decryptedBytes = decoder.Decode(result, result.Length);
                string message1 = Encoding.UTF8.GetString(decryptedBytes);
                richTextBox2.Text = richTextBox2.Text + message1 + "\r\n";
                using (var sw = File.AppendText(@"Log_chat.txt"))
                {
                    sw.WriteLine(message + "\r\n");
                }
                richTextBox1.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //Регистрация
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form2 newForm = new Form2();
            newForm.Show();
        }

        //Шифрование
        public class RC4
        {
            byte[] S = new byte[256];

            int x = 0;
            int y = 0;

            public RC4(byte[] key)
            {
                init(key);
            }
 
            private void init(byte[] key)
            {
                int keyLength = key.Length;

                for (int i = 0; i < 256; i++)
                {
                    S[i] = (byte)i;
                }

                int j = 0;
                for (int i = 0; i < 256; i++)
                {
                    j = (j + S[i] + key[i % keyLength]) % 256;
                    S.Swap(i, j);
                }
            }

            public byte[] Encode(byte[] dataB, int size)
            {
                byte[] data = dataB.Take(size).ToArray();
                byte[] cipher = new byte[data.Length];
                for (int m = 0; m < data.Length; m++)
                {
                    cipher[m] = (byte)(data[m] ^ keyItem());
                }
                return cipher;
            }
            public byte[] Decode(byte[] dataB, int size)
            {
                return Encode(dataB, size);
            }

            private byte keyItem()
            {
                x = (x + 1) % 256;
                y = (y + S[x]) % 256;

                S.Swap(x, y);

                return S[(S[x] + S[y]) % 256];
            }
        }
    }
        static class SwapExt
        {
            public static void Swap<T>(this T[] array, int index1, int index2)
            {
                T temp = array[index1];
                array[index1] = array[index2];
                array[index2] = temp;
            }
        }
    }



    
        


