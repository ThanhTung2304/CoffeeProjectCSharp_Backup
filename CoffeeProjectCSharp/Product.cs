using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoffeeProjectCSharp
{
    public partial class Product : Form
    {
        string Status = "Reset";
        public Product()
        {
            InitializeComponent();
            InitComboBox();

            SetInterface("Reset");
            GetData();
        }
        private void InitComboBox()
        {
            cboTrangthai.Items.Clear();
            cboTrangthai.Items.Add("Đang bán");   // index 0
            cboTrangthai.Items.Add("Ngưng bán");  // index 1

            if (cboTrangthai.Items.Count > 0)
                cboTrangthai.SelectedIndex = 0;
        }

        // UI
        /* ================= UI STATE ================= */
        private void SetInterface(string status)
        {
            if (status == "Reset")
            {
                txtID.Enabled = false;
                txtTen.Enabled = false;
                txtGia.Enabled = false;
                cboTrangthai.Enabled = false;
                

                btnThem.Enabled = true;
                btnSua.Enabled = true;
                btnXoa.Enabled = true;
                btnLuu.Enabled = false;
            }
            else
            {
                txtTen.Enabled = true;
                txtGia.Enabled = true;
                cboTrangthai.Enabled = true;
                

                btnThem.Enabled = false;
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
                btnLuu.Enabled = true;
            }
        }
        // ================= LOAD DATA =================
        private void GetData()
        {
            using (SqlConnection conn =
                 new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                SqlDataAdapter da =
                    new SqlDataAdapter("SELECT * FROM Product", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvProduct.AutoGenerateColumns = true;
                dgvProduct.DataSource = dt;
            }
        }

        private void Product_Load(object sender, EventArgs e)
        {

        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            Status = "Add";
            SetInterface(Status);

            txtID.Clear();
            txtTen.Clear();
            txtGia.Clear();
            cboTrangthai.SelectedIndex = 0;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();

            using (SqlConnection conn =
                new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT * FROM Product WHERE Name LIKE @kw", conn);

                da.SelectCommand.Parameters.AddWithValue(
                    "@kw", "%" + keyword + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvProduct.DataSource = dt;
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvProduct.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chọn sản phẩm cần sửa");
                return;
            }

            DataGridViewRow row = dgvProduct.SelectedRows[0];
            txtID.Text = row.Cells["Id"].Value.ToString();
            txtTen.Text = row.Cells["Name"].Value.ToString();
            txtGia.Text = row.Cells["Price"].Value.ToString();
            cboTrangthai.SelectedIndex =
                Convert.ToBoolean(row.Cells["IsActive"].Value) ? 0 : 1;
            dtpDop.Value =
                Convert.ToDateTime(row.Cells["CreatedTime"].Value);

            Status = "Edit";
            SetInterface(Status);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvProduct.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chọn sản phẩm cần xóa");
                return;
            }

            int id = Convert.ToInt32(
                dgvProduct.SelectedRows[0].Cells["Id"].Value);

            if (MessageBox.Show("Xóa sản phẩm này?",
                "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (SqlConnection conn =
                    new SqlConnection(ConfigDB.connectionString))
                {
                    conn.Open();
                    SqlCommand cmd =
                        new SqlCommand(
                            "DELETE FROM Product WHERE Id=@Id", conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }

                GetData();
                Status = "Reset";
                SetInterface(Status);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTen.Text) ||
                string.IsNullOrWhiteSpace(txtGia.Text))
            {
                MessageBox.Show("Nhập đầy đủ thông tin");
                return;
            }

            if (!decimal.TryParse(txtGia.Text, out decimal price))
            {
                MessageBox.Show("Giá không hợp lệ");
                return;
            }

            bool isActive = cboTrangthai.SelectedIndex == 0;

            using (SqlConnection conn =
                new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (Status == "Add")
                {
                    cmd.CommandText =
                        @"INSERT INTO Product(Name, Price, IsActive)
                          VALUES(@Name, @Price, @IsActive)";
                }
                else
                {
                    cmd.CommandText =
                        @"UPDATE Product
                          SET Name=@Name,
                              Price=@Price,
                              IsActive=@IsActive
                          WHERE Id=@Id";
                    cmd.Parameters.AddWithValue("@Id",
                        Convert.ToInt32(txtID.Text));
                }

                cmd.Parameters.AddWithValue("@Name", txtTen.Text.Trim());
                cmd.Parameters.AddWithValue("@Price", price);
                cmd.Parameters.AddWithValue("@IsActive", isActive);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Lưu thành công");
            GetData();
            Status = "Reset";
            SetInterface(Status);
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            Status = "Reset";
            SetInterface(Status);

            txtID.Clear();
            txtTen.Clear();
            txtGia.Clear();
            cboTrangthai.SelectedIndex = 0;
        }

        private void btnXuat_Click(object sender, EventArgs e)
        {
            if (dgvProduct.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel (*.csv)|*.csv";
            sfd.FileName = "DanhSachSanPham.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw =
                        new StreamWriter(sfd.FileName, false, Encoding.UTF8))
                    {
                        // ====== GHI HEADER ======
                        for (int i = 0; i < dgvProduct.Columns.Count; i++)
                        {
                            sw.Write(dgvProduct.Columns[i].HeaderText);
                            if (i < dgvProduct.Columns.Count - 1)
                                sw.Write(",");
                        }
                        sw.WriteLine();

                        // ====== GHI DATA ======
                        foreach (DataGridViewRow row in dgvProduct.Rows)
                        {
                            if (row.IsNewRow) continue;

                            for (int i = 0; i < dgvProduct.Columns.Count; i++)
                            {
                                sw.Write(row.Cells[i].Value?.ToString());
                                if (i < dgvProduct.Columns.Count - 1)
                                    sw.Write(",");
                            }
                            sw.WriteLine();
                        }
                    }

                    MessageBox.Show("Xuất Excel thành công!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất file: " + ex.Message);
                }
            }
        }

        private void btnMain_Click(object sender, EventArgs e)
        {
            Mainframe main = new Mainframe();
            main.Show();
            this.Hide();
        }

        private void dgvProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvProduct.Rows[e.RowIndex];
            txtID.Text = row.Cells["Id"].Value.ToString();
            txtTen.Text = row.Cells["Name"].Value.ToString();
            txtGia.Text = row.Cells["Price"].Value.ToString();
            cboTrangthai.SelectedIndex =
                Convert.ToBoolean(row.Cells["IsActive"].Value) ? 0 : 1;
            dtpDop.Value =
                Convert.ToDateTime(row.Cells["CreatedTime"].Value);

            Status = "Reset";
            SetInterface(Status);
        }

        private void cboTrangthai_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
