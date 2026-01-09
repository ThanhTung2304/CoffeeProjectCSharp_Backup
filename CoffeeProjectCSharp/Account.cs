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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace CoffeeProjectCSharp
{
    public partial class Account : Form


    {

        DataSet ds = new DataSet();
        string Status = "Reset";

        

        public Account()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized; // mở full màn hình
            this.AutoScaleMode = AutoScaleMode.Dpi;
       

            SetInterface("Reset");
            GetData();
        }


        private void SetInterface(string status)
        {
            if (status == "Reset")
            {
                txtID.Enabled = false;
                txtUsername.Enabled = false;
                txtPassword.Enabled = false;
                cboRole.Enabled = false;
                dtpDob.Enabled = false;
                rdoMale.Enabled = false;
                rdoFemale.Enabled = false;

                btnThem.Enabled = true;
                btnLuu.Enabled = false;
                btnSua.Enabled = true;
                btnXoa.Enabled = true;
                btnHuy.Enabled = false;
            }
            else if (status == "Add")
            {
                txtID.Enabled = false;
                txtUsername.Enabled = true;
                txtPassword.Enabled = true;
                cboRole.Enabled = true;
                dtpDob.Enabled = true;
                rdoMale.Enabled = true;
                rdoFemale.Enabled = true;
                btnThem.Enabled = false;
                btnLuu.Enabled = true;
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
                btnHuy.Enabled = true;

                txtUsername.Focus();
            }
            else if (status == "Edit")
            {
                txtID.Enabled = false;  
                txtUsername.Enabled = true;
                txtPassword.Enabled = true;
                cboRole.Enabled = true;
                dtpDob.Enabled = true;
                rdoMale.Enabled = true;
                rdoFemale.Enabled = true;
                btnThem.Enabled = false;
                btnLuu.Enabled = true;
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
                btnHuy.Enabled = true;

                txtUsername.Focus();
            }
        }

        private void GetData()
        {
            using (SqlConnection conn =
                  new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                SqlDataAdapter da =
                    new SqlDataAdapter("SELECT * FROM Account", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvAccount.AutoGenerateColumns = true;
                dgvAccount.DataSource = dt;
            }
        }

        private void btnMain_Click(object sender, EventArgs e)
        {
            Mainframe mainframe = new Mainframe();
            mainframe.Show();
            this.Hide();
        }

        //Thêm dữ liệu
        private void btnThem_Click(object sender, EventArgs e)
        {
            Status = "Add";
            SetInterface(Status);
        }


        //Sửa dữ liệu
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvAccount.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần sửa!");
                return;
            }

            DataGridViewRow row = dgvAccount.SelectedRows[0];
            txtID.Text = row.Cells["id"].Value.ToString();
            txtUsername.Text = row.Cells["Username"].Value.ToString();
            txtPassword.Text = row.Cells["Password"].Value.ToString();
            cboRole.Text = row.Cells["Role"].Value.ToString();
            if (row.Cells["Ngaysinh"].Value != null && row.Cells["Ngaysinh"].Value != DBNull.Value)
            {
                dtpDob.Value = Convert.ToDateTime(row.Cells["Ngaysinh"].Value);
            }
            else
            {
                // Nếu NULL, set giá trị mặc định
                dtpDob.Value = DateTime.Now;
            }

            string gioiTinh = row.Cells["Gioitinh"].Value.ToString();
            if (Gioitinh.Equals("Nam"))
            {
                rdoMale.Checked = true;
            }
            else
            {
                rdoFemale.Checked = true;
            }

            Status = "Edit";
            SetInterface(Status);
        }


        //Xóa dữ liệu
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if(dgvAccount.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần xóa");
                return;
            }

            if(int.TryParse(dgvAccount.Rows[4].Cells[0].Value?.ToString(), out int id))
            {
                // Sử dụng id
            }
            else
            {
                MessageBox.Show("Giá trị không hợp lệ!");
            }

            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn tài khoản này không?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM Account WHERE id = @ID";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@ID", id);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            MessageBox.Show("Xóa tài khoản thành công");
                            GetData();
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy tài khoản để xóa");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }


        //Lưu dữ liệu
        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text.Trim();
                string role = cboRole.Text.Trim();
                DateTime ngaySinh = dtpDob.Value;
                string gioiTinh = rdoMale.Checked ? "Nam" : "Nữ";

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;

                    // THÊM
                    if (Status == "Add")
                    {
                        cmd.CommandText = @"INSERT INTO Account
                        (Username, Password, Role, Ngaysinh, Gioitinh)
                        VALUES (@Username, @Password, @Role, @Ngaysinh, @Gioitinh)";
                    }
                    // SỬA
                    else if (Status == "Edit")
                    {
                        int accountId = Convert.ToInt32(txtID.Text);
                        cmd.CommandText = @"UPDATE Account 
                        SET Username=@Username,
                            Password=@Password,
                            Role=@Role,
                            Ngaysinh=@Ngaysinh,
                            Gioitinh=@Gioitinh
                        WHERE Id=@ID";
                        cmd.Parameters.AddWithValue("@ID", accountId);
                    }

                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@Role", role);
                    cmd.Parameters.AddWithValue("@Ngaysinh", ngaySinh);
                    cmd.Parameters.AddWithValue("@Gioitinh", gioiTinh);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Lưu dữ liệu thành công!");
                GetData();
                Status = "Reset";
                SetInterface(Status);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }


        //Hủy thao tác
        private void btnHuy_Click(object sender, EventArgs e)
        {
            Status = "Reset";
            SetInterface(Status);

        }


        // Hiển thị dữ liệu lên các ô nhập khi click vào dgv
        private void dgvView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvAccount.Rows[e.RowIndex];
                txtUsername.Text = row.Cells["Username"].Value.ToString();
                txtPassword.Text = row.Cells["Password"].Value.ToString();
                cboRole.Text = row.Cells["Role"].Value.ToString();
                dtpDob.Value = Convert.ToDateTime(row.Cells["Ngaysinh"].Value);
                string gioiTinh = row.Cells["Gioitinh"].Value.ToString();
                if (Gioitinh.Equals("Nam"))
                {
                    rdoMale.Checked = true;
                }
                else if (Gioitinh.Equals("Nữ"))
                {
                    rdoFemale.Checked = true;
                }
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            String keyword = txtSearch.Text.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                GetData();
                return;
            }
            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Account WHERE Username LIKE @keyword OR Role LIKE @keyword ";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
                ds.Clear();
                da.Fill(ds);
                dgvAccount.AutoGenerateColumns = false;
                dgvAccount.DataSource = ds.Tables[0];
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Supplier supplier = new Supplier();
            supplier.Show();
            this.Hide();
        }
    }
}
