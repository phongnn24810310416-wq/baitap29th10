using System.Data;
using System.Data.SqlClient;
namespace Bai53Cafe.DAL
{
    public class CafeDAL
    {
        public DataTable LayBan()
        {
            using (var c = Db.TaoKetNoi()) { var d = new SqlDataAdapter("SELECT MaBan,TenBan,TrangThai FROM BanAn ORDER BY MaBan", c); var t = new DataTable(); d.Fill(t); return t; }
        }
        public DataTable LayDanhMuc()
        {
            using (var c = Db.TaoKetNoi()) { var d = new SqlDataAdapter("SELECT MaDanhMuc,TenDanhMuc FROM DanhMuc ORDER BY TenDanhMuc", c); var t = new DataTable(); d.Fill(t); return t; }
        }
        public DataTable LayMon(int maDanhMuc)
        {
            using (var c = Db.TaoKetNoi()) { var d = new SqlDataAdapter("SELECT MaMon,TenMon,DonGia FROM MonAn WHERE MaDanhMuc=@id ORDER BY TenMon", c); d.SelectCommand.Parameters.AddWithValue("@id", maDanhMuc); var t = new DataTable(); d.Fill(t); return t; }
        }
        public DataTable LayChiTiet(int maBan)
        {
            using (var c = Db.TaoKetNoi()) { var d = new SqlDataAdapter(@"SELECT m.TenMon AS [Món], c.SoLuong AS [Số lượng], c.DonGia AS [Đơn giá], c.SoLuong*c.DonGia AS [Thành tiền] FROM HoaDon h JOIN ChiTietHoaDon c ON h.MaHoaDon=c.MaHoaDon JOIN MonAn m ON m.MaMon=c.MaMon WHERE h.MaBan=@b AND h.TrangThai=0", c); d.SelectCommand.Parameters.AddWithValue("@b", maBan); var t = new DataTable(); d.Fill(t); return t; }
        }
        public void ThemMon(int maBan, int maMon, int soLuong, decimal donGia)
        {
            using (var c = Db.TaoKetNoi())
            {
                c.Open(); var tx = c.BeginTransaction();
                try
                {
                    int id;
                    using (var q = new SqlCommand("SELECT TOP 1 MaHoaDon FROM HoaDon WHERE MaBan=@b AND TrangThai=0 ORDER BY MaHoaDon DESC", c, tx))
                    { q.Parameters.AddWithValue("@b", maBan); var v = q.ExecuteScalar(); if (v == null) { using (var n = new SqlCommand("INSERT INTO HoaDon(MaBan) OUTPUT INSERTED.MaHoaDon VALUES(@b)", c, tx)) { n.Parameters.AddWithValue("@b", maBan); id = (int)n.ExecuteScalar(); } } else id = (int)v; }
                    using (var q = new SqlCommand(@"IF EXISTS(SELECT 1 FROM ChiTietHoaDon WHERE MaHoaDon=@h AND MaMon=@m) UPDATE ChiTietHoaDon SET SoLuong=SoLuong+@s WHERE MaHoaDon=@h AND MaMon=@m ELSE INSERT INTO ChiTietHoaDon VALUES(@h,@m,@s,@g); UPDATE BanAn SET TrangThai=N'Có khách' WHERE MaBan=@b;", c, tx))
                    { q.Parameters.AddWithValue("@h", id); q.Parameters.AddWithValue("@m", maMon); q.Parameters.AddWithValue("@s", soLuong); q.Parameters.AddWithValue("@g", donGia); q.Parameters.AddWithValue("@b", maBan); q.ExecuteNonQuery(); }
                    tx.Commit();
                }
                catch { tx.Rollback(); throw; }
            }
        }
        public decimal ThanhToan(int maBan)
        {
            using (var c = Db.TaoKetNoi())
            {
                c.Open(); var tx = c.BeginTransaction();
                try
                {
                    int id; using (var q = new SqlCommand("SELECT TOP 1 MaHoaDon FROM HoaDon WHERE MaBan=@b AND TrangThai=0 ORDER BY MaHoaDon DESC", c, tx))
                    { q.Parameters.AddWithValue("@b", maBan); var v=q.ExecuteScalar(); if(v==null) throw new System.Exception("Bàn chưa có hóa đơn."); id=(int)v; }
                    decimal total; using (var q = new SqlCommand("SELECT ISNULL(SUM(SoLuong*DonGia),0) FROM ChiTietHoaDon WHERE MaHoaDon=@h", c, tx))
                    { q.Parameters.AddWithValue("@h", id); total=(decimal)q.ExecuteScalar(); }
                    using (var q = new SqlCommand("UPDATE HoaDon SET GioRa=GETDATE(),TongTien=@t,TrangThai=1 WHERE MaHoaDon=@h; UPDATE BanAn SET TrangThai=N'Trống' WHERE MaBan=@b", c, tx))
                    { q.Parameters.AddWithValue("@t", total); q.Parameters.AddWithValue("@h", id); q.Parameters.AddWithValue("@b", maBan); q.ExecuteNonQuery(); }
                    tx.Commit(); return total;
                }
                catch { tx.Rollback(); throw; }
            }
        }
    }
}
