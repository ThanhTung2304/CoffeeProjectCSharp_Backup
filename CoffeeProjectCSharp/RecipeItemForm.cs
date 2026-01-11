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
    public partial class RecipeItemForm : Form
    {
        // Các biến trạng thái cơ bản
        public string Status = "Reset";
        public int SelectedProductId = 0; // ID sản phẩm cha
        public int SelectedRecipeId = 0;  // ID của dòng công thức (dùng khi Sửa)
        public RecipeItemForm()
        {
            InitializeComponent();
        }

        private void btnMain_Click(object sender, EventArgs e)
        {
            Mainframe main = new Mainframe();
            main.Show();
            this.Hide();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNguyenLieu.Text) || string.IsNullOrWhiteSpace(txtSoLuong.Text))
            {
                MessageBox.Show("Vui lòng nhập đủ thông tin!");
                return;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigDB.connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;

                    if (Status == "Add")
                    {
                        // Sửa tên cột: ingredient_name -> Ingredient, amount -> Quantity
                        cmd.CommandText = @"INSERT INTO Recipe (Product_Id, Ingredient, Quantity, Unit) 
                                    VALUES (@pid, @name, @qty, @unit)";
                    }
                    else
                    {
                        // Sửa tên cột cho câu lệnh Update
                        cmd.CommandText = @"UPDATE Recipe SET Ingredient=@name, Quantity=@qty, Unit=@unit 
                                    WHERE Id=@rid";
                        cmd.Parameters.AddWithValue("@rid", SelectedRecipeId);
                    }

                    cmd.Parameters.AddWithValue("@pid", SelectedProductId);
                    cmd.Parameters.AddWithValue("@name", txtNguyenLieu.Text.Trim());

                    // Ép kiểu số an toàn cho Quantity
                    decimal qty = 0;
                    decimal.TryParse(txtSoLuong.Text, out qty);
                    cmd.Parameters.AddWithValue("@qty", qty);

                    cmd.Parameters.AddWithValue("@unit", txtDonVi.Text.Trim());

                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Lưu thành công!");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi SQL: " + ex.Message);
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
