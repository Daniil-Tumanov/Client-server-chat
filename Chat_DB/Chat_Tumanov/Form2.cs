using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chat_Tumanov
{
    public partial class Form2 : Form
    {
       
        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\AppData\Registration.mdf;Integrated Security=True";
        public Form2()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text !="" && textBox2.Text != "")
            {
                string sql = "INSERT INTO Users (Login, Password) VALUES (N'" + textBox1.Text + "', N'" + textBox2.Text + "') ;";
                try
                {
                    string check = "SELECT Users WHERE login = '" + textBox1.Text + "'";
                    using (SqlConnection connect = new SqlConnection(connectionString))
                    {
                       
                        connect.Open();
                        SqlCommand Check = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Login = N'" + textBox1.Text + "'", connect);
                        int temp = Convert.ToInt16(Check.ExecuteScalar());
                        if (temp == 1)
                        {
                            MessageBox.Show("Пользователь с логином " + textBox1.Text + " уже существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            SqlCommand command = new SqlCommand(sql, connect);
                            command.ExecuteNonQuery();
                            MessageBox.Show("Регистрация успешно завершена", "Регистрация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }

                catch (Exception)
                {
                    MessageBox.Show("Ошибка регистрации", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
              
            }
            else   
            {
                MessageBox.Show("Не введены данные", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
    }
}
