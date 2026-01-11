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



        private void Mainframe_Load(object sender, EventArgs e)
        {

            MessageBox.Show($"Username: {Form1.CurrentUsername}\nRole: [{Form1.CurrentRole}]\nLength: {Form1.CurrentRole.Length}",
        "Debug Info");

            // PHÂN QUYỀN
            PhanQuyen();
        }

        private void PhanQuyen()
        {
            string role = Form1.CurrentRole.Trim();

            if (role == "Admin")
            {
                // Admin thấy tất cả
                button1.Visible = true;      // Quản lý tài khoản
                button10.Visible = true;      // Nhà cung cấp
            }
            else if (role == "Nhân viên")
            {
                // Ẩn các chức năng quản lý
                button1.Visible = false;     // Quản lý tài khoản
                button10.Visible = false;     // Nhà cung cấp
            }
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

        private void button4_Click(object sender, EventArgs e)
        {
            frmReservation frm = new frmReservation();
            frm.Show();
            this.Hide();
        }

        private void btnVoucher_Click(object sender, EventArgs e)
        {
            Voucher voucher = new Voucher();
            voucher.Show();
            this.Hide();
        }

        private void btnBan_Click(object sender, EventArgs e)
        {
            QLBan qLBan = new QLBan();
            qLBan.Show();
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
            scheduleForm.Show ();
            this.Hide();
            
        }
        private void button10_Click(object sender, EventArgs e)
        {
            Supplier supplier = new Supplier();
            supplier.Show();
            this.Hide();
        }

        private void btnVoucher1_Click(object sender, EventArgs e)
        {
            Voucher voucher = new Voucher();
            voucher.Show();
            this.Hide();
        }

        private void btnQLBan_Click(object sender, EventArgs e)
        {
            QLBan qLBan = new QLBan();
            qLBan.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Product product = new Product();
            product.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Inventory inventory = new Inventory();
            inventory.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Recipe recipe = new Recipe();
            recipe.Show();
            this.Hide();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            form.Show();
            this.Hide();
        }
    }
}
