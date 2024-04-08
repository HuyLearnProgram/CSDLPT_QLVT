﻿using DevExpress.XtraGrid;
using QLVT.DS1TableAdapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLVT
{
    public partial class FormPhieuXuat : Form
    {
        public string makho = "";
        string maChiNhanh = "";
        String brandId = "";
        String cheDo = "PX";
        int position = 0;
        bool isAdding = false;

        //Undo -> dùng để hoàn tác dữ liệu nếu lỡ có thao tác không mong muốn
        Stack undoList = new Stack();

        BindingSource bds = null;
        GridControl gc = null;
        string type = "";
        private Dictionary<int, Dictionary<string, object>> previousRowDataDict = new Dictionary<int, Dictionary<string, object>>();
        private Dictionary<string, Dictionary<int, Dictionary<string, object>>> previousCTPX = new Dictionary<string, Dictionary<int, Dictionary<string, object>>>();

        private Form CheckExists(Type ftype)
        {
            foreach (Form f in this.MdiChildren)
                if (f.GetType() == ftype)
                    return f;
            return null;
        }
        private void ThongBao(String mess)
        {
            MessageBox.Show(mess, "Thông báo", MessageBoxButtons.OK);
        }
        public FormPhieuXuat()
        {
            InitializeComponent();
        }

        private void phieuXuatBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsPhieuXuat.EndEdit();
            this.tableAdapterManager.UpdateAll(this.dS1);

        }
        private void FormPhieuXuat_Load(object sender, EventArgs e)
        {
            //Không cần kiểm tra khóa ngoại
            dS1.EnforceConstraints = false;

            // TODO: This line of code loads data into the 'dS1.DSNV' table. You can move, or remove it, as needed.
            this.dSNVTableAdapter.Connection.ConnectionString = Program.conStr;
            this.dSNVTableAdapter.Fill(this.dS1.DSNV);
            // TODO: This line of code loads data into the 'dS1.DSKHO' table. You can move, or remove it, as needed.
            this.dSKHOTableAdapter.Connection.ConnectionString = Program.conStr;
            this.dSKHOTableAdapter.Fill(this.dS1.DSKHO);
            // TODO: This line of code loads data into the 'dS1.Vattu' table. You can move, or remove it, as needed.
            this.vattuTableAdapter.Connection.ConnectionString = Program.conStr;
            this.vattuTableAdapter.Fill(this.dS1.Vattu);
            // TODO: This line of code loads data into the 'dS1.CTPX' table. You can move, or remove it, as needed.
            this.cTPXTableAdapter.Connection.ConnectionString = Program.conStr;
            this.cTPXTableAdapter.Fill(this.dS1.CTPX);
            // TODO: This line of code loads data into the 'dS1.PhieuXuat' table. You can move, or remove it, as needed.
            this.phieuXuatTableAdapter.Connection.ConnectionString = Program.conStr;
            this.phieuXuatTableAdapter.FillBy(this.dS1.PhieuXuat);
            
            cbChiNhanh.DataSource = Program.bindingSource;
            cbChiNhanh.DisplayMember = "TENCN";
            cbChiNhanh.ValueMember = "TENSERVER";
            cbChiNhanh.SelectedIndex = Program.brand;

            /*brandId = ((DataRowView)bdsPhieuNhap[0])["MACN"].ToString();*/
            //Phân quyền nhóm CONGTY chỉ được xem dữ liệu
            if (Program.mGroup == "CONGTY")
            {
                btnThem.Enabled = false;
                btnGhi.Enabled = false;
                btnXoa.Enabled = false;
                btnHoanTac.Enabled = false;
                groupBoxPhieuXuat.Enabled = false;
            }

            //Phân quyền nhóm CHINHANH-USER có thể thao tác với dữ liệu
            //Nhưng không được quyền chuyển chi nhánh khác để xem dữ liệu
            if (Program.mGroup == "CHINHANH" || Program.mGroup == "USER")
            {
                cbChiNhanh.Enabled = false;
            }
        }
        private void hOTENComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (hOTENComboBox.SelectedValue == null) return;
            try
            {
                txtMANV.Text = hOTENComboBox.SelectedValue.ToString();
            }
            catch (Exception) { }
        }
        private void tENKHOComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tENKHOComboBox.SelectedValue == null) return;
            try
            {
                txtMAKHO.Text = tENKHOComboBox.SelectedValue.ToString();
            }
            catch (Exception) { }
        }
        private void cbChiNhanh_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbChiNhanh.SelectedValue.ToString() == "System.Data.DataRowView")
                return;

            Program.servername = cbChiNhanh.SelectedValue.ToString();

            /*Neu chon sang chi nhanh khac voi chi nhanh hien tai*/
            if (cbChiNhanh.SelectedIndex != Program.brand)
            {
                // Dùng tài khoản hỗ trợ kết nối để chuẩn bị cho việc login vào chi nhánh khác
                Program.mlogin = Program.remotelogin;
                Program.password = Program.remotepassword;
            }
            /*Neu chon trung voi chi nhanh dang dang nhap o formDangNhap*/
            else
            {
                // Lấy tài khoản hiện tại đang đăng xuất để đăng xuất
                Program.mlogin = Program.mloginDN;
                Program.password = Program.passwordDN;
            }
            if (Program.connectDB() == 0)
            {
                MessageBox.Show("Xảy ra lỗi kết nối với chi nhánh hiện tại", "Thông báo", MessageBoxButtons.OK);
            }
            else
            {
                this.dSNVTableAdapter.Connection.ConnectionString = Program.conStr;
                this.dSNVTableAdapter.Fill(this.dS1.DSNV);
                this.dSKHOTableAdapter.Connection.ConnectionString = Program.conStr;
                this.dSKHOTableAdapter.Fill(this.dS1.DSKHO);
                this.vattuTableAdapter.Connection.ConnectionString = Program.conStr;
                this.vattuTableAdapter.Fill(this.dS1.Vattu);
                this.cTPXTableAdapter.Connection.ConnectionString = Program.conStr;
                this.cTPXTableAdapter.Fill(this.dS1.CTPX);
                this.phieuXuatTableAdapter.Connection.ConnectionString = Program.conStr;
                this.phieuXuatTableAdapter.FillBy(this.dS1.PhieuXuat);
            }
        }
        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Lấy vị trí của con trỏ
            position = bdsPhieuXuat.Position;
            cheDo = "PX";
            isAdding = true;
            groupBoxPhieuXuat.Enabled = true;

            bdsPhieuXuat.AddNew();
            txtMAPX.Enabled = true;
            dteNGAY.EditValue = DateTime.Now;
            dteNGAY.Enabled = false;
            hOTENComboBox.Enabled = true;
            /*txtMANV.Text = "";*/
            tENKHOComboBox.Enabled = true;

            // Thay đổi bật/ tắt các nút chức năng
            btnThem.Enabled = false;
            btnXoa.Enabled = false;

            btnLamMoi.Enabled = false;
            btnChitietPX.Enabled = false;
            btnThoat.Enabled = true;

            phieuXuatGridControl.Enabled = false;
            groupBoxPhieuXuat.Enabled = true;
            dgvCTPX.Enabled = false;
            contextMenuStripCTPX.Enabled = false;
        }
        private void btnGhi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Lấy dữ liệu trước khi ghi phục vụ cho việc hoàn tác
            String maPX = txtMAPX.Text.Trim();
            DataRowView dr = ((DataRowView)bdsPhieuXuat[bdsPhieuXuat.Position]);
            DateTime ngayLap = new DateTime();
            if (dr["NGAY"] != DBNull.Value)
            {
                if (DateTime.TryParse(dr["NGAY"].ToString(), out ngayLap))
                {
                }
                else
                {
                    // Xử lý trường hợp không thể chuyển đổi ngày
                    MessageBox.Show("Giá trị trong cột NGAY không phải là kiểu ngày tháng hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            String maNV = dr["MANV"].ToString();
            String maKho = dr["MAKHO"].ToString();
            String hotenKH = dr["HOTENKH"].ToString();

            DialogResult dlr = MessageBox.Show("Bạn có chắc chắn muốn ghi dữ liệu vào cơ sở dữ liệu không?", "Thông báo",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dlr == DialogResult.OK)
            {
                try
                {
                    // Lưu truy vấn phục vụ hoàn tác
                    string undoQuery = "";
                    if (cheDo.Equals("PX"))
                    {
                        // Trường hợp thêm phiếu xuất
                        if (isAdding == true)
                        {
                            undoQuery = "DELETE FROM CTPX WHERE MAPX = N'" + maPX + "'\r\nDELETE FROM PhieuXuat WHERE MAPX= N'" + maPX + "'";
                        }
                        // Trường hợp sửa phiếu xuất
                        else
                        {
                            undoQuery = "UPDATE DBO.PhieuXuat " +
                            "SET " +
                            "MANV = N'" + maNV + "'," +
                            "MAKHO = N'" + maKho + "'," +
                            "NGAY = CAST('" + ngayLap.ToString("yyyy-MM-dd") + "' AS DATETIME)," +
                            "HOTENKH = N'" + hotenKH + "' " +
                            "WHERE MAPX = N'" + maPX + "'";
                        }
                        this.bdsPhieuXuat.EndEdit();
                        this.phieuXuatTableAdapter.Update(this.dS1.PhieuXuat);
                        this.phieuXuatTableAdapter.Connection.ConnectionString = Program.conStr;
                        this.phieuXuatTableAdapter.FillBy(this.dS1.PhieuXuat);

                    }
                    else if (cheDo.Equals("CTPX"))
                    {
                        previousCTPX[maPX] = previousRowDataDict;
                        //Lấy dữ liệu để hoàn tác cho CTPX
                        String maPhieuXuat = previousCTPX[maPX][position]["MAPX"].ToString().Trim();
                        String maVT = previousCTPX[maPX][position]["MAVT"].ToString().Trim();
                        if (isAdding == true)
                        {
                            DataRowView drCTPX = (DataRowView)bdsCTPX[position];
                            undoQuery = "DELETE FROM DBO.CTPX " +
                            "WHERE MAPX = N'" + drCTPX["MAPX"].ToString() + "' " +
                            "AND MAVT = N'" + drCTPX["MAVT"] + "'";

                        }
                        else
                        {
                            undoQuery = "UPDATE dbo.CTPX\r\n" +
                                "SET SOLUONG = CAST(" + previousRowDataDict[position]["SOLUONG"] + " AS INT), DONGIA = CAST(" + previousRowDataDict[position]["DONGIA"] + " AS float)\r\n" +
                                "WHERE MAPX = N'" + maPhieuXuat + "' AND MAVT = N'" + maVT + "'";

                        }
                        previousRowDataDict.Remove(position);
                        this.bdsCTPX.EndEdit();
                        this.cTPXTableAdapter.Update(this.dS1.CTPX);
                        this.phieuXuatTableAdapter.Connection.ConnectionString = Program.conStr;
                        this.phieuXuatTableAdapter.Fill(this.dS1.PhieuXuat);
                    }

                    /*cập nhật lại trạng thái thêm mới cho chắc*/
                    isAdding = false;
                    ThongBao("Ghi thành công.");
                    undoList.Push(undoQuery);
                }
                catch (Exception ex)
                {
                    if (cheDo.Equals("PX"))
                    {
                        bdsPhieuXuat.RemoveCurrent();
                        this.phieuXuatTableAdapter.Connection.ConnectionString = Program.conStr;
                        this.phieuXuatTableAdapter.FillBy(this.dS1.PhieuXuat);
                    }
                    else
                    {
                        bdsCTPX.RemoveCurrent();
                        this.cTPXTableAdapter.Connection.ConnectionString = Program.conStr;
                        this.cTPXTableAdapter.Fill(this.dS1.CTPX);
                        phieuXuatGridControl.Enabled = true;
                    }

                    MessageBox.Show("Thất bại. Vui lòng kiểm tra lại!\n" + ex.Message, "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    txtMAPX.Enabled = false;
                    dteNGAY.Enabled = true;
                    // Thay đổi bật/ tắt các nút chức năng
                    btnThem.Enabled = true;
                    btnXoa.Enabled = true;
                    btnGhi.Enabled = true;
                    btnHoanTac.Enabled = true;
                    btnLamMoi.Enabled = true;
                    btnThoat.Enabled = true;
                    btnChitietPX.Enabled = true;


                    dgvCTPX.Enabled = true;
                    contextMenuStripCTPX.Enabled = true;

                    phieuXuatGridControl.Enabled = true;
                    groupBoxPhieuXuat.Enabled = true;
                }
            }
        }
        private void btnThoat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }
        private void btnLamMoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                phieuXuatTableAdapter.FillBy(this.dS1.PhieuXuat);
                phieuXuatGridControl.Enabled = true;
            }
            catch (Exception ex)
            {
                ThongBao("Lỗi khi làm mới dữ liệu: " + ex.Message);
                return;
            }
        }
        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRowView dr = ((DataRowView)bdsPhieuXuat[bdsPhieuXuat.Position]);
            DateTime ngayLap = new DateTime();
            if (dr["NGAY"] != DBNull.Value)
            {
                if (DateTime.TryParse(dr["NGAY"].ToString(), out ngayLap))
                {
                }
                else
                {
                    // Xử lý trường hợp không thể chuyển đổi ngày
                    MessageBox.Show("Giá trị trong cột NGAY không phải là kiểu ngày tháng hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            String maNV = dr["MANV"].ToString();
            String MAPX = dr["MAPX"].ToString().Trim();
            String maKho = dr["MAKHO"].ToString();
            String hotenKH = dr["HOTENKH"].ToString();

            if (Program.username != maNV)
            {
                ThongBao("Không thể xóa phiếu xuất không phải do mình tạo");
                return;
            }
            if (bdsCTPX.Count > 0)
            {
                ThongBao("Không thể xóa phiếu xuất vì có chi tiết phiếu xuất");
                return;
            }
            String cauTruyVanHoanTac = "INSERT INTO DBO.PHIEUNHAP(MAPX, NGAY, HOTENKH, MANV, MAKHO) " +
                    "VALUES( '" + MAPX + "', '" +
                    ngayLap.ToString("yyyy-MM-dd") + "', '" +
                    hotenKH + "', '" +
                    maNV + "', '" +
                    maKho + "')";
            undoList.Push(cauTruyVanHoanTac);
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa không ?", "Thông báo",
                MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    position = bdsPhieuXuat.Position;
                    bdsPhieuXuat.RemoveCurrent();
                    this.phieuXuatTableAdapter.Connection.ConnectionString = Program.conStr;
                    this.phieuXuatTableAdapter.Update(this.dS1.PhieuXuat);
                    this.phieuXuatTableAdapter.FillBy(this.dS1.PhieuXuat);

                    /*Cap nhat lai do ben tren can tao cau truy van nen da dat dangThemMoi = true*/
                    isAdding = false;
                    ThongBao("Xóa phiếu xuất thành công");
                    btnHoanTac.Enabled = true;
                }
                catch (Exception ex)
                {
                    ThongBao("Lỗi xóa phiếu xuất! " + ex.Message);
                    this.phieuXuatTableAdapter.Connection.ConnectionString = Program.conStr;
                    this.phieuXuatTableAdapter.Update(this.dS1.PhieuXuat);
                    this.phieuXuatTableAdapter.FillBy(this.dS1.PhieuXuat);
                    // Trả lại vị trí cũ của nhân viên xóa bị lỗi
                    bdsPhieuXuat.Position = position;
                    return;
                }
            }
            else
            {
                undoList.Pop();
            }

        }
        private void btnHoanTac_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Thay đổi bật/ tắt các nút chức năng
            
            btnXoa.Enabled = true;

            btnLamMoi.Enabled = true;
            btnThoat.Enabled = true;
            btnChitietPX.Enabled = true;

            phieuXuatGridControl.Enabled = true;
            dgvCTPX.Enabled = true;


            if (isAdding == true && btnThem.Enabled == false && cheDo.Equals("PX"))
            {
                btnThem.Enabled = true;
                txtMAPX.Enabled = false;
                dteNGAY.Enabled = true;
                hOTENComboBox.Enabled = true;
                tENKHOComboBox.Enabled = true;

                contextMenuStripCTPX.Enabled = true;
                //Hủy thao tác trên bds
                bdsPhieuXuat.CancelEdit();
                bdsPhieuXuat.RemoveCurrent();
                /* trở về lúc đầu con trỏ đang đứng*/
                bdsPhieuXuat.Position = position;
                return;
            }
            else if (isAdding == true && contextMenuStripCTPX.Enabled == false && cheDo.Equals("CTPX"))
            {
                contextMenuStripCTPX.Enabled = true;
                thêmToolStripMenuItem.Enabled = true;
                xóaToolStripMenuItem.Enabled = true;
                bdsCTPX.CancelEdit();
                bdsCTPX.RemoveCurrent();
                bdsCTPX.Position = position;
                this.cTPXTableAdapter.Connection.ConnectionString = Program.conStr;
                this.cTPXTableAdapter.Fill(this.dS1.CTPX);
                return;
            }
            btnThem.Enabled = true;
            // Kiểm tra undoStack trống hay không
            // Trường hợp stack trống thì không thực hiện
            if (undoList.Count == 0)
            {
                ThongBao("Không có tháo tác để khôi phục");
                btnHoanTac.Enabled = false;
                return;
            }

            // Tạo một String để lưu truy vấn được lấy ra từ stack
            String undoSql = undoList.Pop().ToString();
            int n = Program.ExecSqlNonQuery(undoSql);
            if (cheDo.Equals("PX"))
            {
                bdsPhieuXuat.CancelEdit();
                this.phieuXuatTableAdapter.Connection.ConnectionString = Program.conStr;
                this.phieuXuatTableAdapter.FillBy(this.dS1.PhieuXuat);
                bdsPhieuXuat.Position = position;
            }
            else
            {
                bdsCTPX.CancelEdit();
                this.cTPXTableAdapter.Connection.ConnectionString = Program.conStr;
                this.cTPXTableAdapter.Fill(this.dS1.CTPX);
                bdsCTPX.Position = position;
            }
        }
        private void thêmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Lấy vị trí của con trỏ
            position = bdsCTPX.Position;
            isAdding = true;
            cheDo = "CTPX";

            bdsCTPX.AddNew();


            // Thay đổi bật/ tắt các nút chức năng
            btnThem.Enabled = false;
            btnXoa.Enabled = false;

            btnLamMoi.Enabled = false;
            btnChitietPX.Enabled = false;
            btnThoat.Enabled = true;


            phieuXuatGridControl.Enabled = false;
            groupBoxPhieuXuat.Enabled = false;
            dgvCTPX.Enabled = true;
            contextMenuStripCTPX.Enabled = false;
        }
        private void dgvCTPX_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            cheDo = "CTPX";
            DataGridViewRow selectedRow = dgvCTPX.Rows[e.RowIndex];
            Dictionary<string, object> previousRowData = new Dictionary<string, object>();
            foreach (DataGridViewCell cell in selectedRow.Cells)
            {
                string columnName = dgvCTPX.Columns[cell.ColumnIndex].DataPropertyName;

                // Lưu dữ liệu vào biến previousRowData
                previousRowData[columnName] = cell.Value;
            }
            if (!previousRowDataDict.Keys.Contains(bdsCTPX.Position))
            {
                previousRowDataDict[bdsCTPX.Position] = previousRowData;
            }
            else
            {
                if (string.IsNullOrEmpty(previousRowDataDict[bdsCTPX.Position]["MAVT"].ToString()))
                {
                    previousRowDataDict[bdsCTPX.Position]["MAVT"] = previousRowData["MAVT"];
                }
                if (string.IsNullOrEmpty(previousRowDataDict[bdsCTPX.Position]["DONGIA"].ToString()))
                {
                    previousRowDataDict[bdsCTPX.Position]["DONGIA"] = previousRowData["DONGIA"];
                }
                if (string.IsNullOrEmpty(previousRowDataDict[bdsCTPX.Position]["SOLUONG"].ToString()))
                {
                    previousRowDataDict[bdsCTPX.Position]["SOLUONG"] = previousRowData["SOLUONG"];
                }
            }
            position = bdsCTPX.Position;

        }

        private void xóaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRowView dr = (DataRowView)bdsCTPX[bdsCTPX.Position];
            String MAPX = dr["MAPX"].ToString().Trim();
            String maVT = dr["MAVT"].ToString().Trim();
            int soLuong = Int32.Parse(dr["SOLUONG"].ToString());
            float donGia = float.Parse(dr["DONGIA"].ToString());
            String undoQuery = "INSERT INTO dbo.CTPX(MAPX,MAVT,SOLUONG,DONGIA)\r\n" +
                "VALUES(N'" + MAPX + "',N'" + maVT + "',CAST(" + soLuong + " AS INT),CAST(" + donGia + " AS float))";
            undoList.Push(undoQuery);
            if (MessageBox.Show("Bạn có muốn xóa chi tiết phiếu xuất này không", "Thông báo", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    position = bdsCTPX.Position;
                    bdsCTPX.RemoveCurrent();

                    this.cTPXTableAdapter.Connection.ConnectionString = Program.conStr;
                    this.cTPXTableAdapter.Update(this.dS1.CTPX);
                    this.cTPXTableAdapter.Fill(this.dS1.CTPX);
                    /*Cap nhat lai do ben tren can tao cau truy van nen da dat dangThemMoi = true*/
                    isAdding = false;
                    ThongBao("Xóa chi tiết phiếu xuất thành công");
                    btnHoanTac.Enabled = true;
                }
                catch (Exception ex)
                {
                    ThongBao("Lỗi xóa chi tiết phiếu xuất! " + ex.Message);
                    this.cTPXTableAdapter.Connection.ConnectionString = Program.conStr;
                    this.cTPXTableAdapter.Update(this.dS1.CTPX);
                    this.cTPXTableAdapter.Fill(this.dS1.CTPX);
                    // Trả lại vị trí cũ của nhân viên xóa bị lỗi
                    bdsCTPX.Position = position;
                    return;
                }
            }
            else
            {
                undoList.Pop();
            }
        }
        private void groupBoxPhieuXuat_Enter(object sender, EventArgs e)
        {
            cheDo = "PX";
        }


    }
}