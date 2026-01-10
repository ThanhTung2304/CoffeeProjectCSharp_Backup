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
    public partial class Mainframe : Form
    {
        public Mainframe()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Account account = new Account();
            account.Show();
            this.Hide();
        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            Account account = new Account();
            account.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            EmployeeForm emp = new EmployeeForm();

            emp.FormClosed += (s, args) =>
            {
                this.Show();
            };

            emp.Show();
            this.Hide();
        }

        private void btnCaLam_Click(object sender, EventArgs e)
        {
            ShiftForm shiftForm = new ShiftForm();

            shiftForm.FormClosed += (s, args) =>
            {
                this.Show();
            };

            shiftForm.Show();
            this.Hide();
        }

        private void btnLichLam_Click(object sender, EventArgs e)
        {
            EmployeeShiftForm scheduleForm = new EmployeeShiftForm();

            scheduleForm.FormClosed += (s, args) =>
            {
                this.Show();
            };

            scheduleForm.Show();
        }
        private void button10_Click(object sender, EventArgs e)
        {
            Supplier supplier = new Supplier();
            supplier.Show();
            this.Hide();
        }
    }
}
