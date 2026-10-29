using System;
using System.Data;
using System.Windows.Forms;
using Bai54Hotel.BUS;
namespace Bai54Hotel.GUI
{
    public class MainForm : Form
    {
        HotelBUS bus=new HotelBUS();
        DataGridView grid=new DataGridView();
        TextBox txtPhong=new TextBox(),txtTen=new TextBox(),txtCCCD=new TextBox(),txtSDT=new TextBox();
        ComboBox cboLoai=new ComboBox(),cboTrangThai=new ComboBox(),cboDV=new ComboBox();
        NumericUpDown num=new NumericUpDown();
        Button btnCheckIn=new Button(),btnCheckOut=new Button(),btnDV=new Button(),btnTai=new Button();
        public MainForm()
        {
            Text="Bài 5.4 - Quản lý khách sạn"; Width=1050; Height=650; StartPosition=FormStartPosition.CenterScreen;
            var p=new FlowLayoutPanel{Dock=DockStyle.Top,Height=145,AutoScroll=true,Padding=new Padding(8)};
            cboLoai.Items.AddRange(new object[]{"","Đơn","Đôi","VIP"}); cboLoai.Width=90; cboLoai.DropDownStyle=ComboBoxStyle.DropDownList; cboLoai.SelectedIndex=0;
            cboTrangThai.Items.AddRange(new object[]{"","Trống","Đang ở","Đang dọn dẹp"}); cboTrangThai.Width=120; cboTrangThai.DropDownStyle=ComboBoxStyle.DropDownList; cboTrangThai.SelectedIndex=0;
            cboDV.Width=160; num.Minimum=1; num.Maximum=100; num.Value=1; num.Width=50;
            btnCheckIn.Text="Check-in"; btnCheckIn.Click+=(s,e)=>CheckIn();
            btnCheckOut.Text="Check-out"; btnCheckOut.Click+=(s,e)=>CheckOut();
            btnDV.Text="Gọi dịch vụ"; btnDV.Click+=(s,e)=>GoiDV();
            btnTai.Text="Lọc / Tải lại"; btnTai.Click+=(s,e)=>Tai();
            p.Controls.AddRange(new Control[]{new Label{Text="Loại phòng",AutoSize=true},cboLoai,new Label{Text="Trạng thái",AutoSize=true},cboTrangThai,btnTai,new Label{Text="Số phòng",AutoSize=true},txtPhong,new Label{Text="Họ tên",AutoSize=true},txtTen,new Label{Text="CCCD",AutoSize=true},txtCCCD,new Label{Text="Điện thoại",AutoSize=true},txtSDT,btnCheckIn,btnCheckOut,cboDV,num,btnDV});
            grid.Dock=DockStyle.Fill; grid.ReadOnly=true; grid.AllowUserToAddRows=false; grid.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill;
            Controls.Add(grid); Controls.Add(p);
            Load+=(s,e)=>{cboDV.DisplayMember="TenDV";cboDV.ValueMember="MaDV";cboDV.DataSource=bus.LayDichVu();Tai();};
        }
        void Tai(){try{grid.DataSource=bus.LayPhong(cboLoai.Text,cboTrangThai.Text);}catch(Exception ex){MessageBox.Show(ex.Message);}}
        void CheckIn(){try{bus.CheckIn(txtPhong.Text.Trim(),txtTen.Text.Trim(),txtCCCD.Text.Trim(),txtSDT.Text.Trim());MessageBox.Show("Check-in thành công.");Tai();}catch(Exception ex){MessageBox.Show(ex.Message);}}
        void CheckOut(){try{decimal t=bus.CheckOut(txtPhong.Text.Trim());MessageBox.Show("Tổng thanh toán: "+t.ToString("N0")+" VNĐ");Tai();}catch(Exception ex){MessageBox.Show(ex.Message);}}
        void GoiDV(){try{if(cboDV.SelectedValue==null)throw new Exception("Chọn dịch vụ."); bus.ThemDichVu(txtPhong.Text.Trim(),Convert.ToInt32(cboDV.SelectedValue),(int)num.Value); MessageBox.Show("Đã thêm dịch vụ.");}catch(Exception ex){MessageBox.Show(ex.Message);}}
    }
}
