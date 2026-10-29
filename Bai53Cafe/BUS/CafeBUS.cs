using System.Data;
using Bai53Cafe.DAL;
namespace Bai53Cafe.BUS
{
    public class CafeBUS
    {
        private readonly CafeDAL dal = new CafeDAL();
        public DataTable LayBan() { return dal.LayBan(); }
        public DataTable LayDanhMuc() { return dal.LayDanhMuc(); }
        public DataTable LayMon(int id) { return dal.LayMon(id); }
        public DataTable LayChiTiet(int id) { return dal.LayChiTiet(id); }
        public void ThemMon(int ban, int mon, int soLuong, decimal gia) { if (soLuong <= 0) throw new System.Exception("Số lượng phải lớn hơn 0."); if (gia < 0) throw new System.Exception("Đơn giá không hợp lệ."); dal.ThemMon(ban, mon, soLuong, gia); }
        public decimal ThanhToan(int ban) { return dal.ThanhToan(ban); }
    }
}
