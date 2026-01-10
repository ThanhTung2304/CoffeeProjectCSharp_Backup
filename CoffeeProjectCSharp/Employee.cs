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
using CoffeeProjectCSharp;
using ClosedXML.Excel;
using System.IO;



namespace CoffeeProjectCSharp
{
    public partial class EmployeeForm : Form
    {
        DataSet ds = new DataSet();
        string Status = "Reset";
        public EmployeeForm()
        {
            InitializeComponent();
            SetInterface("Reset");
            GetData();
        }

        private void btnNhanVien_Click(object sender, EventArgs e)
        {
            
        }


        // ================== GIAO DIỆN ==================
        private void SetInterface(string status)
        {
            bool editing = status != "Reset";

            txtHoTen.Enabled = editing;
            txtSDT.Enabled = editing;
            cboRole.Enabled = editing;
            dtpNgaySinh.Enabled = editing;
            rdoNam.Enabled = editing;
            rdoNu.Enabled = editing;

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
    @"SELECT Id, FullName, Phone, Role, BirthDate, Gender, Status
  FROM Employee",
    conn);

                DataTable dt = new DataTable();
                da.Fill(dt);


                dgvEmployee.AutoGenerateColumns = true;
                dgvEmployee.DataSource = dt;

                // ẨN CỘT ID
                if (dgvEmployee.Columns["Id"] != null)
                    dgvEmployee.Columns["Id"].Visible = false;

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
            if (dgvEmployee.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần sửa!");
                return;
            }

            DataGridViewRow row = dgvEmployee.SelectedRows[0];

            txtHoTen.Text = row.Cells["FullName"].Value.ToString();
            txtSDT.Text = row.Cells["Phone"].Value.ToString();
            cboRole.Text = row.Cells["Role"].Value.ToString();
            dtpNgaySinh.Value = Convert.ToDateTime(row.Cells["BirthDate"].Value);

            string gt = row.Cells["Gender"].Value.ToString();
            rdoNam.Checked = gt == "Nam";
            rdoNu.Checked = gt == "Nữ";

            Status = "Edit";
            SetInterface(Status);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvEmployee.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần xóa!");
                return;
            }

            int id = Convert.ToInt32(
                dgvEmployee.SelectedRows[0].Cells["Id"].Value);

            if (MessageBox.Show("Xóa nhân viên này?",
                "Xác nhận",
                MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            using (SqlConnection conn =
                new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "UPDATE Employee SET Status = 0 WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Xóa nhân viên thành công!");
            GetData();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                string hoTen = txtHoTen.Text.Trim();
                string sdt = txtSDT.Text.Trim();
                string role = cboRole.Text.Trim();
                DateTime ngaySinh = dtpNgaySinh.Value;
                string gioiTinh = rdoNam.Checked ? "Nam" : "Nữ";

                int statusValue = chkStatus.Checked ? 1 : 0;


                if (string.IsNullOrEmpty(hoTen))
                {
                    MessageBox.Show("Vui lòng nhập họ tên!");
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
                        cmd.CommandText = @"INSERT INTO Employee
                        (FullName, Phone, Role, BirthDate, Gender, Status)
                        VALUES (@Name, @Phone, @Role, @Birth, @Gender,@Status )";
                    }
                    else if (Status == "Edit")
                    {
                        int id = Convert.ToInt32(
                            dgvEmployee.SelectedRows[0].Cells["Id"].Value);

                        cmd.CommandText = @"UPDATE Employee SET
                        FullName=@Name,
                        Phone=@Phone,
                        Role=@Role,
                        BirthDate=@Birth,
                        Gender=@Gender,
                        Status=@Status
                        WHERE Id=@Id";

                        cmd.Parameters.AddWithValue("@Id", id);
                    }

                    cmd.Parameters.AddWithValue("@Name", hoTen);
                    cmd.Parameters.AddWithValue("@Phone", sdt);
                    cmd.Parameters.AddWithValue("@Role", role);
                    cmd.Parameters.AddWithValue("@Birth", ngaySinh);
                    cmd.Parameters.AddWithValue("@Gender", gioiTinh);
                    cmd.Parameters.AddWithValue("@Status", statusValue);


                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Lưu dữ liệu thành công!");
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

        private void dgvEmployee_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvEmployee.Rows[e.RowIndex];

            txtHoTen.Text = row.Cells["FullName"].Value.ToString();
            txtSDT.Text = row.Cells["Phone"].Value.ToString();
            cboRole.Text = row.Cells["Role"].Value.ToString();
            dtpNgaySinh.Value = Convert.ToDateTime(row.Cells["BirthDate"].Value);

            string gt = row.Cells["Gender"].Value.ToString();
            rdoNam.Checked = gt == "Nam";
            rdoNu.Checked = gt == "Nữ";
            chkStatus.Checked = Convert.ToInt32(row.Cells["Status"].Value) == 1;

        }
        private void ClearInput()
        {
            txtHoTen.Clear();
            txtSDT.Clear();
            cboRole.SelectedIndex = -1;
            dtpNgaySinh.Value = DateTime.Now;
            rdoNam.Checked = true;
            chkStatus.Checked = true;
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim();

            using (SqlConnection conn =
                new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();

                string sql = @"SELECT Id, FullName, Phone, Role, BirthDate, Gender
                       FROM Employee
                       WHERE Status = 1
                       AND (FullName LIKE @kw OR Phone LIKE @kw)";

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvEmployee.DataSource = dt;
            }
        }

        private void btnMain_Click(object sender, EventArgs e)
        {
            Mainframe mainframe = new Mainframe();
            mainframe.Show();
            this.Hide();
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            if (dgvEmployee.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Files|*.xlsx";
            sfd.Title = "Lưu file Excel";
            sfd.FileName = "DanhSachNhanVien.xlsx";

            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Nhân viên");

                    // ===== HEADER =====
                    for (int i = 0; i < dgvEmployee.Columns.Count; i++)
                    {
                        ws.Cell(1, i + 1).Value = dgvEmployee.Columns[i].HeaderText;
                        ws.Cell(1, i + 1).Style.Font.Bold = true;
                    }

                    // ===== DATA =====
                    for (int i = 0; i < dgvEmployee.Rows.Count; i++)
                    {
                        for (int j = 0; j < dgvEmployee.Columns.Count; j++)
                        {
                            ws.Cell(i + 2, j + 1).Value =
                                dgvEmployee.Rows[i].Cells[j].Value?.ToString();
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
    }

    }
