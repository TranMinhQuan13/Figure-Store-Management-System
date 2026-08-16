using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DoAnQLFIGURE.Controllers
{
    public class AccountController : Controller
    {
        QL_FigureEntities6 ac = new QL_FigureEntities6();
        // GET: Account
        public ActionResult Index()
        {
            ViewBag.Url = Request.UrlReferrer.ToString() ?? "/";
            return View();
        }

        [HttpPost]
        public ActionResult XuLyFormDN(string url, FormCollection form)
        {
            string email = form["email"];
            string password = form["password"];
            var kq = ac.KHACHHANGs.FirstOrDefault(kh => kh.Email == email && kh.MatKhau == password);
            var ad = ac.NHANVIENs.FirstOrDefault(nv => nv.Email == email && nv.MatKhau == password);
            if (kq != null)
            {
                FormsAuthentication.SetAuthCookie(kq.TenKH, true);
                Session["TenKH"] = kq.TenKH;
                Session["MaKH"] = kq.MaKH;
                Session["DiaChi"]=kq.DiaChi;
                Session["SDT"] = kq.DienThoai;
                return Redirect(url);
            }
            else if (ad != null)
            {
                FormsAuthentication.SetAuthCookie(ad.TenNV, true);
                Session["TenNV"] = ad.TenNV;
                Session["MANV"] = ad.MaNV;
                
                return RedirectToAction("Welcome","Admin");
            }
            else
            {
                ViewBag.Error = "Đăng nhập thất bại";
                ViewBag.Url = string.IsNullOrEmpty(url) ? "/" : url;
                return View("Index");
            }
        }

        public ActionResult LogOut()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("TrangChu", "BanHang");
        }

        public ActionResult Info()
        {
            // Lấy MaKH từ Session (bạn đang dùng Session["MaKH"])
            if (Session["MaKH"] == null)
            {
                return RedirectToAction("Index", "Account"); // Chưa đăng nhập → về trang login
            }

            int maKH = Convert.ToInt32(Session["MaKH"]);
            var khachHang = ac.KHACHHANGs.FirstOrDefault(kh => kh.MaKH == maKH);

            if (khachHang == null)
            {
                return HttpNotFound();
            }

            return View(khachHang); // Truyền model KHACHHANG vào view
        }

        // GET: /Account/EditInfo
       
        public ActionResult EditInfo()
        {
            // Lấy MaKH từ Session (bạn đang dùng Session["MaKH"])
            if (Session["MaKH"] == null)
            {
                return RedirectToAction("Index", "Account"); // Chưa đăng nhập → về trang login
            }

            int maKH = Convert.ToInt32(Session["MaKH"]);
            var khachHang = ac.KHACHHANGs.FirstOrDefault(kh => kh.MaKH == maKH);

            if (khachHang == null)
            {
                return HttpNotFound();
            }

            return View(khachHang); // Truyền model KHACHHANG vào view
        }


        // POST: /Account/Edit
        [HttpPost]


        public ActionResult Edit(KHACHHANG model, HttpPostedFileBase AvatarFile)
        {
            if (Session["MaKH"] == null || Convert.ToInt32(Session["MaKH"]) != model.MaKH)
            {
                return RedirectToAction("Index", "Account");
            }

            if (ModelState.IsValid)
            {
                var khachHang = ac.KHACHHANGs.FirstOrDefault(kh => kh.MaKH == model.MaKH);
                if (khachHang == null)
                {
                    return HttpNotFound();
                }

                // Cập nhật các trường thông tin
                khachHang.TenKH = model.TenKH;
                khachHang.GioiTinh = model.GioiTinh; // "true" = Nam, "false" = Nữ, "" = Khác
                khachHang.NamSinh = model.NamSinh;
                khachHang.DienThoai = model.DienThoai;
                khachHang.Email = model.Email;
                khachHang.DiaChi = model.DiaChi;

                // Xử lý upload Avatar mới (nếu có)
                if (AvatarFile != null && AvatarFile.ContentLength > 0)
                {
                    // Kiểm tra định dạng file
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(AvatarFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("", "Chỉ chấp nhận file ảnh: jpg, jpeg, png, gif.");
                        return View("EditInfo", model);
                    }

                    // Tạo tên file mới (dùng MaKH + thời gian để tránh trùng)
                    var fileName = model.MaKH + "_" + DateTime.Now.Ticks + extension;
                    var path = Path.Combine(Server.MapPath("~/Content/Hinh/"), fileName);

                    AvatarFile.SaveAs(path);

                    // Xóa ảnh cũ nếu có (tùy chọn)
                    if (!string.IsNullOrEmpty(khachHang.Avarta))
                    {
                        var oldPath = Path.Combine(Server.MapPath("~/Content/Avatars/"), khachHang.Avarta);
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    // Lưu tên file mới vào DB
                    khachHang.Avarta = fileName;
                }

                try
                {
                    ac.SaveChanges();

                    // Cập nhật lại Session nếu cần (ví dụ: đổi tên hiển thị)
                    Session["TenKH"] = khachHang.TenKH;

                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                    return RedirectToAction("Info", "Account"); // Quay về trang xem thông tin
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu: " + ex.Message);
                }
            }

            // Nếu có lỗi → trả lại form với dữ liệu đã nhập
            return View("EditInfo", model);
        }
        public ActionResult Register(string returnUrl)
        {
            ViewBag.Url = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
            return View();
        }

        // POST: /Account/Register - Xử lý form đăng ký
        [HttpPost]

        public ActionResult XuLyFormDK(FormCollection form, string returnUrl)
        {
            string hoTen = form["fullname"];
            string email = form["email"];
            string matKhau = form["password"];
            string nhapLaiMatKhau = form["confirmPassword"];
            string gioiTinh = form["gender"];

            // Validation cơ bản
            if (string.IsNullOrEmpty(hoTen) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(matKhau))
            {
                ViewBag.Error = "Vui lòng điền đầy đủ thông tin.";
                ViewBag.Url = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
                return View("Register");
            }

            if (matKhau != nhapLaiMatKhau)
            {
                ViewBag.Error = "Mật khẩu nhập lại không khớp.";
                ViewBag.Url = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
                return View("Register");
            }

            // Kiểm tra email đã tồn tại chưa
            var existingUser = ac.KHACHHANGs.FirstOrDefault(kh => kh.Email == email);
            if (existingUser != null)
            {
                ViewBag.Error = "Email này đã được đăng ký.";
                ViewBag.Url = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
                return View("Register");
            }

            // Tạo khách hàng mới
            KHACHHANG newKH = new KHACHHANG
            {
                TenKH = hoTen,
                Email = email,
                MatKhau = matKhau,
                GioiTinh = gioiTinh == "Male" ? "Nam" : "Nữ",
            };

            ac.KHACHHANGs.Add(newKH);
            ac.SaveChanges();


            FormsAuthentication.SetAuthCookie(hoTen, true);
            Session["TenKH"] = hoTen;
            Session["MaKH"] = newKH.MaKH;


            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "BanHang");
        }
    }

}
