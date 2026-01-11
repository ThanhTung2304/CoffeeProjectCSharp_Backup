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
    public partial class Recipe : Form
    {
        public Recipe()
        {
            InitializeComponent();
            LoadProductsToCombo();  
        }
        //Đổ  danh sách sản phẩm vào combobox
        private void LoadProductsToCombo()
        {
            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT Id, Name FROM Product WHERE IsActive = 1", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cboProduct.DataSource = dt;
                cboProduct.DisplayMember = "Name";
                cboProduct.ValueMember = "Id";
                cboProduct.SelectedIndex = -1;
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            //RecipeItemForm recipeItemForm = new RecipeItemForm();
            //recipeItemForm.Show();
            //this.Hide();
            //Kiểm tra xem người dùng đã chọn sản phẩm ở ComboBox chưa
            if (cboProduct.SelectedValue == null || cboProduct.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm trước khi thêm công thức!");
                return;
            }

            // Khởi tạo form phụ
            RecipeItemForm recipeItemForm = new RecipeItemForm();

            //Truyền dữ liệu sang form phụ trước khi hiển thị
            recipeItemForm.Status = "Add";
            recipeItemForm.SelectedProductId = Convert.ToInt32(cboProduct.SelectedValue);

            // Hiển thị dưới dạng Dialog (Form chính sẽ đợi cho đến khi Form phụ đóng)
            if (recipeItemForm.ShowDialog() == DialogResult.OK)
            {
                // Nếu lưu thành công (DialogResult.OK), tự động load lại lưới
                LoadRecipeData();
            }
        }

        private void btnMain_Click(object sender, EventArgs e)
        {
            Mainframe main = new Mainframe();
            main.Show();
            this.Hide();
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            if (cboProduct.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm!");
                return;
            }

            LoadRecipeData();
        }
        private void LoadRecipeData()
        {
            int productId = Convert.ToInt32(cboProduct.SelectedValue);
            using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();
                string sql = "SELECT Id, Ingredient AS [Nguyên liệu], Quantity AS [Số lượng], Unit AS [Đơn vị] " +
                             "FROM Recipe WHERE Product_Id = @pid";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@pid", productId);

                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvRecipe.DataSource = dt;
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvRecipe.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn nguyên liệu cần xóa!");
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa nguyên liệu này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dgvRecipe.SelectedRows[0].Cells["Id"].Value);
                using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM recipe WHERE Id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
                LoadRecipeData();
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra đã chọn dòng ở lưới chưa
            if (dgvRecipe.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn nguyên liệu cần sửa!");
                return;
            }

            // 2. Khởi tạo form phụ
            RecipeItemForm f = new RecipeItemForm();
            f.Status = "Edit";

            // 3. Lấy dữ liệu từ dòng đang chọn
            DataGridViewRow row = dgvRecipe.SelectedRows[0];
            f.SelectedRecipeId = Convert.ToInt32(row.Cells["Id"].Value);
            f.SelectedProductId = Convert.ToInt32(cboProduct.SelectedValue);

            // 4. Đổ dữ liệu vào các ô của form phụ (Sau khi đã chỉnh Modifiers thành Public)
            f.txtNguyenLieu.Text = row.Cells["Nguyên liệu"].Value.ToString();
            f.txtSoLuong.Text = row.Cells["Số lượng"].Value.ToString();
            f.txtDonVi.Text = row.Cells["Đơn vị"].Value.ToString();

            if (f.ShowDialog() == DialogResult.OK)
            {
                LoadRecipeData();
            }
        }
    }
}
