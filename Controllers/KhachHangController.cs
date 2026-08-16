using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnQLFIGURE.Controllers
{
    public class KhachHangController : Controller
    {
        // GET: KhachHang
        QL_FigureEntities6 kh = new QL_FigureEntities6();
        // GET: KhachHang
        public ActionResult Index()
        {
            List<KHACHHANG> khs = kh.KHACHHANGs.ToList();
            return View(khs);
        }
    }
}