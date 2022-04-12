using _220318_OS_RestaurantMVC.Filters;
using _220318_OS_RestaurantMVC.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace _220318_OS_RestaurantMVC.Controllers
{
    [Area("Admin")]
    [Login]
    public class UrunController : Controller
    {
        private readonly RestaurantContext _db;
        private readonly IWebHostEnvironment _webHost;

        public UrunController(ILogger<HomeController> logger, RestaurantContext db, IWebHostEnvironment webHost)
        {
            this._db = db;
            this._webHost = webHost;
        }

        public IActionResult Index()
        {
            var urunler = _db.Urunler.Include(x => x.Kategori).Include(x => x.UrunlerMalzemeler).ThenInclude(z => z.Malzeme).ToList();
            return View(urunler);
        }

        public IActionResult Yeni()
        {
            ViewBag.Kategoriler = new SelectList(_db.Kategoriler.ToList(), "KategoriId", "KategoriAdi");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Yeni(Urun urun, IFormFile resim)
        {
            ResimHataKontrolleri(resim);

            if (ModelState.IsValid)
            {
                urun.UrunResimURL = ResimYukle(resim);
                _db.Urunler.Add(urun);
                _db.SaveChanges();
                return RedirectToAction("Index", "Urun");
            }

            ViewBag.Kategoriler = new SelectList(_db.Kategoriler.ToList(), "KategoriId", "KategoriAdi");
            return View();
        }

        private string ResimYukle(IFormFile resim)
        {
            string ext = Path.GetExtension(resim.FileName);
            string resimAd = Guid.NewGuid() + ext;
            string dosyaYolu = Path.Combine(_webHost.WebRootPath, "images", "uploads", resimAd);
            using (var stream = new FileStream(dosyaYolu, FileMode.Create))
            {
                resim.CopyTo(stream);
            }
            return resimAd;
        }

        private void ResimHataKontrolleri(IFormFile resim)
        {
            string[] izinVerilenler = { ".jpg", ".png", ".jpeg" };
            if (resim != null)
            {
                string ext = Path.GetExtension(resim.FileName);
                if (!izinVerilenler.Contains(ext))
                {
                    ModelState.AddModelError("resim", "Resim verilen dosya uzanıları jpeg, png, jpeg");
                }
                else if (resim.Length > 1000 * 1000 * 1)//1 mb anlamına gelmekte
                {
                    ModelState.AddModelError("resim", "Maximum Dosya Boyutu 1MB.");
                }
            }
            else
            {
                ModelState.AddModelError("resim", "Resim Zorunludur");
            }
        }

        public IActionResult Duzenle(int? UrunId)
        {
            var urun = _db.Urunler.FirstOrDefault(x => x.UrunId == UrunId);
            ViewBag.Kategoriler = new SelectList(_db.Kategoriler.ToList(), "KategoriId", "KategoriAdi");
            if (urun == null)
            {
                return NotFound();
            }
            return View(urun);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Duzenle(Urun urun, IFormFile resim, bool gorselEkle)
        {
            if (resim != null)
            {
                ResimHataKontrolleri(resim);

            }
            if (ModelState.IsValid)
            {
                if (resim != null)
                {
                    ResimSil(urun.UrunResimURL);
                    urun.UrunResimURL = ResimYukle(resim);
                }
                _db.Update(urun);
                _db.SaveChanges();
                return RedirectToAction("Index", "Urun");
            }
            ViewBag.Kategoriler = new SelectList(_db.Kategoriler.ToList(), "KategoriId", "KategoriAdi");
            return View(urun);
        }

        public IActionResult Sil(int? UrunId)
        {
            if (UrunId == null)
            {
                return RedirectToAction("Index", "Urun");
            }
            Urun silinecekUrun = _db.Urunler.Find(UrunId);
            if (silinecekUrun == null)
            {
                return NotFound();
            }
            return View(silinecekUrun);
        }
        [HttpPost]
        [ActionName("Sil")]
        [ValidateAntiForgeryToken]
        public IActionResult SilOnay(int? UrunId)
        {
            Urun silinecekUrun = _db.Urunler.Find(UrunId);
            _db.Urunler.Remove(silinecekUrun);
            _db.SaveChanges();
            ResimSil(silinecekUrun.UrunResimURL);
            TempData["mesaj"] = "Ürün Başarıyla Silindi.";
            return RedirectToAction("Index", "Urun");
        }
        private void ResimSil(string urunResimURL)
        {
            if (!string.IsNullOrEmpty(urunResimURL))
            {
                var silYol = Path.Combine(_webHost.WebRootPath, "images", "uploads", urunResimURL);
                if (System.IO.File.Exists(silYol))
                {
                    System.IO.File.Delete(silYol);
                }
            }
        }

        public IActionResult MalzemeDuzenle(int? UrunId)
        {

            Urun duzenlenecekUrun = _db.Urunler.Include(x => x.UrunlerMalzemeler).ThenInclude(x => x.Malzeme).FirstOrDefault(x => x.UrunId == UrunId);

            if (duzenlenecekUrun == null)
            {
                return NotFound();
            }
            SelectList selectLists = new SelectList(_db.Malzemeler.ToList(), "MalzemeId", "MalzemeAdi");
            foreach (var item in selectLists)
            {
                if (duzenlenecekUrun.UrunlerMalzemeler.Select(x => x.MalzemeId).ToList().Contains(Convert.ToInt32(item.Value)))
                {
                    item.Selected = true;
                }
            }
            ViewBag.malzemeler = selectLists;
            return View(duzenlenecekUrun);

        }
        [HttpPost]
        public IActionResult MalzemeDuzenle(int? UrunId, List<string> Malzemeler)
        {

            Urun duzenlenecekUrun = _db.Urunler.Include(x => x.UrunlerMalzemeler).FirstOrDefault(x => x.UrunId == UrunId);
            if (duzenlenecekUrun == null)
            {
                return NotFound();
            }

            List<UrunMalzeme> urunMalzemeler = new List<UrunMalzeme>();

            foreach (var item in Malzemeler)
            {
                Malzeme malzeme = _db.Malzemeler.FirstOrDefault(x => x.MalzemeId == Convert.ToInt32(item));
                UrunMalzeme yeniMalzeme = new UrunMalzeme();
                yeniMalzeme.Urun = duzenlenecekUrun;
                yeniMalzeme.Malzeme = malzeme;
                urunMalzemeler.Add(yeniMalzeme);
            }
            duzenlenecekUrun.UrunlerMalzemeler.Clear();
            duzenlenecekUrun.UrunlerMalzemeler = urunMalzemeler;
            _db.Urunler.Update(duzenlenecekUrun);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
