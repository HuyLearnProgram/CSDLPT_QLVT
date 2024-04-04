﻿using DevExpress.XtraReports.UI;
using DevExpress.XtraRichEdit.Fields;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLVT.Report
{
    public partial class FormSupport : Form
    {
        private int choice;
        private string brand = "";
        public FormSupport(int choice)
        {
            InitializeComponent();
            //Phân quyền nhóm CONGTY được đổi chi nhánh report
            if (Program.mGroup == "CONGTY")
            {
                cbChiNhanh.Enabled = true;
            }
            else
            {
                cbChiNhanh.Enabled = false;
            }
            this.choice = choice;
        }

        private void FormSupport_Load(object sender, EventArgs e)
        {
            //Lấy thông tin chi nhánh từ form đăng nhập
            cbChiNhanh.DataSource = Program.bindingSource;
            cbChiNhanh.DisplayMember = "TENCN";
            cbChiNhanh.ValueMember = "TENSERVER";
            cbChiNhanh.SelectedIndex = Program.brand;
        }

        private void cbChiNhanh_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbChiNhanh.SelectedValue.ToString() == "System.Data.DataRowView" || cbChiNhanh.SelectedValue == null)
            {
                return;
            }
            //Program.servername = cbChiNhanh.SelectedValue.ToString();

            if (cbChiNhanh.SelectedValue.ToString().Contains('1'))
            {
                brand = "CN1";
            }
            else if (cbChiNhanh.SelectedValue.ToString().Contains('2'))
            {
                brand = "CN2";
            }

            /*// Nếu chọn chi nhánh khác với chi nhánh hiện tại
            if (cbChiNhanh.SelectedIndex != Program.brand)
            {
                // Dùng tài khoản HTKN để chuẩn bị cho việc login vào chi nhánh khác
                Program.mlogin = Program.remotelogin;
                Program.password = Program.remotepassword;
            }
            else
            {
                // Lấy tài khoản hiện tại đang đăng nhập để đăng nhập
                Program.mlogin = Program.mloginDN;
                Program.password = Program.passwordDN;
            }
            if (Program.connectDB() == 0)
            {
                MessageBox.Show("Lỗi kết nối về chi nhánh", "Thông báo", MessageBoxButtons.OK);
            }*/
        }

        private void button1_Click(object sender, EventArgs e)
        {

           /* switch (choice)
            {
                case 1:
                    ReportDSNV rpdsnv = new ReportDSNV(Program.mGroup, brand);
                    ReportPrintTool rpt = new ReportPrintTool(rpdsnv);
                    rpt.ShowPreviewDialog();
                    break;
            }*/
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*switch (choice)
            {
                case 1:
                    ReportDSNV rpDSNV = new ReportDSNV(Program.mGroup, brand);
                    try
                    {
                        if (File.Exists(@"D:\ReportQLVT\ReportDSNhanVien.pdf"))
                        {
                            DialogResult dr = MessageBox.Show("File report đã tồn tại.\nBạn có muốn ghi đè không?", "Thông báo", MessageBoxButtons.YesNo);
                            if (dr == DialogResult.Yes)
                            {
                                rpDSNV.ExportToPdf(@"D:\ReportQLVT\ReportDSNhanVien.pdf");
                                MessageBox.Show("File ReportDSNhanVien.pdf đã được ghi thành công",
                        "Thông báo", MessageBoxButtons.OK);

                            }
                        }
                        else
                        {
                            rpDSNV.ExportToPdf(@"D:\ReportQLVT\ReportDSNhanVien.pdf");
                            MessageBox.Show("File ReportDSNhanVien.pdf đã được ghi thành công tại ổ D",
                        "Thông báo", MessageBoxButtons.OK);
                        }

                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show("Có thể bạn chưa đóng file\nVui lòng đóng file ReportDSNhanVien.pdf",
                           "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                        return;
                    }
                    break;
            }*/
        }
    }
}