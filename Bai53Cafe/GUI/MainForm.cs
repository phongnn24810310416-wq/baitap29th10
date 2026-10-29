using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Bai53Cafe.BUS;
namespace Bai53Cafe.GUI
{
    public class MainForm : Form
    {
        CafeBUS bus = new CafeBUS();
        FlowLayoutPanel banPanel = new FlowLayoutPanel();
        ComboBox cboDanhMuc = new ComboBox(), cboMon = new ComboBox();
        NumericUpDown num = new NumericUpDown();
        DataGridView grid = new DataGridView();
        Button btnThem = new Button(), btnThanhToan = new Button(), btnTaiLai = new Button();
        int maBan = 0;
        public MainForm()
        {
            Text = "Bài 5.3 - Quản lý quán cafe"; Width = 1000; Height = 650; StartPosition = FormStartPosition.CenterScreen;
            var top = new Panel { Dock = DockStyle.Top, Height = 115 };
            banPanel.Dock = DockStyle.Fill; banPanel.AutoScroll = true; banPanel.Padding = new Padding(8);
            var left = new Panel { Dock = DockStyle.Left, Width = 350 };
            var lbl = new Label { Text = "Sơ đồ bàn", Dock = DockStyle.Top, Height = 25 };
            left.Controls.Add(banPanel); left.Controls.Add(lbl);
            var right = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            var controls = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 75 };
            cboDanhMuc.Width = 150; cboMon.Width = 190; num.Minimum = 1; num.Maximum = 100; num.Value = 1; num.Width = 65;
            btnThem.Text = "Thêm món"; btnThem.Width = 100; btnThem.Click += (s,e)=>Them();
            btnThanhToan.Text = "Thanh toán"; btnThanhToan.Width = 100; btnThanhToan.Click += (s,e)=>ThanhToan();
            btnTaiLai.Text = "Tải lại"; btnTaiLai.Click += (s,e)=>TaiBan();
            controls.Controls.AddRange(new Control[] { cboDanhMuc,cboMon,num,btnThem,btnThanhToan,btnTaiLai });
            cboDanhMuc.SelectedIndexChanged += (s,e)=>TaiMon();
            grid.Dock = DockStyle.Fill; grid.ReadOnly = true; grid.AllowUserToAddRows = false; grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            right.Controls.Add(grid); right.Controls.Add(controls);
            Controls.Add(right); Controls.Add(left);
            Load += (s,e)=>{ cboDanhMuc.DisplayMember="TenDanhMuc"; cboDanhMuc.ValueMember="MaDanhMuc"; cboDanhMuc.DataSource=bus.LayDanhMuc(); TaiBan(); };
        }
        void TaiBan()
        {
            banPanel.Controls.Clear();
            foreach(DataRow r in bus.LayBan().Rows)
            {
                int id=Convert.ToInt32(r["MaBan"]); string state=r["TrangThai"].ToString();
                var b=new Button { Text=r["TenBan"]+"\n"+state, Width=100, Height=65, BackColor=state=="Trống"?Color.LightGreen:Color.LightCoral, Tag=id };
                b.Click+=(s,e)=>{ maBan=(int)((Button)s).Tag; grid.DataSource=bus.LayChiTiet(maBan); };
                banPanel.Controls.Add(b);
            }
        }
        void TaiMon()
        {
            if(cboDanhMuc.SelectedValue==null) return;
            int id; if(!int.TryParse(cboDanhMuc.SelectedValue.ToString(),out id)) return;
            cboMon.DisplayMember="TenMon"; cboMon.ValueMember="MaMon"; cboMon.DataSource=bus.LayMon(id);
        }
        void Them()
        {
            try { if(maBan==0 || cboMon.SelectedItem==null) throw new Exception("Hãy chọn bàn và món."); DataRowView r=(DataRowView)cboMon.SelectedItem; bus.ThemMon(maBan,Convert.ToInt32(r["MaMon"]),(int)num.Value,Convert.ToDecimal(r["DonGia"])); grid.DataSource=bus.LayChiTiet(maBan); TaiBan(); }
            catch(Exception ex){ MessageBox.Show(ex.Message,"Thông báo"); }
        }
        void ThanhToan()
        {
            try { if(maBan==0) throw new Exception("Hãy chọn bàn."); decimal t=bus.ThanhToan(maBan); MessageBox.Show("Tổng thanh toán: "+t.ToString("N0")+" VNĐ"); grid.DataSource=bus.LayChiTiet(maBan); TaiBan(); }
            catch(Exception ex){ MessageBox.Show(ex.Message,"Thông báo"); }
        }
    }
}
