using System;
using System.Data;
using System.Data.SqlClient;
namespace Bai54Hotel.DAL
{
    public class HotelDAL
    {
        public DataTable LayPhong(string loai, string trangThai)
        {
            using(var c=Db.TaoKetNoi()) { var d=new SqlDataAdapter("SELECT SoPhong,LoaiPhong,GiaTheoGio,TrangThai FROM Phong WHERE (@l=N'' OR LoaiPhong=@l) AND (@t=N'' OR TrangThai=@t) ORDER BY SoPhong",c); d.SelectCommand.Parameters.AddWithValue("@l",loai); d.SelectCommand.Parameters.AddWithValue("@t",trangThai); var t=new DataTable(); d.Fill(t); return t; }
        }
        public DataTable LayDichVu()
        {
            using(var c=Db.TaoKetNoi()) { var d=new SqlDataAdapter("SELECT MaDV,TenDV,DonGia FROM DichVu ORDER BY TenDV",c); var t=new DataTable(); d.Fill(t); return t; }
        }
        public int CheckIn(string phong,string ten,string cccd,string sdt)
        {
            using(var c=Db.TaoKetNoi()) { c.Open(); var tx=c.BeginTransaction(); try {
                int kh; using(var q=new SqlCommand("SELECT MaKH FROM KhachHang WHERE CCCD=@cccd",c,tx)) { q.Parameters.AddWithValue("@cccd",cccd); var v=q.ExecuteScalar(); if(v==null) { using(var n=new SqlCommand("INSERT INTO KhachHang(HoTen,CCCD,SoDienThoai) OUTPUT INSERTED.MaKH VALUES(@ten,@cccd,@sdt)",c,tx)) { n.Parameters.AddWithValue("@ten",ten); n.Parameters.AddWithValue("@cccd",cccd); n.Parameters.AddWithValue("@sdt",sdt); kh=(int)n.ExecuteScalar(); } } else kh=(int)v; }
                using(var q=new SqlCommand("IF NOT EXISTS(SELECT 1 FROM Phong WHERE SoPhong=@p AND TrangThai=N'Trống') THROW 50001, 'Phòng không trống', 1; INSERT INTO PhieuDatPhong(SoPhong,MaKH) VALUES(@p,@kh); UPDATE Phong SET TrangThai=N'Đang ở' WHERE SoPhong=@p",c,tx)) { q.Parameters.AddWithValue("@p",phong); q.Parameters.AddWithValue("@kh",kh); q.ExecuteNonQuery(); }
                tx.Commit(); return kh;
            } catch { tx.Rollback(); throw; } }
        }
        public void ThemDichVu(string phong,int dv,int sl)
        {
            using(var c=Db.TaoKetNoi())
            {
                c.Open();
                using(var q=new SqlCommand("INSERT INTO SuDungDichVu(MaPhieu,MaDV,SoLuong) SELECT TOP 1 MaPhieu,@d,@s FROM PhieuDatPhong WHERE SoPhong=@p AND NgayTraPhong IS NULL ORDER BY MaPhieu DESC",c))
                {
                    q.Parameters.AddWithValue("@p",phong);
                    q.Parameters.AddWithValue("@d",dv);
                    q.Parameters.AddWithValue("@s",sl);
                    if(q.ExecuteNonQuery()!=1) throw new Exception("Phòng chưa check-in hoặc không tồn tại.");
                }
            }
        }
        public decimal CheckOut(string phong)
        {
            using(var c=Db.TaoKetNoi()) { c.Open(); var tx=c.BeginTransaction(); try {
                int p; decimal gia; DateTime vao;
                using(var q=new SqlCommand("SELECT TOP 1 p.MaPhieu,f.GiaTheoGio,p.NgayNhanPhong FROM PhieuDatPhong p JOIN Phong f ON f.SoPhong=p.SoPhong WHERE p.SoPhong=@s AND p.NgayTraPhong IS NULL ORDER BY p.MaPhieu DESC",c,tx)) { q.Parameters.AddWithValue("@s",phong); using(var r=q.ExecuteReader()) { if(!r.Read()) throw new Exception("Phòng chưa check-in."); p=r.GetInt32(0); gia=r.GetDecimal(1); vao=r.GetDateTime(2); } }
                decimal dv; using(var q=new SqlCommand("SELECT ISNULL(SUM(s.SoLuong*d.DonGia),0) FROM SuDungDichVu s JOIN DichVu d ON d.MaDV=s.MaDV WHERE s.MaPhieu=@p",c,tx)) { q.Parameters.AddWithValue("@p",p); dv=Convert.ToDecimal(q.ExecuteScalar()); }
                decimal gio=(decimal)Math.Ceiling((DateTime.Now-vao).TotalHours); if(gio<1) gio=1; decimal total=gio*gia+dv;
                using(var q=new SqlCommand("UPDATE PhieuDatPhong SET NgayTraPhong=GETDATE(),TongTienThanhToan=@t WHERE MaPhieu=@p; UPDATE Phong SET TrangThai=N'Đang dọn dẹp' WHERE SoPhong=@s",c,tx)) { q.Parameters.AddWithValue("@t",total); q.Parameters.AddWithValue("@p",p); q.Parameters.AddWithValue("@s",phong); q.ExecuteNonQuery(); }
                tx.Commit(); return total;
            } catch { tx.Rollback(); throw; } }
        }
    }
}
