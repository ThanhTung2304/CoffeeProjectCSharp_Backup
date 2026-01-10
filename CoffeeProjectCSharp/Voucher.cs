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
    public partial class Voucher : Form
    {
        string Status = "Reset";

        public Voucher()
        {
            InitializeComponent();
            SetInterface("Reset");
            GetData();
        }

        void SetInterface(string status)
        {
            if (status == "Reset")
            {
                txtID.Enabled = false;
                txtMaVoucher.Enabled = false;
                numGiam.Enabled = false;
                dtpBD.Enabled = false;
                dtpKT.Enabled = false;

                btnThem.Enabled = true;
                btnSua.Enabled = true;
                btnXoa.Enabled = true;

                btnLuu.Enabled = false;
                btnHuy.Enabled = false;
            }
            else // Add hoặc Edit
            {
                txtID.Enabled = false;
                txtMaVoucher.Enabled = true;
                numGiam.Enabled = true;
                dtpBD.Enabled = true;
                dtpKT.Enabled = true;

                btnThem.Enabled = false;
                btnSua.Enabled = false;
                btnXoa.Enabled = false;

                btnLuu.Enabled = true;
                btnHuy.Enabled = true;
            }
        }

        // ================== LOAD DATA ==================
        void GetData()
        {
            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Voucher", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvVoucher.AutoGenerateColumns = true;
                dgvVoucher.DataSource = dt;
            }
        }

        private void dgvVoucher_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvVoucher.Rows[e.RowIndex];
            txtID.Text = row.Cells["Id"].Value.ToString();
            txtMaVoucher.Text = row.Cells["MaVoucher"].Value.ToString();
            numGiam.Value = Convert.ToInt32(row.Cells["PhanTramGiam"].Value);
            dtpBD.Value = Convert.ToDateTime(row.Cells["NgayBatDau"].Value);
            dtpKT.Value = Convert.ToDateTime(row.Cells["NgayKetThuc"].Value);
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            Status = "Add";
            SetInterface(Status);

            txtMaVoucher.Clear();
            numGiam.Value = 0;
            dtpBD.Value = DateTime.Now;
            dtpKT.Value = DateTime.Now;
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Vui lòng chọn voucher cần sửa");
                return;
            }

            Status = "Edit";
            SetInterface(Status);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                MessageBox.Show("Vui lòng chọn voucher cần xóa");
                return;
            }

            DialogResult r = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa voucher này?",
                "Xác nhận",
                MessageBoxButtons.YesNo);

            if (r == DialogResult.No) return;

            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM Voucher WHERE Id=@Id", conn);
                cmd.Parameters.AddWithValue("@Id", txtID.Text);
                cmd.ExecuteNonQuery();
            }

            GetData();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaVoucher.Text))
            {
                MessageBox.Show("Mã voucher không được để trống");
                return;
            }

            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (Status == "Add")
                {
                    cmd.CommandText = @"
                        INSERT INTO Voucher
                        (MaVoucher, PhanTramGiam, NgayBatDau, NgayKetThuc)
                        VALUES (@Ma, @Giam, @BD, @KT)";
                }
                else if (Status == "Edit")
                {
                    cmd.CommandText = @"
                        UPDATE Voucher SET
                        MaVoucher=@Ma,
                        PhanTramGiam=@Giam,
                        NgayBatDau=@BD,
                        NgayKetThuc=@KT
                        WHERE Id=@Id";
                    cmd.Parameters.AddWithValue("@Id", txtID.Text);
                }

                cmd.Parameters.AddWithValue("@Ma", txtMaVoucher.Text);
                cmd.Parameters.AddWithValue("@Giam", numGiam.Value);
                cmd.Parameters.AddWithValue("@BD", dtpBD.Value);
                cmd.Parameters.AddWithValue("@KT", dtpKT.Value);

                cmd.ExecuteNonQuery();
            }

            Status = "Reset";
            SetInterface(Status);
            GetData();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            Status = "Reset";
            SetInterface(Status);
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT * FROM Voucher WHERE MaVoucher LIKE @kw", conn);
                da.SelectCommand.Parameters.AddWithValue(
                    "@kw", "%" + txtSearch.Text.Trim() + "%");
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvVoucher.DataSource = dt;
            }
        }

        private void btnTrangChu_Click(object sender, EventArgs e)
        {
            Mainframe main = new Mainframe();
            main.Show();
            this.Hide();
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            if (dgvVoucher.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel (*.csv)|*.csv";
            sfd.FileName = "DanhSachVoucher.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
                    {
                        // ====== GHI HEADER ======
                        for (int i = 0; i < dgvVoucher.Columns.Count; i++)
                        {
                            sw.Write(dgvVoucher.Columns[i].HeaderText);
                            if (i < dgvVoucher.Columns.Count - 1)
                                sw.Write(",");
                        }
                        sw.WriteLine();

                        // ====== GHI DATA ======
                        foreach (DataGridViewRow row in dgvVoucher.Rows)
                        {
                            if (row.IsNewRow) continue;

                            for (int i = 0; i < dgvVoucher.Columns.Count; i++)
                            {
                                sw.Write(row.Cells[i].Value?.ToString());
                                if (i < dgvVoucher.Columns.Count - 1)
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
    }
}
