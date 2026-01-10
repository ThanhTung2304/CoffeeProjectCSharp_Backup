using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            // Kiểm tra kết nối database
            txtPassword.UseSystemPasswordChar = true;
            chbShowPassword.Checked = false;
            txtPassword.MaxLength = 50;

            if (!DangNhap.TestConnection())
            {
                MessageBox.Show("Không thể kết nối đến database!",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            ConfigDB configDB = new ConfigDB(); // Tạo đối tượng
            DataTable dt = configDB.DangNhap(username, password); // ĐÚNG - Gọi qua đối tượng

            if (dt != null && dt.Rows.Count > 0)
            {
               
                string role = dt.Rows[0]["Role"].ToString();

                MessageBox.Show($"Đăng nhập thành công!\nXin chào: {role}",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);


                Mainframe mainframe = new Mainframe();
                mainframe.Show();
                // Đóng form đăng nhập
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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Register register = new Register();
            register.Show();
            this.Hide();
        }

        private void chbShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chbShowPassword.Checked;
        }
    }
}
