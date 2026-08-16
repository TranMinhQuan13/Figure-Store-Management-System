using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using DoAnQLFIGURE.Models;

namespace DoAnQLFIGURE.Controllers
{
    public class HoaDonController : Controller
    {
        // GET: HoaDon
        QL_FigureEntities6 hd = new QL_FigureEntities6();
        public ActionResult Index()
        {
            List<HoaDon> hds = hd.HoaDons.Include(hd => hd.TINHTRANG1).ToList();
            return View(hds);
        }

        public ActionResult ChinhSua(int id)
        {
            var detail = hd.HoaDons.FirstOrDefault(c => c.MaHD == id);
            return View(detail);
        }

        [HttpPost]
        public ActionResult EditSubmit(HoaDon hoadon)
        {
            if (ModelState.IsValid)
            {
                var detail = hd.HoaDons.FirstOrDefault(c => c.MaHD == hoadon.MaHD);

                detail.TinhTrang = hoadon.TinhTrang;
                detail.DaThanhToan = hoadon.DaThanhToan;
                UpdateModel(detail);
                hd.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult ThongKeDoanhThu(int? nam = null)
        {
            nam = nam ?? DateTime.Now.Year;

            var data = hd.HoaDons
                .Where(h => h.NgayLap.Year == nam && h.DaThanhToan == true)
                .GroupBy(h => h.NgayLap.Month)
                .Select(g => new DoanhThuModel
                {
                    Thang = g.Key,
                    TongDoanhThu = g.Sum(h => h.TongTien)
                })
                .OrderBy(g => g.Thang)
                .ToList();

            var fullData = new List<DoanhThuModel>();
            for (int thang = 1; thang <= 12; thang++)
            {
                var item = data.FirstOrDefault(d => d.Thang == thang);
                fullData.Add(item ?? new DoanhThuModel { Thang = thang, TongDoanhThu = 0 });
            }

            ViewBag.Nam = nam;
            ViewBag.DanhSachNam = hd.HoaDons
                .Where(h => h.DaThanhToan == true)
                .Select(h => h.NgayLap.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            ViewBag.TongNam = fullData.Sum(x => x.TongDoanhThu);

            return View(fullData);
        }

        public JsonResult GetDoanhThuTheoThang(int nam)
        {
            var data = hd.HoaDons
                .Where(h => h.NgayLap.Year == nam && h.DaThanhToan == true)
                .GroupBy(h => h.NgayLap.Month)
                .Select(g => new { Thang = g.Key, DoanhThu = g.Sum(h => h.TongTien) })
                .OrderBy(g => g.Thang)
                .ToList();

            var labels = new List<string>();
            var values = new List<decimal>();

            for (int i = 1; i <= 12; i++)
            {
                var item = data.FirstOrDefault(d => d.Thang == i);
                labels.Add("Tháng " + i);
                values.Add(item?.DoanhThu ?? 0);
            }

            return Json(new { labels, values }, JsonRequestBehavior.AllowGet);
        }

    }
}
