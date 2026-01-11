using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace CoffeeProjectCSharp
{
    public partial class Form1 : Form
    {
        ConfigDB DangNhap = new ConfigDB();

        public static string CurrentUsername = "";
        public static string CurrentRole = ""; 
     

        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Resize += Form1_Resize;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            panelMain.Left = (this.ClientSize.Width - panelMain.Width) / 2;
            panelMain.Top = (this.ClientSize.Height - panelMain.Height) / 2;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = '●';
            chbShowPassword.Checked = false;
        }



        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Kiểm tra rỗng
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Đăng nhập
            ConfigDB configDB = new ConfigDB();
            DataTable dt = configDB.DangNhap(username, password);

            if (dt != null && dt.Rows.Count > 0)
            {
                // LƯU THÔNG TIN USER
                DataRow row = dt.Rows[0];
                CurrentUsername = row["Username"].ToString();
                CurrentRole = row["Role"].ToString();

                // Hỏi lưu mật khẩu
                DialogResult result = MessageBox.Show(
                    "Đăng nhập thành công!\nBạn có muốn lưu mật khẩu không?",
                    "Lưu mật khẩu",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    LuuMatKhau(username, password);
                }

                MessageBox.Show($"Xin chào: {CurrentUsername}\nChức vụ: {CurrentRole}",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Mainframe mainframe = new Mainframe();
                mainframe.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!",
                    "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtUsername.Focus();
            }
        }

        //Lưu mật khẩu vào file text
        private void LuuMatKhau(string username, string password)
        {
            try
            {
                string filePath = "accounts.txt";

                // Kiểm tra xem tài khoản đã tồn tại chưa
                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);
                    bool daTonTai = false;

                    foreach (string line in lines)
                    {
                        if (line.StartsWith(username + "|"))
                        {
                            daTonTai = true;
                            break;
                        }
                    }

                    if (daTonTai)
                    {
                        MessageBox.Show("Tài khoản này đã được lưu trước đó!",
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                // Lưu tài khoản mới
                string data = $"{username}|{password}\n";
                File.AppendAllText(filePath, data);

                MessageBox.Show("Đã lưu mật khẩu thành công!",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Register register = new Register();
            register.Show();
            this.Hide();
        }

        private void chbShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (chbShowPassword.Checked)
            {
                txtPassword.PasswordChar = '\0';
            }
            else
            {
                txtPassword.PasswordChar = '●'; 
            }
        }
    }
}
