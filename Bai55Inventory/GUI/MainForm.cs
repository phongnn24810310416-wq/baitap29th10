using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Bai55Inventory.BUS;
using Bai55Inventory.DTO;
namespace Bai55Inventory.GUI
{
    public class MainForm : Form
    {
        InventoryBUS bus=new InventoryBUS();
        DataGridView grid=new DataGridView(),gridPhieu=new DataGridView();
        TextBox txtMa=new TextBox(),txtTen=new TextBox(),txtDVT=new TextBox(),txtTim=new TextBox();
        NumericUpDown numTon=new NumericUpDown(),numSL=new NumericUpDown(),numGia=new NumericUpDown(),numMuc=new NumericUpDown();
        ComboBox cboLoai=new ComboBox(),cboNCC=new ComboBox();
        Button btnTai=new Button(),btnThem=new Button(),btnSua=new Button(),btnXoa=new Button(),btnThemCT=new Button(),btnLuu=new Button(),btnCanhBao=new Button(),btnTong=new Button();
        List<ChiTietPhieuDTO> ds=new List<ChiTietPhieuDTO>();
        public MainForm()
        {
            Text="Bài 5.5 - Quản lý nhập xuất kho"; Width=1200; Height=760; StartPosition=FormStartPosition.CenterScreen;
            var p=new FlowLayoutPanel{Dock=DockStyle.Top,Height=160,AutoScroll=true,Padding=new Padding(8)};
            numTon.Maximum=1000000; numSL.Maximum=1000000; numGia.Maximum=1000000; numMuc.Maximum=1000000; numSL.Minimum=1; numSL.Value=1; numGia.DecimalPlaces=2; numTon.DecimalPlaces=2; numSL.DecimalPlaces=2; numMuc.Value=5;
            cboLoai.Items.AddRange(new object[]{"NHAP","XUAT"}); cboLoai.DropDownStyle=ComboBoxStyle.DropDownList; cboLoai.SelectedIndex=0;
            cboNCC.Width=150; txtMa.Width=90; txtTen.Width=150; txtDVT.Width=70; txtTim.Width=100;
            btnTai.Text="Tìm / Tải"; btnTai.Click+=(s,e)=>Tai();
            btnThem.Text="Thêm"; btnThem.Click+=(s,e)=>LuuVT(false);
            btnSua.Text="Sửa"; btnSua.Click+=(s,e)=>LuuVT(true);
            btnXoa.Text="Xóa"; btnXoa.Click+=(s,e)=>Xoa();
            btnThemCT.Text="Thêm dòng phiếu"; btnThemCT.Click+=(s,e)=>ThemCT();
            btnLuu.Text="Lưu phiếu"; btnLuu.Click+=(s,e)=>LuuPhieu();
            btnCanhBao.Text="Cảnh báo tồn"; btnCanhBao.Click+=(s,e)=>{try{grid.DataSource=bus.CanhBao(numMuc.Value);}catch(Exception ex){MessageBox.Show(ex.Message);}};
            btnTong.Text="Tổng giá trị tồn"; btnTong.Click+=(s,e)=>{try{MessageBox.Show("Giá trị tồn ước tính: "+bus.TongGiaTri().ToString("N0"));}catch(Exception ex){MessageBox.Show(ex.Message);}};
            p.Controls.AddRange(new Control[]{new Label{Text="Mã",AutoSize=true},txtMa,new Label{Text="Tên",AutoSize=true},txtTen,new Label{Text="ĐVT",AutoSize=true},txtDVT,new Label{Text="Tồn",AutoSize=true},numTon,btnThem,btnSua,btnXoa,new Label{Text="Tìm",AutoSize=true},txtTim,btnTai,new Label{Text="Loại phiếu",AutoSize=true},cboLoai,new Label{Text="NCC",AutoSize=true},cboNCC,new Label{Text="SL",AutoSize=true},numSL,new Label{Text="Đơn giá",AutoSize=true},numGia,btnThemCT,btnLuu,new Label{Text="Ngưỡng cảnh báo",AutoSize=true},numMuc,btnCanhBao,btnTong});
            var split=new SplitContainer{Dock=DockStyle.Fill,Orientation=Orientation.Horizontal,SplitterDistance=280};
            grid.Dock=DockStyle.Fill; grid.ReadOnly=true; grid.AllowUserToAddRows=false; grid.SelectionMode=DataGridViewSelectionMode.FullRowSelect; grid.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill;
            grid.SelectionChanged+=(s,e)=>Chon();
            gridPhieu.Dock=DockStyle.Fill; gridPhieu.ReadOnly=true; gridPhieu.AllowUserToAddRows=false; gridPhieu.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill;
            split.Panel1.Controls.Add(grid); split.Panel2.Controls.Add(gridPhieu); Controls.Add(split); Controls.Add(p);
            Load+=(s,e)=>{cboNCC.DisplayMember="TenNCC";cboNCC.ValueMember="MaNCC";cboNCC.DataSource=bus.LayNCC();Tai();};
        }
        void Tai(){try{grid.DataSource=bus.LayVatTu(txtTim.Text.Trim());}catch(Exception ex){MessageBox.Show(ex.Message);}}
        void Chon(){if(grid.CurrentRow==null)return; try{txtMa.Text=grid.CurrentRow.Cells["MaVatTu"].Value.ToString();txtTen.Text=grid.CurrentRow.Cells["TenVatTu"].Value.ToString();txtDVT.Text=grid.CurrentRow.Cells["DVT"].Value.ToString();numTon.Value=Convert.ToDecimal(grid.CurrentRow.Cells["SoLuongTon"].Value);}catch{}}
        void LuuVT(bool sua){try{bus.LuuVatTu(new VatTuDTO{MaVatTu=txtMa.Text.Trim(),TenVatTu=txtTen.Text.Trim(),DVT=txtDVT.Text.Trim(),SoLuongTon=numTon.Value},sua);Tai();}catch(Exception ex){MessageBox.Show(ex.Message);}}
        void Xoa(){try{bus.XoaVatTu(txtMa.Text.Trim());Tai();}catch(Exception ex){MessageBox.Show(ex.Message);}}
        void ThemCT(){try{if(grid.CurrentRow==null)throw new Exception("Chọn vật tư trong danh sách.");string ma=grid.CurrentRow.Cells["MaVatTu"].Value.ToString();ds.Add(new ChiTietPhieuDTO{MaVatTu=ma,SoLuong=numSL.Value,DonGia=numGia.Value});var t=new DataTable();t.Columns.Add("Mã vật tư");t.Columns.Add("Số lượng");t.Columns.Add("Đơn giá");foreach(var x in ds)t.Rows.Add(x.MaVatTu,x.SoLuong,x.DonGia);gridPhieu.DataSource=t;}catch(Exception ex){MessageBox.Show(ex.Message);}}
        void LuuPhieu(){try{int? ncc=cboLoai.Text=="NHAP"&&cboNCC.SelectedValue!=null?(int?)Convert.ToInt32(cboNCC.SelectedValue):null;int id=bus.LuuPhieu(cboLoai.Text,ncc,ds);MessageBox.Show("Đã lưu phiếu số "+id);ds.Clear();gridPhieu.DataSource=null;Tai();}catch(Exception ex){MessageBox.Show(ex.Message);}}
    }
}
