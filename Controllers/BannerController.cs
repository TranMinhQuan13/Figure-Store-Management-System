using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAnQLFIGURE;
using Microsoft.Ajax.Utilities;

namespace DoAnQLFIGURE.Controllers
{
    public class BannerController : Controller
    {
        // GET: Banner
        QL_FigureEntities6 bh = new QL_FigureEntities6();
        public ActionResult Banner()
        {
            List<Banner> fg = bh.Banners.OrderByDescending(s => s.MaBanner).Take(6).ToList();
            return PartialView(fg);
        }
    }
}