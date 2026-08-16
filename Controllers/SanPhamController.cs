using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace DoAnQLFIGURE.Controllers
{
    public class SanPhamController : Controller
    {
        // GET: SanPham
        QL_FigureEntities6 sp=new QL_FigureEntities6    ();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DanhSachSP()
        {

            List<FIGURE> sps = sp.FIGUREs.Include(c => c.HINHANHs).ToList();
            return View(sps);
        }
        public ActionResult ChinhSua(int? id)
        {
            var detail = sp.FIGUREs.Include(c => c.HINHANHs).FirstOrDefault(c => c.MaFigure == id);
            return View(detail);
        }

        [HttpPost]
        public ActionResult EditSubmit(FIGURE mohinh)
        {
            if (ModelState.IsValid)
            {
                var detail = sp.FIGUREs.Include(c => c.HINHANHs).FirstOrDefault(c => c.MaFigure == mohinh.MaFigure);
                detail.TenFigure = mohinh.TenFigure;
                detail.MoTa = mohinh.MoTa;
                UpdateModel(detail);
                sp.SaveChanges();
            }
            return RedirectToAction("DanhSachSP");
        }
        public ActionResult Xoa(int? id)
        {
            var detail = sp.FIGUREs.FirstOrDefault(c => c.MaFigure == id);
            return View(detail);
        }

        [HttpPost]
        public ActionResult DeleteSubmit(FIGURE id)
        {
            if (ModelState.IsValid)
            {
                var detail = sp.FIGUREs.Include(c => c.HINHANHs).Include(c => c.BINHLUANs).FirstOrDefault(c => c.MaFigure == id.MaFigure);
                var hinhAnhs = sp.HINHANHs.Where(h => h.MaFigure == id.MaFigure).ToList();
                foreach (var ha in hinhAnhs)
                {
                    sp.HINHANHs.Remove(ha);
                }
                var binhLuans = sp.BINHLUANs.Where(h => h.MaFigure == id.MaFigure).ToList();
                foreach (var bl in binhLuans)
                {
                    sp.BINHLUANs.Remove(bl);
                }
                sp.FIGUREs.Remove(detail);
                sp.SaveChanges();
            }
            return RedirectToAction("DanhSachSP");
        }

        public ActionResult Them()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateSubmit(FIGURE figure, HttpPostedFileBase[] DuongDan)
        {
            sp.FIGUREs.Add(figure);
            sp.SaveChanges();
            if (DuongDan != null && DuongDan.Length > 0)
            {
                foreach (var file in DuongDan)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        string tenduongdan = Path.GetFileName(file.FileName);

                        HINHANH hinhanh = new HINHANH()
                        {
                            MaFigure = figure.MaFigure,
                            DuongDan = tenduongdan
                        };
                        sp.HINHANHs.Add(hinhanh);

                    }
                }
                sp.SaveChanges();
            }
            return RedirectToAction("DanhSachSP");
        }
    }
}