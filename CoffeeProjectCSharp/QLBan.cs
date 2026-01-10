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
    public partial class QLBan : Form
    {
        string Status = "Reset";

        public QLBan()
        {
            InitializeComponent();
            LoadTrangThai();
            SetInterface("Reset");
            LoadData();

        }

        void LoadTrangThai()
        {
            cboTrangThai.Items.Clear();
            cboTrangThai.Items.AddRange(new string[]
            {
                "Trống", "Đã đặt", "Đang sử dụng"
            });
            cboTrangThai.SelectedIndex = 0;
        }

        void SetInterface(string status)
        {
            bool edit = status != "Reset";

            txtMaBan.Enabled = edit;
            numSoCho.Enabled = edit;
            cboTrangThai.Enabled = edit;

            btnThem.Enabled = !edit;
            btnSua.Enabled = !edit;
            btnXoa.Enabled = !edit;

            btnLuu.Enabled = edit;
            btnHuy.Enabled = edit;
        }

        void LoadData()
        {
            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Ban", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvBan.DataSource = dt;
            }
        }

        private void dgvBan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow r = dgvBan.Rows[e.RowIndex];

            txtID.Text = r.Cells["Id"].Value.ToString();
            txtMaBan.Text = r.Cells["MaBan"].Value.ToString();
            numSoCho.Value = Convert.ToInt32(r.Cells["SoCho"].Value);
            cboTrangThai.Text = r.Cells["TrangThai"].Value.ToString();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            Status = "Add";
            SetInterface(Status);
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (txtID.Text == "") return;
            Status = "Edit";
            SetInterface(Status);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM Ban WHERE Id=@Id", conn);
                cmd.Parameters.AddWithValue("@Id", txtID.Text);
                cmd.ExecuteNonQuery();
            }
            LoadData();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                if (Status == "Add")
                {
                    cmd.CommandText =
                        "INSERT INTO Ban(MaBan,SoCho,TrangThai) VALUES(@Ma,@SoCho,@TT)";
                }
                else
                {
                    cmd.CommandText =
                        "UPDATE Ban SET MaBan=@Ma,SoCho=@SoCho,TrangThai=@TT WHERE Id=@Id";
                    cmd.Parameters.AddWithValue("@Id", txtID.Text);
                }

                cmd.Parameters.AddWithValue("@Ma", txtMaBan.Text);
                cmd.Parameters.AddWithValue("@SoCho", numSoCho.Value);
                cmd.Parameters.AddWithValue("@TT", cboTrangThai.Text);
                cmd.ExecuteNonQuery();
            }

            Status = "Reset";
            SetInterface(Status);
            LoadData();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            Status = "Reset";
            SetInterface(Status);
        }

        private void btnTrangChu_Click(object sender, EventArgs e)
        {
            new Mainframe().Show();
            this.Hide();
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT * FROM Ban WHERE MaBan LIKE @kw", conn);
                da.SelectCommand.Parameters.AddWithValue(
                    "@kw", "%" + txtSearch.Text + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvBan.DataSource = dt;
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            if (dgvBan.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel (*.csv)|*.csv";
            sfd.FileName = "DanhSachBan.csv";
            sfd.DefaultExt = "csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
                    {
                        // ===== HEADER =====
                        for (int i = 0; i < dgvBan.Columns.Count; i++)
                        {
                            sw.Write(dgvBan.Columns[i].HeaderText);
                            if (i < dgvBan.Columns.Count - 1)
                                sw.Write(",");
                        }
                        sw.WriteLine();

                        // ===== DATA =====
                        foreach (DataGridViewRow row in dgvBan.Rows)
                        {
                            if (row.IsNewRow) continue;

                            for (int i = 0; i < dgvBan.Columns.Count; i++)
                            {
                                sw.Write(row.Cells[i].Value?.ToString());
                                if (i < dgvBan.Columns.Count - 1)
                                    sw.Write(",");
                            }
                            sw.WriteLine();
                        }
                    }

                    MessageBox.Show("Xuất Excel danh sách bàn thành công!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất file: " + ex.Message);
                }
            }
        }


    }
}
