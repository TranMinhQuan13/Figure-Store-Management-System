using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnQLFIGURE.Controllers
{
    public class BanHangController : Controller
    {
        QL_FigureEntities6 bh = new QL_FigureEntities6();
        // GET: BanHang
        public ActionResult Index()
        {
            List<FIGURE> fg = bh.FIGUREs.OrderByDescending(s => s.TenFigure).ToList();
            return View(fg);
        }

        public ActionResult TrangChu()
        {
            List<FIGURE> fg = bh.FIGUREs.Where(s => s.Loai==1).ToList();
            List<FIGURE> limited = bh.FIGUREs.Where(s => s.Loai == 3).OrderByDescending(s => s.TenFigure).Take(8).ToList();
            ViewBag.Limited = limited;
            List<FIGURE> anime = bh.FIGUREs.Where(s => s.Loai == 5).OrderByDescending(s => s.TenFigure).Take(8).ToList();
            ViewBag.Anime = anime;
            List<FIGURE> deluxe = bh.FIGUREs.Where(s => s.Loai == 2).OrderByDescending(s => s.TenFigure).Take(8).ToList();
            ViewBag.Deluxe = deluxe;
            List<FIGURE> robot = bh.FIGUREs.Where(s => s.Loai == 4).OrderByDescending(s => s.TenFigure).Take(8).ToList();
            ViewBag.Robot = robot;
            return View(fg);
        }

        public ActionResult Detail(int masp)
        {
            var sp = bh.FIGUREs.FirstOrDefault(s => s.MaFigure == masp);
            List<BINHLUAN> bls = bh.BINHLUANs.Where(s => s.MaFigure == sp.MaFigure).OrderByDescending(s=>s.Ngay).ToList();
            ViewBag.lstbl = bls;
            List<FIGURE> fgsNCC = bh.FIGUREs.Where(s => s.NhaCungCap == sp.NhaCungCap && s.MaFigure != masp).ToList();
            ViewBag.lstncc = fgsNCC;
            List<FIGURE> fgsLOAI = bh.FIGUREs.Where(s => s.Loai == sp.Loai && s.MaFigure != masp).ToList();
            ViewBag.lstloai = fgsLOAI;
            return View(sp);
        }

        public ActionResult GioiThieu()
        {
            return View();
        }
        public ActionResult LienHe()
        {
            return View();
        }
        public ActionResult ThemBL(int masp, FormCollection form)
        {
            if (Session["TenKH"] == null)
                return RedirectToAction("Index", "Account");
            BINHLUAN bl = new BINHLUAN();
            string NoiDung = form["noiDung"];
            int SoSao = 0;
            if (!int.TryParse(form["rating"], out SoSao))
                SoSao = 0;
            bl.MaFigure = masp;
            bl.Ngay = DateTime.Now;
            bl.SoSao = SoSao;
            bl.NoiDung = NoiDung;
            bl.HoTen = Session["TenKH"]?.ToString();
            bh.BINHLUANs.Add(bl);
            bh.SaveChanges();
            return RedirectToAction("Detail", new { masp = masp });

        }


        public ActionResult LocSP(int idloc, int type)
        {
            List<FIGURE> lstfigure = new List<FIGURE>();
            if (type == 1)
            {
                lstfigure = bh.FIGUREs.Where(s => s.Loai == idloc).ToList();
            }
            else if (type == 2)
            {
                lstfigure = bh.FIGUREs.Where(s => s.NhaCungCap == idloc).ToList();
            }
                return View("Index",lstfigure);
        }
        public ActionResult TimKiem1(string keyword)
        {
            List<FIGURE> sps = bh.FIGUREs.Where(s => s.TenFigure.ToLower().Contains(keyword.ToLower())).ToList();
            return View("Index", sps);
        }
        public ActionResult TimKiem(string kw, int? chude, string[] gia)
        {
            List<FIGURE> listfigure = new List<FIGURE>();
            if (!string.IsNullOrEmpty(kw))
            {
                listfigure = bh.FIGUREs.Where(s=>s.TenFigure.Contains(kw.ToLower())).ToList();
            }
            if (chude != null)
            {
                listfigure = bh.FIGUREs.Where(s=>s.Loai==chude).ToList();
            }
            if (gia!=null && gia.Length > 0)
            {
                var kqgia =new List<FIGURE>();
                foreach (string g in gia)
                {
                    if (g.Contains("-"))
                    {
                        var arr = g.Split('-');
                        int min = int.Parse(arr[0]);
                        int max = int.Parse(arr[1]);
                        kqgia.AddRange(listfigure.Where(s => s.GiaBan >= min && s.GiaBan <= max).ToList());
                    }
                    else if (g.Contains(">"))
                    {
                        int min = int.Parse(g.Replace(">", ""));
                        kqgia.AddRange(listfigure.Where(s => s.GiaBan > min).ToList());
                    }
                }
                listfigure =kqgia.Distinct().ToList();


            }
            return View("Index",listfigure);
        }


    }
}