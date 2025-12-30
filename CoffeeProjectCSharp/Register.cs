using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            string username = txtUsername.Text;

        }

        private void llbRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }
    }
}
