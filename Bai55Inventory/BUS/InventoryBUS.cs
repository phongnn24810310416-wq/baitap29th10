using System.Collections.Generic;
using System.Data;
using Bai55Inventory.DAL;
using Bai55Inventory.DTO;
namespace Bai55Inventory.BUS
{
    public class InventoryBUS
    {
        InventoryDAL dal=new InventoryDAL();
        public DataTable LayVatTu(string k) { return dal.LayVatTu(k); }
        public DataTable LayNCC() { return dal.LayNCC(); }
        public void LuuVatTu(VatTuDTO v,bool sua) { if(string.IsNullOrWhiteSpace(v.MaVatTu)||string.IsNullOrWhiteSpace(v.TenVatTu)) throw new System.Exception("Mã và tên vật tư không được trống."); if(v.SoLuongTon<0) throw new System.Exception("Tồn kho không được âm."); dal.LuuVatTu(v,sua); }
        public void XoaVatTu(string ma) { dal.XoaVatTu(ma); }
        public int LuuPhieu(string loai,int? ncc,List<ChiTietPhieuDTO> ds) { foreach(var x in ds) if(x.SoLuong<=0||x.DonGia<0) throw new System.Exception("Số lượng phải lớn hơn 0 và đơn giá không âm."); return dal.LuuPhieu(loai,ncc,ds); }
        public DataTable CanhBao(decimal m) { return dal.CanhBao(m); }
        public decimal TongGiaTri() { return dal.TongGiaTri(); }
    }
}
