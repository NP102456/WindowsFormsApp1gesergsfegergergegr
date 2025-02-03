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

namespace WindowsFormsApp1gesergsfegergergegr
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load1;
        }
        SqlConnection conn;
        SqlDataAdapter da;
        SqlCommand com;
        private void Form1_Load1(object sender, EventArgs e)
        {
           conn = ConnectDB.ConnectMinimart();
            showdata();
        }

        private void showdata()
        {
            string sql = "select * from Categories";
            com = new SqlCommand(sql, conn);
            da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvCategories.DataSource = dt;
        }

        private void dgvCategories_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            txtCategoryID.Text = dgvCategories.CurrentRow.Cells["categoryID"].Value.ToString();
            txtCategoryName.Text = dgvCategories.CurrentRow.Cells["categoryName"].Value.ToString();
            txtDescription.Text = dgvCategories.CurrentRow.Cells["description"].Value.ToString();
        }

        private void btnClearForm_Click(object sender, EventArgs e)
        {
            txtCategoryID.Text = "";
            txtCategoryName.Text = "";
            txtDescription.Text = "";
            txtCategoryName.Focus();
            txtCategoryID.Enabled = false;
        }

        private void btnInserf_Click(object sender, EventArgs e)
        {
           // btnClearForm.PerformClick();

            if(string.IsNullOrEmpty(txtCategoryName.Text))
            {
                MessageBox.Show("ชื่อหมวดหมู่ต้องไม่ว่าง","ERROR");
                txtCategoryName.Focus();
                return;
            }
            string sql = "Insert into categories values(@categoryName,@description)";
            com = new SqlCommand(sql, conn);
            com.Parameters.AddWithValue("@categoryName", txtCategoryName.Text);
            com.Parameters.AddWithValue("@description", txtDescription.Text);
            if (com.ExecuteNonQuery() > 0)
            {
                showdata();
                btnClearForm.PerformClick();
            }
            
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCategoryID.Text))
            {
                MessageBox.Show("กรุณาเลือกข้อมูลที่ต้องการแก้ไข", "ERROR");
                return;
            }
            if (string.IsNullOrEmpty(txtCategoryName.Text))
            {
                MessageBox.Show("ชื่อหมวดหมู่ต้องไม่ว่าง", "ERROR");
                txtCategoryName.Focus();
                return;
            }
            string sql = "Update Categories set CategoryName = @categoryName, Description = @Description where CategoryID = @categoryID";


            com = new SqlCommand(sql, conn);
            com.Parameters.AddWithValue("@categoryName", txtCategoryName.Text);
            com.Parameters.AddWithValue("@description", txtDescription.Text);
            com.Parameters.AddWithValue("@categoryID", txtCategoryID.Text);
            if (com.ExecuteNonQuery() > 0)
            {
                showdata();
                btnClearForm.PerformClick();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCategoryID.Text))
            {
                MessageBox.Show("ต้องเลือกข้อมูลที่ต้องการลบก่อน", "เกิดข้อผิดพลาด");
                return;
            }
            if (MessageBox.Show("ต้องการลบหรือไม่", "โปรดยืนยัน", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }
            string sql = "DELETE FROM Categories WHERE CategoryID = @categoryID";
            com = new SqlCommand(sql, conn);
            com.Parameters.AddWithValue("@categoryID", txtCategoryID.Text.Trim());
            try
            {
                if (com.ExecuteNonQuery() > 0)
                {
                    showdata();
                    btnClearForm.PerformClick();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดำลาด :" + Environment.NewLine + ex.Message, "ไม่สามารถลบข้อมูลได้");
            }

        }
    }
}
