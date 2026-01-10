using ClosedXML.Excel;
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
    public partial class ShiftForm : Form
    {
        string Status = "Reset";
        public ShiftForm()
        {
            InitializeComponent();
            SetInterface("Reset");
            GetData();
        }
        private void SetInterface(string status)
        {
            bool editing = status != "Reset";

            txtTenCa.Enabled = editing;
            dtpGioBatDau.Enabled = editing;
            dtpGioKetThuc.Enabled = editing;
            chkStatus.Enabled = editing;

            btnThem.Enabled = !editing;
            btnSua.Enabled = !editing;
            btnXoa.Enabled = !editing;

            btnLuu.Enabled = editing;
            btnHuy.Enabled = editing;
        }
        // ================== LOAD DATA ==================
        private void GetData()
        {
            using (SqlConnection conn =
                new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT Id, ShiftName, StartTime, EndTime, Status FROM Shift",
                    conn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvShift.AutoGenerateColumns = true;
                dgvShift.DataSource = dt;

                if (dgvShift.Columns["Id"] != null)
                    dgvShift.Columns["Id"].Visible = false;
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            Status = "Add";
            SetInterface(Status);
            ClearInput();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvShift.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ca làm cần sửa!");
                return;
            }

            DataGridViewRow row = dgvShift.SelectedRows[0];

            txtTenCa.Text = row.Cells["ShiftName"].Value.ToString();
            dtpGioBatDau.Value = DateTime.Today + (TimeSpan)row.Cells["StartTime"].Value;
            dtpGioKetThuc.Value = DateTime.Today + (TimeSpan)row.Cells["EndTime"].Value;
            chkStatus.Checked = Convert.ToInt32(row.Cells["Status"].Value) == 1;

            Status = "Edit";
            SetInterface(Status);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvShift.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ca cần xóa!");
                return;
            }

            int id = Convert.ToInt32(
                dgvShift.SelectedRows[0].Cells["Id"].Value);

            if (MessageBox.Show("Xóa ca làm này?",
                "Xác nhận",
                MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            using (SqlConnection conn =
                new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "UPDATE Shift SET Status = 0 WHERE Id=@Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Xóa ca làm thành công!");
            GetData();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                string tenCa = txtTenCa.Text.Trim();
                TimeSpan gioBD = dtpGioBatDau.Value.TimeOfDay;
                TimeSpan gioKT = dtpGioKetThuc.Value.TimeOfDay;
                int statusValue = chkStatus.Checked ? 1 : 0;

                if (string.IsNullOrEmpty(tenCa))
                {
                    MessageBox.Show("Vui lòng nhập tên ca!");
                    return;
                }

                using (SqlConnection conn =
                    new SqlConnection(ConfigDB.connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;

                    if (Status == "Add")
                    {
                        cmd.CommandText = @"INSERT INTO Shift
                        (ShiftName, StartTime, EndTime, Status)
                        VALUES (@Name, @Start, @End, @Status)";
                    }
                    else if (Status == "Edit")
                    {
                        int id = Convert.ToInt32(
                            dgvShift.SelectedRows[0].Cells["Id"].Value);

                        cmd.CommandText = @"UPDATE Shift SET
                        ShiftName=@Name,
                        StartTime=@Start,
                        EndTime=@End,
                        Status=@Status
                        WHERE Id=@Id";

                        cmd.Parameters.AddWithValue("@Id", id);
                    }

                    cmd.Parameters.AddWithValue("@Name", tenCa);
                    cmd.Parameters.AddWithValue("@Start", gioBD);
                    cmd.Parameters.AddWithValue("@End", gioKT);
                    cmd.Parameters.AddWithValue("@Status", statusValue);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Lưu ca làm thành công!");
                Status = "Reset";
                SetInterface(Status);
                GetData();
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
            ClearInput();
        }

        private void dgvShift_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvShift.Rows[e.RowIndex];
            txtTenCa.Text = row.Cells["ShiftName"].Value.ToString();
            dtpGioBatDau.Value = DateTime.Today + (TimeSpan)row.Cells["StartTime"].Value;
            dtpGioKetThuc.Value = DateTime.Today + (TimeSpan)row.Cells["EndTime"].Value;
            chkStatus.Checked = Convert.ToInt32(row.Cells["Status"].Value) == 1;
        }
        private void ClearInput()
        {
            txtTenCa.Clear();
            dtpGioBatDau.Value = DateTime.Now;
            dtpGioKetThuc.Value = DateTime.Now;
            chkStatus.Checked = true;
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            if (dgvShift.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Files|*.xlsx";
            sfd.Title = "Lưu file Excel";
            sfd.FileName = "DanhSachCaLam.xlsx";

            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Ca làm");

                    for (int i = 0; i < dgvShift.Columns.Count; i++)
                    {
                        ws.Cell(1, i + 1).Value = dgvShift.Columns[i].HeaderText;
                        ws.Cell(1, i + 1).Style.Font.Bold = true;
                    }

                    for (int i = 0; i < dgvShift.Rows.Count; i++)
                    {
                        for (int j = 0; j < dgvShift.Columns.Count; j++)
                        {
                            ws.Cell(i + 2, j + 1).Value =
                                dgvShift.Rows[i].Cells[j].Value?.ToString();
                        }
                    }

                    ws.Columns().AdjustToContents();
                    wb.SaveAs(sfd.FileName);
                }

                MessageBox.Show("Xuất Excel thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất Excel: " + ex.Message);
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim();

            using (SqlConnection conn =
                new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();

                string sql = @"
            SELECT Id, ShiftName, StartTime, EndTime, Status
            FROM Shift
            WHERE Status = 1
              AND ShiftName LIKE @kw
        ";

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvShift.AutoGenerateColumns = true;
                dgvShift.DataSource = dt;

                // Ẩn cột ID
                if (dgvShift.Columns["Id"] != null)
                    dgvShift.Columns["Id"].Visible = false;
            }
        }

        private void btnMain_Click(object sender, EventArgs e)
        {
            Mainframe main = new Mainframe();
            main.Show();
            this.Hide();
        }
    }
}
    
