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
    public partial class frmPos : Form
    {
        public frmPos()
        {
            InitializeComponent();
        }
        SqlConnection conn;
        SqlTransaction tr;

        public object Comm { get; private set; }

        private void frmPos_Load(object sender, EventArgs e)
        {
            conn = ConnectDB.ConnectMinimart();
            ListViewFormat();
            ClearProductData();
        }

        private void ListViewFormat()
        {
            lsvProduct.Columns.Add("รหัสสินค้า", 100, HorizontalAlignment.Left);
            lsvProduct.Columns.Add("สินค้า", 150, HorizontalAlignment.Left);
            lsvProduct.Columns.Add("ราคา", 65, HorizontalAlignment.Right);
            lsvProduct.Columns.Add("จำนวน", 50, HorizontalAlignment.Right);
            lsvProduct.Columns.Add("รวมเป็นเงิน", 70, HorizontalAlignment.Right);
            lsvProduct.View = View.Details;
            lsvProduct.GridLines = true;
            lsvProduct.FullRowSelect = true;
        }
        private void ClearProductData()
        {
            txtProductID.Text = "";
            txtProductName.Text = "";
            txtUnitPrice.Text = "0";
            txtQuantity.Text = "1";
            txtTotal.Text = "0";
            txtProductID.Focus();
        }
        private void ClearEmployeeData()
        {
            txtEmployeeID.Text = "";
            txtEmployeeName.Text = "";
            txtEmployeeID.Focus();
        }
        private void calculateTotal()
        {
            double total = Convert.ToDouble(txtUnitPrice.Text) * Convert.ToDouble(txtQuantity.Text);
            txtTotal.Text = total.ToString();
            txtProductID.Focus();
        }
        private void calulateNetPrice()
        {
            int i = 0;
            double tmpNetPrice = 0.0;
            for (i = 0; i < lsvProduct.Items.Count; i++)
            {
                tmpNetPrice += Convert.ToDouble(lsvProduct.Items[i].SubItems[4].Text);
            }
            lblNetPrice.Text = tmpNetPrice.ToString("#,##0.00");
        }

        private void txtEmployeeID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) 
            {
                string sql = "select EmployeeID, Title + FirstName + Space(2) + LastName as empName, Position from Employees where EmployeeID = @EmployeeID";

                SqlCommand comm = new SqlCommand(sql, conn);
                comm.Parameters.AddWithValue("@EmployeeID", txtEmployeeID.Text);
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                SqlDataReader dr = comm.ExecuteReader();
                if (dr.HasRows)
                {
                    DataTable dt = new DataTable();
                    dt.Load(dr);
                    txtEmployeeName.Text = dt.Rows[0][1].ToString();
                    txtEmployeeID.Text = dt.Rows[0][0].ToString();
                    txtProductID.Focus();
                }
                else
                {
                    MessageBox.Show("ไม่พบข้อมูลพนักงานรหัส " + txtEmployeeID.Text, "ERROR");
                    ClearEmployeeData();
                    txtEmployeeName.Focus();
                }
                dr.Close();
                conn.Close();
            }
        }

        private void txtProductID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) 
            {
                string sql = "select ProductID,ProductName,UnitPrice from Products where ProductID=@ProductID";
                SqlCommand comm = new SqlCommand(sql, conn);
                comm.Parameters.AddWithValue("@ProductID", txtProductID.Text);
                conn.Open();
                SqlDataReader dr = comm.ExecuteReader();
                if (dr.HasRows) 
                {
                    DataTable dt = new DataTable();
                    dt.Load(dr);
                    txtProductID.Text = dt.Rows[0][0].ToString();
                    txtProductName.Text = dt.Rows[0][1].ToString();
                    txtUnitPrice.Text = dt.Rows[0][2].ToString();
                    calculateTotal();
                    txtProductID.Focus();
                }
                else
                {
                    MessageBox.Show("ไม่พบข้อมูลสินค้ารหัส " + txtProductID.Text, "ERROR");
                    ClearProductData();
                    txtProductID.Focus();
                }
                dr.Close();
                conn.Close();
            }
        }
        

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtProductID.Text.Trim() == "" || txtProductName.Text.Trim() == "")
            {
                txtProductID.Focus();return;
            }
            if (Convert.ToInt16(txtQuantity.Text) == 0)
            {
                txtQuantity.Focus(); return;
            }
            ListViewItem lvi;
            int i = 0; string tmpProductID = "";
            for (i = 0; i < lsvProduct.Items.Count - 1; i++) 
            {
                tmpProductID = lsvProduct.Items[i].SubItems[0].Text;
                if (tmpProductID == txtProductID.Text)
                {
                    int qty = Convert.ToInt16(lsvProduct.Items[i].SubItems[3].Text)
                                + Convert.ToInt16(txtQuantity.Text);
                    double newTotal = Convert.ToDouble(lsvProduct.Items[i].SubItems[4].Text)
                    + Convert.ToDouble(txtTotal.Text); //**
                    lsvProduct.Items[i].SubItems[3].Text = qty.ToString();
                    lsvProduct.Items[i].SubItems[4].Text = newTotal.ToString("#,##0.00");
                    ClearProductData();
                    calculateTotal();
                    calulateNetPrice();
                    return;
                }
            }
            string[] anyData;
            anyData = new string[] { txtProductID.Text, txtProductName.Text,
                        txtUnitPrice.Text, txtQuantity.Text, txtTotal.Text };
            lvi = new ListViewItem(anyData);
            lsvProduct.Items.Add(lvi);
            calulateNetPrice(); ClearProductData(); btnSave.Enabled = true;
            txtProductID.Focus();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearProductData();
        }

        private void lsvProduct_DoubleClick(object sender, EventArgs e)
        {
            int i = 0;
            for (i = 0; i <= lsvProduct.SelectedItems.Count - 1; i++)
            {
                ListViewItem lvi = lsvProduct.SelectedItems[i];
                lsvProduct.Items.Remove(lvi);
            }
            calulateNetPrice();
            txtProductID.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            lsvProduct.Clear(); 
            ClearEmployeeData();
            ClearProductData();
            ListViewFormat(); 
            lblNetPrice.Text = "0.00"; 
            txtEmployeeID.Focus(); 
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string msg = "";
            int lastOrderID = 0; //จะเอําไว้เก็บรหัสที่ใหม่ที่สุดตอนที่ insert order แล ้ว
            if (txtEmployeeID.Text.Trim() == "")
            {
                MessageBox.Show("โปรดระบุผู้ขายสินค้าก่อน", "มีข้อผิดพลาด");
                txtEmployeeID.Focus();
                return;
            }
            if (lsvProduct.Items.Count > 0) //ตรวจสอบวํา่ เลอื กสนิคํา้ไวห้ รอื ยัง
            {
                if (MessageBox.Show("ต้องการบันบึกรายการสั่งซื้อหรือไม่", "กรุณายืนยัน", MessageBoxButtons.YesNo)
                == DialogResult.Yes)
                {
                    //ประกําศเริ่ม Transaction

                    conn.Open();
                    tr = conn.BeginTransaction();
                    string sql = "insert into Receipts(ReceiptDate,EmployeeID,TotalCash)"
                    + " values (getdate(),@EmployeeID,@TotalCash)";
                    SqlCommand comm = new SqlCommand(sql, conn, tr);
                    comm.Parameters.AddWithValue("@EmployeeID", txtEmployeeID.Text.Trim());
                    comm.Parameters.AddWithValue("@TotalCash", lblNetPrice.Text);
                    comm.ExecuteNonQuery();
                    //อ่ําน OrderID ลํา่ สดุ ใสไ่ วใ้นตัวแปร lastOrderID
                    string sql1 = "select top 1 ReceiptID from Receipts order by ReceiptID desc";
                    SqlCommand comm1 = new SqlCommand(sql1, conn, tr);
                    SqlDataReader dr = comm1.ExecuteReader();
                    if (dr.HasRows)
                    {
                        dr.Read();
                        lastOrderID = dr.GetInt32(dr.GetOrdinal("ReceiptID"));
                    }
                    dr.Close();
                    msg += "ผู้ขาย: " + txtEmployeeName.Text + Environment.NewLine;
                    msg += "หมายเลขใบสั่งซื้อ: " + lastOrderID.ToString() + Environment.NewLine;
                    //เพมิ่ ขอ้มลู รํายกํารสนิคํา้ OrderDetail ที่ตรงกับ lastOrderID
                    for (int i = 0; i <= lsvProduct.Items.Count - 1; i++)
                    {
                        string sql2 = "insert into Details(ReceiptID,ProductID,UnitPrice,Quantity)"
                        + " values(@ReceiptID,@ProductID,@UnitPrice,@Quantity)";
                        SqlCommand comm3 = new SqlCommand(sql2, conn, tr);
                        comm3.Parameters.AddWithValue("@ReceiptID", lastOrderID);
                        comm3.Parameters.AddWithValue("@ProductID", lsvProduct.Items[i].SubItems[0].Text);
                        comm3.Parameters.AddWithValue("@UnitPrice", lsvProduct.Items[i].SubItems[2].Text);
                        comm3.Parameters.AddWithValue("@Quantity", lsvProduct.Items[i].SubItems[3].Text);
                        comm3.ExecuteNonQuery();
                        msg += lsvProduct.Items[i].SubItems[0].Text + ", ";
                        msg += lsvProduct.Items[i].SubItems[1].Text + ", ";
                        msg += lsvProduct.Items[i].SubItems[2].Text + ", ";
                        msg += lsvProduct.Items[i].SubItems[3].Text + ", " + Environment.NewLine;
                    }
                    tr.Commit();
                    conn.Close();
                    msg += "\nยอดรวมทั้งหมด: " + lblNetPrice.Text;
                    MessageBox.Show(msg, "บันทึกรายการขายเรียบร้อยแล้ว");
                }
                btnCancel.PerformClick(); //สั่งใหไ้ปกดป่มุ cancel เคลีย์หน้ําจอทั้งหมดใหม่เพื่อเริ่มรํายกํารใหม่
            }
        }

        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            if (txtQuantity.Text.Trim() == "") txtQuantity.Text = "1";

            if (Convert.ToInt16(txtQuantity.Text) == 0) txtQuantity.Text = "1";
            calculateTotal();

        }
    }
 }

