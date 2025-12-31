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

namespace CoffeeProjectCSharp
{
    public partial class Register : Form
    {

        ConfigDB configDB = new ConfigDB();

        public Register()
        {
            InitializeComponent();

            this.StartPosition = FormStartPosition.CenterScreen;

            txtPassword.PasswordChar = '*';
            txtConfirm.PasswordChar = '*';
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string confirm = txtConfirm.Text;

            if (username == "" || password == "" || confirm == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            if (password != confirm)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!");
                return;
            }

            try
            {
                using (SqlConnection conn = configDB.GetConnection())
                {
                    conn.Open();

                    string checkSql = "SELECT COUNT(*) FROM Account WHERE Username=@username";
                    SqlCommand checkCmd = new SqlCommand(checkSql, conn);
                    checkCmd.Parameters.AddWithValue("@username", username);

                    if ((int)checkCmd.ExecuteScalar() > 0)
                    {
                        MessageBox.Show("Tên đăng nhập đã tồn tại!");
                        return;
                    }

                    string insertSql =
                        "INSERT INTO Account (Username, Password, Role) VALUES (@username, @password, @role)";

                    SqlCommand insertCmd = new SqlCommand(insertSql, conn);
                    insertCmd.Parameters.AddWithValue("@username", username);
                    insertCmd.Parameters.AddWithValue("@password", password);
                    insertCmd.Parameters.AddWithValue("@role", "User"); 

                    int rows = insertCmd.ExecuteNonQuery();

                    if (rows > 0)
                        MessageBox.Show("Đăng ký thành công!");
                    else
                        MessageBox.Show("Đăng ký thất bại!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }


        private void llbRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }
    }
}
