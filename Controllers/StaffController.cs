using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace DoAnQLFIGURE.Controllers
{
    public class StaffController : Controller
    {
        // GET: Staff
        QL_FigureEntities6 sp = new QL_FigureEntities6();
        public ActionResult Index()
        {
            List<NHANVIEN> nvs = sp.NHANVIENs.Include(nv => nv.VAITRO1).ToList();
            return View(nvs);
        }
    }
}