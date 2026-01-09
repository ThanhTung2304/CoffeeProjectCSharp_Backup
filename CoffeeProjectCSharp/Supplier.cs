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
                txtName.Enabled = true;
                txtAddress.Enabled = true;
                txtPhone.Enabled = true;
                txtNote.Enabled = true;
                txtEmail.Enabled = true;
                cboStatus.Enabled = true;

                btnThem.Enabled = true;
                btnLuu.Enabled = true;
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
                btnHuy.Enabled = true;

                txtName.Focus();

            }
            else if(Status == "Reset")
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
                btnSua.Enabled = true;
                btnXoa.Enabled = true;
                btnHuy.Enabled = false;
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
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
                btnHuy.Enabled = true;

                txtName.Focus();
            }
        }

        private void btnMain_Click(object sender, EventArgs e)
        {
            Mainframe mainframe = new Mainframe();
            mainframe.Show();
            this.Hide();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            Status = "Add";
            SetInterface(Status);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Account account = new Account();
            account.Show();
            this.Hide();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if(dgvSupplier.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp cần sửa!");
                return;
            }

            DataGridViewRow row = dgvSupplier.SelectedRows[0];
            txtID.Text = row.Cells["SupplierID"].Value.ToString();
            txtName.Text = row.Cells["SupplierName"].Value.ToString();
            txtAddress.Text = row.Cells["Address"].Value.ToString();
            txtPhone.Text = row.Cells["Phone"].Value.ToString();
            txtEmail.Text = row.Cells["Email"].Value.ToString();
            txtNote.Text = row.Cells["Note"].Value.ToString();
            cboStatus.Text = row.Cells["Status1"].Value.ToString();

            Status = "Edit";
            SetInterface(Status);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvSupplier.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp cần xóa!");
                return;
            }

            // Lấy ID từ dòng được chọn
            int supplierID = Convert.ToInt32(
                dgvSupplier.SelectedRows[0].Cells["SupplierID"].Value
            );

            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn ngừng hợp tác nhà cung cấp này không?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM Supplier WHERE SupplierID = @SupplierID";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@SupplierID", supplierID);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            MessageBox.Show("Đã xóa nhà cung cấp thành công");
                            GetData();
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy nhà cung cấp");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                string supplierName = txtName.Text;
                string address = txtAddress.Text;
                string phone = txtPhone.Text;
                string email = txtEmail.Text;
                string note = txtNote.Text;
                string status = cboStatus.Text;
                using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    if (Status == "Add")
                    {
                        cmd.CommandText = "INSERT INTO Supplier (SupplierName, Address, Phone, Email, Note, Status1) " +
                                          "VALUES (@SupplierName, @Address, @Phone, @Email, @Note, @Status1)";
                    }
                    else if (Status == "Edit")
                    {
                        cmd.CommandText = "UPDATE Supplier SET SupplierName = @SupplierName, Address = @Address, " +
                                          "Phone = @Phone, Email = @Email, Note = @Note, Status1 = @Status1 " +
                                          "WHERE SupplierID = @SupplierID";
                        cmd.Parameters.AddWithValue("@SupplierID", txtID.Text);
                    }
                    cmd.Parameters.AddWithValue("@SupplierName", supplierName);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Note", note);
                    cmd.Parameters.AddWithValue("@Status1", status);
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("Lưu thông tin nhà cung cấp thành công");
                        GetData();
                        Status = "Reset";
                        SetInterface(Status);
                    }
                    else
                    {
                        MessageBox.Show("Lưu thông tin nhà cung cấp thất bại");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }


        private void btnHuy_Click(object sender, EventArgs e)
        {
            Status = "Reset";
            SetInterface(Status);
        }

        private void dgvView_CellClick(object sender, DataGridViewCellEventArgs e)
        { 
            if(e.RowIndex >= 0)
            {
                DataGridViewRow row = this.dgvSupplier.Rows[e.RowIndex];
                txtID.Text = row.Cells["SupplierID"].Value.ToString();
                txtName.Text = row.Cells["SupplierName"].Value.ToString();
                txtAddress.Text = row.Cells["Address"].Value.ToString();
                txtPhone.Text = row.Cells["Phone"].Value.ToString();
                txtEmail.Text = row.Cells["Email"].Value.ToString();
                txtNote.Text = row.Cells["Note"].Value.ToString();
                cboStatus.Text = row.Cells["Status1"].Value.ToString();
            }
        }

        private void cboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Chỉ xử lý khi chọn "Ngừng"
            if (cboStatus.SelectedItem != null &&
                cboStatus.SelectedItem.ToString() == "Ngừng")
            {
                DialogResult result = MessageBox.Show(
                    "Bạn có chắc chắn muốn ngừng hợp tác với nhà cung cấp này không?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                // Nếu KHÔNG đồng ý → trả lại Hoạt động
                if (result == DialogResult.No)
                {
                    cboStatus.SelectedItem = "Hoạt động";
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                GetData();
                return;
            }

            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                string query = @"SELECT * FROM Supplier
                         WHERE SupplierName LIKE @keyword
                         OR Phone LIKE @keyword
                         OR Email LIKE @keyword";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@keyword", "%" + keyword + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvSupplier.AutoGenerateColumns = true;
                dgvSupplier.DataSource = dt;
            }
        }
    }
}
