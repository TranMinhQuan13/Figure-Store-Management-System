using DoAnQLFIGURE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnQLFIGURE.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        public ActionResult Index()
        {
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "BanHang");
            }
            List<GioHang> dsgiohang = LayGioHang();
            ViewBag.TongThanhTien = TongThanhTien();
            FIGURE fg = new FIGURE();

            return View(dsgiohang);
            
        }

        public List<GioHang> LayGioHang()
        {
            List<GioHang> dsgiohang = Session["GioHang"] as List<GioHang>;
            if (dsgiohang == null)
            {
                dsgiohang = new List<GioHang>();
                Session["GioHang"] = dsgiohang;

            }
            return dsgiohang;
        }

        public ActionResult ThemGioHang(int mafigure, string url, int soluong = 1)
        {
            if (Session["TenKH"] == null)
                return RedirectToAction("Index", "Account");
            List<GioHang> dsgiohang = LayGioHang();
            GioHang fg = dsgiohang.FirstOrDefault(sp => sp.MaFigure == mafigure);
            if (fg == null)

            {
                fg = new GioHang(mafigure, soluong);
                dsgiohang.Add(fg);
                fg.MaFigure = mafigure;
                fg.SoLuong = soluong;
                Session["SoLuong"] = dsgiohang.Sum(s => s.SoLuong);
                return Redirect(url);
            }
            else
            {
                fg.SoLuong += soluong;
                fg.ThanhTien += soluong * fg.GiaBan;
                Session["SoLuong"] = dsgiohang.Sum(s => s.SoLuong);
                return Redirect(url);
            }
        }

        private decimal TongThanhTien()
        {
            decimal tt = 0;
            List<GioHang> lst = Session["GioHang"] as List<GioHang>;
            if (lst != null)
            {
                tt = lst.Sum(s => s.GiaBan * s.SoLuong);
            }
            return tt;
        }
        [HttpPost]
        public ActionResult CapNhatGioHang(int ms, FormCollection f)
        {
            var action = f["action"];
            var giohang = LayGioHang();
            var item = giohang.FirstOrDefault(s => s.MaFigure == ms);
            if (item != null)
            {
                int soluong = int.Parse(f["soluong"].ToString());
                if (action == "plus")
                {
                    soluong++;
                }
                else if (action == "minus" && soluong > 1)
                {
                    soluong--;

                }
                if (soluong <= 0)
                {
                    giohang.Remove(item);
                }
                else
                {
                    item.SoLuong = soluong;
                    Session["GioHang"] = giohang;
                }
            }
            return RedirectToAction("Index", "GioHang");
        }

        public ActionResult XoaGioHang(int ms)
        {
            List<GioHang> lst = LayGioHang();
            GioHang fg = lst.FirstOrDefault(s => s.MaFigure == ms);
            if (fg != null)
            {
                lst.RemoveAll(sp => sp.MaFigure == ms);
                Session["SoLuong"] = lst.Sum(s => s.SoLuong);
                if (lst.Count == 0)
                {
                    return RedirectToAction("TrangChu", "BanHang");
                }
                return RedirectToAction("Index", "GioHang");
            }

            //if (lst.Count == 0)
            //{
            //    return RedirectToAction("Index", "BanHang");
            //}
            return RedirectToAction("Index", "GioHang");
        }
    }
}