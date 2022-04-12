using _220318_OS_RestaurantMVC.Areas.Admin.ViewModel;
using _220318_OS_RestaurantMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace _220318_OS_RestaurantMVC.Controllers
{
    [Area("Admin")]
    public class LoginController : Controller
    {
        private readonly RestaurantContext _db;

        public LoginController(RestaurantContext db)
        {
            this._db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(LoginViewModel kullanici)
        {
            if (ModelState.IsValid)
            {
                var kullanicidb = _db.Kullanicilar.FirstOrDefault(a => a.Email == kullanici.Email && a.Password == kullanici.Password);
                if (kullanicidb != null)
                {
                    if (kullanici.isRememberMe)
                    {
                        HttpContext.Response.Cookies.Append("KullaniciEmail", kullanici.Email, new CookieOptions()
                        {
                            Expires = DateTime.Now.AddDays(14)
                        });
                    }
                    else
                    {
                        HttpContext.Session.SetString("KullaniciEmail", kullanici.Email);
                    }
                    TempData["mesaj"] = "Başarıyla Giriş Yaptınız.";
                    return RedirectToAction("Index", "Urun", new { area = "Admin" });
                }
                ModelState.AddModelError("", "E-mail veya parola hatalı.");
                return View();
            }
            else
            {
                return View();
            }
        }
    }
}
