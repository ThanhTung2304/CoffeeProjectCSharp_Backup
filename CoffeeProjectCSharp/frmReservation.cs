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
    public partial class frmReservation : Form
    {
        string Status = "Reset";
        public frmReservation()
        {
            InitializeComponent();
            LoadBan();
            LoadTrangThai();
            SetInterface("Reset");
            LoadData();
        }

        void LoadBan()
        {
            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT Id, MaBan FROM Ban", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                comboBox1.DataSource = dt;
                comboBox1.DisplayMember = "MaBan";
                comboBox1.ValueMember = "Id";
            }
        }

        void LoadTrangThai()
        {
            cboTrangThai.Items.AddRange(new string[]
            {
                "Đã đặt", "Đang sử dụng"
            });
            cboTrangThai.SelectedIndex = 0;
        }

        void SetInterface(string status)
        {
            bool edit = status != "Reset";

            comboBox1.Enabled = edit;
            txtTenKhach.Enabled = edit;
            txtSDT.Enabled = edit;
            dtpThoiGian.Enabled = edit;
            cboTrangThai.Enabled = edit;
            txtGhiChu.Enabled = edit;

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
                SqlDataAdapter da = new SqlDataAdapter(
                    @"SELECT d.Id, b.MaBan, d.TenKhach, d.SDT,
                             d.ThoiGian, d.TrangThai, d.GhiChu
                      FROM DatBan d
                      JOIN Ban b ON d.BanId = b.Id", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvDatBan.DataSource = dt;
            }
        }

        private void dgvDatBan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow r = dgvDatBan.Rows[e.RowIndex];

            txtID.Text = r.Cells["Id"].Value.ToString();
            comboBox1.Text = r.Cells["MaBan"].Value.ToString();
            txtTenKhach.Text = r.Cells["TenKhach"].Value.ToString();
            txtSDT.Text = r.Cells["SDT"].Value.ToString();
            dtpThoiGian.Value = Convert.ToDateTime(r.Cells["ThoiGian"].Value);
            cboTrangThai.Text = r.Cells["TrangThai"].Value.ToString();
            txtGhiChu.Text = r.Cells["GhiChu"].Value.ToString();
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
                    "DELETE FROM DatBan WHERE Id=@Id", conn);
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
                        @"INSERT INTO DatBan(BanId,TenKhach,SDT,ThoiGian,TrangThai,GhiChu)
                          VALUES(@Ban,@Ten,@SDT,@TG,@TT,@GC)";
                }
                else
                {
                    cmd.CommandText =
                        @"UPDATE DatBan SET
                          BanId=@Ban, TenKhach=@Ten, SDT=@SDT,
                          ThoiGian=@TG, TrangThai=@TT, GhiChu=@GC
                          WHERE Id=@Id";
                    cmd.Parameters.AddWithValue("@Id", txtID.Text);
                }

                cmd.Parameters.AddWithValue("@Ban", comboBox1.SelectedValue);
                cmd.Parameters.AddWithValue("@Ten", txtTenKhach.Text);
                cmd.Parameters.AddWithValue("@SDT", txtSDT.Text);
                cmd.Parameters.AddWithValue("@TG", dtpThoiGian.Value);
                cmd.Parameters.AddWithValue("@TT", cboTrangThai.Text);
                cmd.Parameters.AddWithValue("@GC", txtGhiChu.Text);
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

        private void btnTim_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(
                    @"SELECT d.Id, b.MaBan, d.TenKhach, d.SDT,
                        d.ThoiGian, d.TrangThai, d.GhiChu
                        FROM DatBan d
                        JOIN Ban b ON d.BanId = b.Id
                        WHERE d.TenKhach LIKE @kw", conn);

                da.SelectCommand.Parameters.AddWithValue(
                    "@kw", "%" + txtSearch.Text + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvDatBan.DataSource = dt;
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
            if (dgvDatBan.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel (*.csv)|*.csv";
            sfd.FileName = "DanhSachDatBan.csv";
            sfd.DefaultExt = "csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
                    {
                        // ===== HEADER =====
                        for (int i = 0; i < dgvDatBan.Columns.Count; i++)
                        {
                            sw.Write(dgvDatBan.Columns[i].HeaderText);
                            if (i < dgvDatBan.Columns.Count - 1)
                                sw.Write(",");
                        }
                        sw.WriteLine();

                        // ===== DATA =====
                        foreach (DataGridViewRow row in dgvDatBan.Rows)
                        {
                            if (row.IsNewRow) continue;

                            for (int i = 0; i < dgvDatBan.Columns.Count; i++)
                            {
                                sw.Write(row.Cells[i].Value?.ToString());
                                if (i < dgvDatBan.Columns.Count - 1)
                                    sw.Write(",");
                            }
                            sw.WriteLine();
                        }
                    }

                    MessageBox.Show("Xuất Excel danh sách đặt bàn thành công!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất file: " + ex.Message);
                }
            }
        }

        private void frmReservation_Load(object sender, EventArgs e)
        {

        }
    }
}
