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
using System.IO;

namespace CoffeeProjectCSharp
{
    public partial class EmployeeShiftForm : Form
    {
        string Status = "Reset";
        public EmployeeShiftForm()
        {
            InitializeComponent();
            SetInterface("Reset");
            LoadEmployee();
            LoadShift();
            GetData();
        }
        private void SetInterface(string status)
        {
            bool editing = status != "Reset";

            cboEmployee.Enabled = editing;
            cboShift.Enabled = editing;
            dtpWorkDate.Enabled = editing;
            txtNote.Enabled = editing;
            cboWorkStatus.Enabled = editing;

            btnThem.Enabled = !editing;
            btnSua.Enabled = !editing;
            btnXoa.Enabled = !editing;
            btnLuu.Enabled = editing;
            btnHuy.Enabled = editing;
        }
        private void LoadEmployee()
        {
            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT Id, FullName FROM Employee WHERE Status = 1", conn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                cboEmployee.DataSource = dt;
                cboEmployee.DisplayMember = "FullName";
                cboEmployee.ValueMember = "Id";
                cboEmployee.SelectedIndex = -1;
            }
        }
        private void LoadShift()
        {
            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT Id, ShiftName FROM Shift WHERE Status = 1", conn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                cboShift.DataSource = dt;
                cboShift.DisplayMember = "ShiftName";
                cboShift.ValueMember = "Id";
                cboShift.SelectedIndex = -1;
            }
        }
        private void GetData()
        {
            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(@"
                    SELECT es.Id,
                           e.FullName AS EmployeeName,
                           s.ShiftName,
                           es.WorkDate,
                           es.WorkStatus,
                           es.Note
                    FROM EmployeeShift es
                    JOIN Employee e ON es.EmployeeId = e.Id
                    JOIN Shift s ON es.ShiftId = s.Id
                    ORDER BY es.WorkDate DESC", conn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvEmployeeShift.AutoGenerateColumns = true;
                dgvEmployeeShift.DataSource = dt;

                if (dgvEmployeeShift.Columns["Id"] != null)
                    dgvEmployeeShift.Columns["Id"].Visible = false;
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
            if (dgvEmployeeShift.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn lịch làm!");
                return;
            }

            DataGridViewRow row = dgvEmployeeShift.SelectedRows[0];

            cboEmployee.Text = row.Cells["EmployeeName"].Value.ToString();
            cboShift.Text = row.Cells["ShiftName"].Value.ToString();
            dtpWorkDate.Value = Convert.ToDateTime(row.Cells["WorkDate"].Value);
            cboWorkStatus.Text = row.Cells["WorkStatus"].Value.ToString();
            txtNote.Text = row.Cells["Note"].Value?.ToString();

            Status = "Edit";
            SetInterface(Status);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvEmployeeShift.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn lịch làm!");
                return;
            }

            int id = Convert.ToInt32(
                dgvEmployeeShift.SelectedRows[0].Cells["Id"].Value);

            if (MessageBox.Show("Xóa lịch làm này?",
                "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM EmployeeShift WHERE Id=@Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Xóa thành công!");
            GetData();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;

                    if (Status == "Add")
                    {
                        cmd.CommandText = @"
                            INSERT INTO EmployeeShift
                            (EmployeeId, ShiftId, WorkDate, WorkStatus, Note)
                            VALUES (@Emp, @Shift, @Date, @Status, @Note)";
                    }
                    else
                    {
                        int id = Convert.ToInt32(
                            dgvEmployeeShift.SelectedRows[0].Cells["Id"].Value);

                        cmd.CommandText = @"
                            UPDATE EmployeeShift SET
                                EmployeeId=@Emp,
                                ShiftId=@Shift,
                                WorkDate=@Date,
                                WorkStatus=@Status,
                                Note=@Note
                            WHERE Id=@Id";

                        cmd.Parameters.AddWithValue("@Id", id);
                    }

                    cmd.Parameters.AddWithValue("@Emp", cboEmployee.SelectedValue);
                    cmd.Parameters.AddWithValue("@Shift", cboShift.SelectedValue);
                    cmd.Parameters.AddWithValue("@Date", dtpWorkDate.Value.Date);
                    cmd.Parameters.AddWithValue("@Status", cboWorkStatus.Text);
                    cmd.Parameters.AddWithValue("@Note", txtNote.Text);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Lưu thành công!");
                Status = "Reset";
                SetInterface(Status);
                GetData();
            }
            catch (SqlException ex)
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
        private void ClearInput()
        {
            cboEmployee.SelectedIndex = -1;
            cboShift.SelectedIndex = -1;
            dtpWorkDate.Value = DateTime.Now;
            cboWorkStatus.Text = "Đã phân công";
            txtNote.Clear();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string kw = txtTimKiem.Text.Trim();

            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(@"
                    SELECT es.Id,
                           e.FullName AS EmployeeName,
                           s.ShiftName,
                           es.WorkDate,
                           es.WorkStatus,
                           es.Note
                    FROM EmployeeShift es
                    JOIN Employee e ON es.EmployeeId = e.Id
                    JOIN Shift s ON es.ShiftId = s.Id
                    WHERE e.FullName LIKE @kw
                       OR s.ShiftName LIKE @kw", conn);

                da.SelectCommand.Parameters.AddWithValue("@kw", "%" + kw + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvEmployeeShift.DataSource = dt;
            }
        }

        private void btnTrangChu_Click(object sender, EventArgs e)
        {
            Mainframe main = new Mainframe();
            main.Show();
            this.Hide();
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            if (dgvEmployeeShift.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Files|*.xlsx";
            sfd.Title = "Lưu file Excel";
            sfd.FileName = "DanhSachLichLam.xlsx";

            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Lịch làm");

                    // ===== HEADER =====
                    for (int i = 0; i < dgvEmployeeShift.Columns.Count; i++)
                    {
                        ws.Cell(1, i + 1).Value =
                            dgvEmployeeShift.Columns[i].HeaderText;

                        ws.Cell(1, i + 1).Style.Font.Bold = true;
                    }

                    // ===== DATA =====
                    for (int i = 0; i < dgvEmployeeShift.Rows.Count; i++)
                    {
                        for (int j = 0; j < dgvEmployeeShift.Columns.Count; j++)
                        {
                            ws.Cell(i + 2, j + 1).Value =
                                dgvEmployeeShift.Rows[i].Cells[j].Value?.ToString();
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

        private void dgvEmployeeShift_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvEmployeeShift.Rows[e.RowIndex];

            cboEmployee.Text = row.Cells["EmployeeName"].Value.ToString();
            cboShift.Text = row.Cells["ShiftName"].Value.ToString();
            dtpWorkDate.Value = Convert.ToDateTime(row.Cells["WorkDate"].Value);

            cboWorkStatus.Text = row.Cells["WorkStatus"].Value.ToString();
            txtNote.Text = row.Cells["Note"].Value?.ToString();
        }
    }
}