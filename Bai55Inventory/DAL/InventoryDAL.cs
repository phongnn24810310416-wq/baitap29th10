using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Bai55Inventory.DTO;
namespace Bai55Inventory.DAL
{
    public class InventoryDAL
    {
        public DataTable LayVatTu(string tuKhoa)
        {
            using(var c=Db.TaoKetNoi()) { var d=new SqlDataAdapter("SELECT MaVatTu,TenVatTu,DVT,SoLuongTon FROM VatTu WHERE MaVatTu LIKE @k OR TenVatTu LIKE @k ORDER BY MaVatTu",c); d.SelectCommand.Parameters.AddWithValue("@k","%"+tuKhoa+"%"); var t=new DataTable(); d.Fill(t); return t; }
        }
        public DataTable LayNCC()
        {
            using(var c=Db.TaoKetNoi()) { var d=new SqlDataAdapter("SELECT MaNCC,TenNCC,DienThoai,DiaChi FROM NhaCungCap ORDER BY TenNCC",c); var t=new DataTable(); d.Fill(t); return t; }
        }
        public void LuuVatTu(VatTuDTO v,bool sua)
        {
            using(var c=Db.TaoKetNoi()) { c.Open(); string sql=sua?"UPDATE VatTu SET TenVatTu=@t,DVT=@d,SoLuongTon=@s WHERE MaVatTu=@m":"INSERT INTO VatTu(MaVatTu,TenVatTu,DVT,SoLuongTon) VALUES(@m,@t,@d,@s)";
                using(var q=new SqlCommand(sql,c)) { q.Parameters.AddWithValue("@m",v.MaVatTu); q.Parameters.AddWithValue("@t",v.TenVatTu); q.Parameters.AddWithValue("@d",v.DVT); q.Parameters.AddWithValue("@s",v.SoLuongTon); q.ExecuteNonQuery(); } }
        }
        public void XoaVatTu(string ma) { using(var c=Db.TaoKetNoi()) { using(var q=new SqlCommand("DELETE FROM VatTu WHERE MaVatTu=@m",c)) { q.Parameters.AddWithValue("@m",ma); c.Open(); q.ExecuteNonQuery(); } } }
        public int LuuPhieu(string loai,int? ncc,List<ChiTietPhieuDTO> ds)
        {
            if(ds==null||ds.Count==0) throw new Exception("Phiếu phải có ít nhất một vật tư.");
            using(var c=Db.TaoKetNoi()) { c.Open(); var tx=c.BeginTransaction(); try {
                decimal tong=0; foreach(var x in ds) tong+=x.SoLuong*x.DonGia;
                int id; using(var q=new SqlCommand("INSERT INTO PhieuKho(LoaiPhieu,MaNCC,TongTien) OUTPUT INSERTED.MaPhieu VALUES(@l,@n,@t)",c,tx)) { q.Parameters.AddWithValue("@l",loai); q.Parameters.AddWithValue("@n",(object)ncc??DBNull.Value); q.Parameters.AddWithValue("@t",tong); id=(int)q.ExecuteScalar(); }
                foreach(var x in ds) {
                    using(var q=new SqlCommand("INSERT INTO ChiTietPhieuKho(MaPhieu,MaVatTu,SoLuong,DonGia) VALUES(@p,@m,@s,@g)",c,tx)) { q.Parameters.AddWithValue("@p",id); q.Parameters.AddWithValue("@m",x.MaVatTu); q.Parameters.AddWithValue("@s",x.SoLuong); q.Parameters.AddWithValue("@g",x.DonGia); q.ExecuteNonQuery(); }
                    string update=loai=="NHAP"?"UPDATE VatTu SET SoLuongTon=SoLuongTon+@s WHERE MaVatTu=@m":"UPDATE VatTu SET SoLuongTon=SoLuongTon-@s WHERE MaVatTu=@m AND SoLuongTon>=@s";
                    using(var q=new SqlCommand(update,c,tx)) { q.Parameters.AddWithValue("@s",x.SoLuong); q.Parameters.AddWithValue("@m",x.MaVatTu); if(q.ExecuteNonQuery()!=1) throw new Exception("Không đủ hàng trong kho hoặc mã vật tư không tồn tại."); }
                }
                tx.Commit(); return id;
            } catch { tx.Rollback(); throw; } }
        }
        public DataTable CanhBao(decimal muc) { using(var c=Db.TaoKetNoi()) { var d=new SqlDataAdapter("SELECT MaVatTu,TenVatTu,DVT,SoLuongTon FROM VatTu WHERE SoLuongTon<@m ORDER BY SoLuongTon",c); d.SelectCommand.Parameters.AddWithValue("@m",muc); var t=new DataTable(); d.Fill(t); return t; } }
        public decimal TongGiaTri() { using(var c=Db.TaoKetNoi()) { using(var q=new SqlCommand("SELECT ISNULL(SUM(v.SoLuongTon*ISNULL(x.DonGia,0)),0) FROM VatTu v OUTER APPLY(SELECT TOP 1 DonGia FROM ChiTietPhieuKho d WHERE d.MaVatTu=v.MaVatTu ORDER BY MaPhieu DESC)x",c)) { c.Open(); return Convert.ToDecimal(q.ExecuteScalar()); } } }
    }
}
