using E_Ticaret_Uygulamasi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Ticaret_Uygulamasi.Controllers
{
    public class HomeController : Controller
    {
        ETicaretVTEntities1 db = new ETicaretVTEntities1();
        public ActionResult Index()
        {
            ViewBag.Kategoriler = db.Kategoriler.ToList();
            ViewBag.Urunler = db.Urunler.OrderByDescending(x=>x.UrunID).Take(10).ToList();
            return View();
        }
        public ActionResult Kategoriler(int id)
        {
           Kategoriler kategori = db.Kategoriler.Find(id);
            ViewBag.KategoriAdi = kategori.KategoriAdi;
            ViewBag.Kategoriler = db.Kategoriler.ToList();
            return View(db.Urunler.Where(x=>x.KategoriID==id).OrderBy(x=>x.UrunAdi).ToList());
        }
        public ActionResult Urun(int id)
        {
            ViewBag.Kategoriler = db.Kategoriler.ToList();
            return View(db.Urunler.Find(id));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}