using DoAnQLFIGURE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnQLFIGURE.Controllers
{
    public class ThanhToanController : Controller
    {
        // GET: ThanhToan
        public ActionResult Index()
        {
            return View();
        }
        QL_FigureEntities6 db = new QL_FigureEntities6();

        public ActionResult ChiTietThanhToan()
        {
           
            var makh = Session["MaKH"];
            var kh = db.KHACHHANGs.FirstOrDefault(s => s.MaKH == (int)makh);

            if (makh == null)
            {
                
                return RedirectToAction("Index", "Account");
            }
            if(string.IsNullOrEmpty(kh.DiaChi) || string.IsNullOrEmpty(kh.DienThoai))
            {
                TempData["WarningMessage"] = "Vui lòng cập nhật địa chỉ và số điện thoại trước khi thanh toán!";
                return RedirectToAction("EditInfo", "Account"); 
            }
            var giohang = Session["GioHang"] as List<GioHang>;
            if (giohang == null || giohang.Count == 0)
            {
                return RedirectToAction("Index", "Giohang");
            }
            using (var bh = new QL_FigureEntities6())
            {
                var hoadon = new HoaDon
                {
                    MaKH = (int)makh,
                    NgayLap = DateTime.Now,
                    MaNV = 0,
                    TongTien = giohang.Sum(sp => sp.ThanhTien) + 10000,
                    TinhTrang = 1,
                    DaThanhToan = true,
                    DiaChiGiaoHang = kh.DiaChi
                };
                bh.HoaDons.Add(hoadon);
                bh.SaveChanges();
                foreach (var item in giohang)
                {
                    var ct = new CHITIETHOADON
                    {
                        MaHD = hoadon.MaHD,
                        MaFigure = item.MaFigure,
                        SoLuong = item.SoLuong,
                        GiaBan = item.GiaBan,
                    };
                    bh.CHITIETHOADONs.Add(ct);
                }
                bh.SaveChanges();

            }
            Session["GioHang"] = null;
            Session["SoLuong"] = 0;
            return RedirectToAction("Index", "ThanhToan");

        }
        public ActionResult ChiTietDonHang()
        {
            var maKH = Session["MaKH"];
            if (maKH == null)
            {
                return RedirectToAction("Index", "Account");
            }
            var hoadon = db.HoaDons
                           .Where(s => s.MaKH == (int)maKH)
                           .OrderByDescending(s => s.MaHD)
                           .FirstOrDefault();

            if (hoadon == null)
            {
                TempData["Error"] = "Bạn chưa có đơn hàng nào.";
                return RedirectToAction("Index", "BanHang");
            }

            var khachhang = db.KHACHHANGs.FirstOrDefault(s => s.MaKH == (int)maKH);

            var chiTietHD = db.CHITIETHOADONs
                              .Where(ct => ct.MaHD == hoadon.MaHD)
                              .Join(db.FIGUREs,
                                    ct => ct.MaFigure,
                                    fg => fg.MaFigure,
                                    (ct, fg) => new GioHang 
                                    {
                                        MaFigure = fg.MaFigure,
                                        TenFigure = fg.TenFigure,
                                        GiaBan = fg.GiaBan ?? 0,
                                        SoLuong = ct.SoLuong,
                                        ThanhTien = ct.SoLuong * (fg.GiaBan ?? 0),
                                        Anh = fg.AnhBia 
                                    })
                              .ToList();

            ViewBag.KhachHang = khachhang;
            ViewBag.GioHang = chiTietHD; 
            return View(hoadon);
        }
    }

}