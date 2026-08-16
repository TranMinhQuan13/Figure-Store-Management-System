using DoAnQLFIGURE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnQLFIGURE.Controllers
{
    public class DanhMucController : Controller
    {
        QL_FigureEntities6 bh = new QL_FigureEntities6();
        // GET: _DanhMuc
        public ActionResult _DanhMuc()
        {
            
            return PartialView();
        }

        public ActionResult _DanhMucTheLoai()
        {
            List<LOAIFIGURE> tl = bh.LOAIFIGUREs.OrderByDescending(s => s.TenLoai).ToList();
            return PartialView(tl);
        }

        public ActionResult _DanhMucNCC()
        {
            List<NHACUNGCAP> ncc = bh.NHACUNGCAPs.OrderByDescending(s=>s.TenNCC).ToList();  
            return PartialView(ncc);
        }

        public ActionResult _DanhMucTimKiemNC()
        {
            List<LOAIFIGURE> tl = bh.LOAIFIGUREs.OrderByDescending(s=>s.TenLoai).ToList();
            return PartialView(tl);
        }
    }
}