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
    public partial class Supplier : Form
    {

        DataSet ds = new DataSet();
        string Status = "Reset";

        public Supplier()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized; // mở full màn hình
            this.AutoScaleMode = AutoScaleMode.Dpi;


            SetInterface("Reset");
            GetData();
        }

        private void GetData()
        {
            using (SqlConnection conn =
                  new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                SqlDataAdapter da =
                    new SqlDataAdapter("SELECT * FROM Supplier", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvSupplier.AutoGenerateColumns = true;
                dgvSupplier.DataSource = dt;
            }
        }

        private void SetInterface(string Status)
        {
            if(Status == "Add")
            {
                txtID.Enabled = false;
                txtName.Enabled = false;
                txtAddress.Enabled = false;
                txtPhone.Enabled = false;
                txtNote.Enabled = false;
                txtEmail.Enabled = false;
                cboStatus.Enabled = false;

                btnThem.Enabled = true;
                btnLuu.Enabled = false;
                btnXoa.Enabled = false;
                btnSua.Enabled = false;
                btnHuy.Enabled = false;

            }
            else if(Status == "Reset")
            {
                txtID.Enabled = false;
                txtName.Enabled = true;
                txtAddress.Enabled = true;
                txtPhone.Enabled = true;
                txtNote.Enabled = true;
                txtEmail.Enabled = true;
                cboStatus.Enabled = true;

                btnThem.Enabled = false;
                btnLuu.Enabled = true;
                btnXoa.Enabled = false;
                btnSua.Enabled = false;
                btnHuy.Enabled = true;
            }
            else if(Status == "Edit")
            {
                txtID.Enabled = false;
                txtName.Enabled = true;
                txtAddress.Enabled = true;
                txtPhone.Enabled = true;
                txtNote.Enabled = true;
                txtEmail.Enabled = true;
                cboStatus.Enabled = true;

                btnThem.Enabled = false;
                btnLuu.Enabled = true;
                btnXoa.Enabled = true;
                btnSua.Enabled = false;
                btnHuy.Enabled = true;
            }
        }
    }
}
