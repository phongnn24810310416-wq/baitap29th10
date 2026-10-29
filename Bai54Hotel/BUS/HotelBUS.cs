using System.Data;
using Bai54Hotel.DAL;
namespace Bai54Hotel.BUS
{
    public class HotelBUS
    {
        HotelDAL dal=new HotelDAL();
        public DataTable LayPhong(string loai,string trangThai) { return dal.LayPhong(loai,trangThai); }
        public DataTable LayDichVu() { return dal.LayDichVu(); }
        public int CheckIn(string p,string ten,string cccd,string sdt) { if(string.IsNullOrWhiteSpace(ten)||string.IsNullOrWhiteSpace(cccd)) throw new System.Exception("Họ tên và CCCD không được trống."); return dal.CheckIn(p,ten,cccd,sdt); }
        public void ThemDichVu(string p,int d,int s) { if(string.IsNullOrWhiteSpace(p)) throw new System.Exception("Nhập số phòng."); if(s<=0) throw new System.Exception("Số lượng phải lớn hơn 0."); dal.ThemDichVu(p,d,s); }
        public decimal CheckOut(string p) { return dal.CheckOut(p); }
    }
}
