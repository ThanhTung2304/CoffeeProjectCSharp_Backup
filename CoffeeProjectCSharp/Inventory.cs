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
    public partial class Inventory : Form
    {
        public Inventory()
        {
            InitializeComponent();

            LoadInventory();
            LoadHistory();
        }

        //Load Tồn kho
        private void LoadInventory()
        {
            using (SqlConnection conn =
                new SqlConnection(ConfigDB.connectionString))
            {
                string sql = @"
                SELECT 
                    p.Id,
                    p.Name,
                    ISNULL(i.Quantity, 0) AS Quantity
                FROM Product p
                LEFT JOIN Inventory i ON p.Id = i.Product_Id";

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvInventory.AutoGenerateColumns = true;
                dgvInventory.DataSource = dt;
            }
        }

        // Load Lịch sử
        private void LoadHistory()
        {
            using (SqlConnection conn =
                new SqlConnection(ConfigDB.connectionString))
            {
                string sql = @"
                SELECT 
                    h.Id,
                    p.Name AS ProductName,
                    h.Quantity_Change,
                    h.Action,
                    h.Note,
                    h.CreatedTime
                FROM Inventory_History h
                JOIN Product p ON h.Product_Id = p.Id
                ORDER BY h.CreatedTime DESC";

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvHistory.AutoGenerateColumns = true;
                dgvHistory.DataSource = dt;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btnMain_Click(object sender, EventArgs e)
        {
            Mainframe main = new Mainframe();
            main.Show();
            this.Hide();
        }

        private void dgvImport_Click(object sender, EventArgs e)
        {
            if (dgvInventory.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chọn sản phẩm");
                return;
            }

            if (!int.TryParse(txtQuantity.Text.Trim(), out int qty) || qty <= 0)
            {
                MessageBox.Show("Số lượng không hợp lệ");
                return;
            }

            int productId = Convert.ToInt32(
                dgvInventory.SelectedRows[0].Cells["Id"].Value);

            using (SqlConnection conn =
                new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();

                // 1️⃣ UPDATE tồn kho
                SqlCommand cmd = new SqlCommand(@"
            UPDATE Inventory
            SET Quantity = Quantity + @qty
            WHERE Product_Id = @pid
        ", conn);

                cmd.Parameters.AddWithValue("@pid", productId);
                cmd.Parameters.AddWithValue("@qty", qty);

                // nếu chưa có dòng inventory → tạo mới
                if (cmd.ExecuteNonQuery() == 0)
                {
                    SqlCommand insertInv = new SqlCommand(@"
                INSERT INTO Inventory(Product_Id, Quantity)
                VALUES(@pid, @qty)
            ", conn);

                    insertInv.Parameters.AddWithValue("@pid", productId);
                    insertInv.Parameters.AddWithValue("@qty", qty);
                    insertInv.ExecuteNonQuery();
                }

                // 2️⃣ GHI LỊCH SỬ
                SqlCommand his = new SqlCommand(@"
            INSERT INTO Inventory_History
            (Product_Id, Quantity_Change, Action, Note)
            VALUES(@pid, @qty, 'IMPORT', @note)
        ", conn);

                his.Parameters.AddWithValue("@pid", productId);
                his.Parameters.AddWithValue("@qty", qty);
                his.Parameters.AddWithValue("@note", txtNote.Text.Trim());
                his.ExecuteNonQuery();
            }

            LoadInventory();
            LoadHistory();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (dgvInventory.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chọn sản phẩm");
                return;
            }

            if (!int.TryParse(txtQuantity.Text.Trim(), out int qty) || qty <= 0)
            {
                MessageBox.Show("Số lượng không hợp lệ");
                return;
            }

            int productId = Convert.ToInt32(
                dgvInventory.SelectedRows[0].Cells["Id"].Value);

            int currentQty = Convert.ToInt32(
                dgvInventory.SelectedRows[0].Cells["Quantity"].Value);

            // Kiểm ra số lượng xuất
            if (qty > currentQty)
            {
                MessageBox.Show("Số lượng xuất vượt quá tồn kho");
                return;
            }

            using (SqlConnection conn =
                new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
            UPDATE Inventory
            SET Quantity = Quantity - @qty
            WHERE Product_Id = @pid
        ", conn);

                cmd.Parameters.AddWithValue("@pid", productId);
                cmd.Parameters.AddWithValue("@qty", qty);
                cmd.ExecuteNonQuery();

                SqlCommand his = new SqlCommand(@"
            INSERT INTO Inventory_History
            (Product_Id, Quantity_Change, Action, Note)
            VALUES(@pid, -@qty, 'EXPORT', @note)
        ", conn);

                his.Parameters.AddWithValue("@pid", productId);
                his.Parameters.AddWithValue("@qty", qty);
                his.Parameters.AddWithValue("@note", txtNote.Text.Trim());
                his.ExecuteNonQuery();
            }

            LoadInventory();
            LoadHistory();
        }

        private void btnClearHistory_Click(object sender, EventArgs e)
        {
            if (dgvHistory.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chọn lịch sử cần xóa");
                return;
            }

            int historyId = Convert.ToInt32(
                dgvHistory.SelectedRows[0].Cells["Id"].Value);

            if (MessageBox.Show(
                "Xóa lịch sử này?\n(Tồn kho sẽ KHÔNG bị ảnh hưởng)",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            using (SqlConnection conn =
                new SqlConnection(ConfigDB.connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
            DELETE FROM Inventory_History
            WHERE Id = @id
        ", conn);

                cmd.Parameters.AddWithValue("@id", historyId);
                cmd.ExecuteNonQuery();
            }

            LoadHistory();
        }

        private void btnXuat_Click(object sender, EventArgs e)
        {
            if (dgvInventory.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel (*.csv)|*.csv";
            sfd.FileName = "TonKhoHienTai.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw =
                        new StreamWriter(sfd.FileName, false, Encoding.UTF8))
                    {
                        // ====== HEADER ======
                        for (int i = 0; i < dgvInventory.Columns.Count; i++)
                        {
                            sw.Write(dgvInventory.Columns[i].HeaderText);
                            if (i < dgvInventory.Columns.Count - 1)
                                sw.Write(",");
                        }
                        sw.WriteLine();

                        // ====== DATA ======
                        foreach (DataGridViewRow row in dgvInventory.Rows)
                        {
                            if (row.IsNewRow) continue;

                            for (int i = 0; i < dgvInventory.Columns.Count; i++)
                            {
                                sw.Write(row.Cells[i].Value?.ToString());
                                if (i < dgvInventory.Columns.Count - 1)
                                    sw.Write(",");
                            }
                            sw.WriteLine();
                        }
                    }

                    MessageBox.Show("Xuất Excel tồn kho thành công!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất file: " + ex.Message);
                }
            }
        }
    }
}
