using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_Ticaret_Uygulamasi.Models;
using Microsoft.AspNet.Identity;

namespace E_Ticaret_Uygulamasi.Controllers
{
    [Authorize]
    public class SepetController : Controller
    {
        ETicaretVTEntities1 db = new ETicaretVTEntities1();
        // GET: Sepet
        public ActionResult SepeteEkle(int? adet,int id)
        {
            Urunler urun = db.Urunler.Find(id);
            Sepet sepetUrun = db.Sepet.FirstOrDefault(x => x.UrunID == id);

            string userID = User.Identity.GetUserId();
            if (sepetUrun==null)
            {
                Sepet yeniUrun = new Sepet()
                {
                    Adet = adet??1,
                    UrunID=id,
                    ToplamTutar=urun.UrunFiyatı*(adet??1),
                    UserID=userID
                };
                db.Sepet.Add(yeniUrun);
                
            }
            else
            {
                sepetUrun.Adet = sepetUrun.Adet + (adet??1);
                sepetUrun.ToplamTutar = sepetUrun.Adet * urun.UrunFiyatı;
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Index()
        {
            string userID = User.Identity.GetUserId();
           List<Sepet> sepet = db.Sepet.Where(x => x.UserID == userID).Include(u=>u.Urunler).ToList();

            return View(sepet);
        }
        public ActionResult SepetGuncelle(int? adet,int? id)
        {
            if (id==null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            Sepet sepet = db.Sepet.Find(id);

            if (sepet==null)
            {
                return HttpNotFound();
            }
            Urunler urun = db.Urunler.Find(sepet.UrunID);

            sepet.Adet = adet ?? 1;
            sepet.ToplamTutar = sepet.Adet * urun.UrunFiyatı;
            db.SaveChanges();

            return RedirectToAction("Index");
        
        }
        public ActionResult Sil(int id)
        {
            Sepet sepet = db.Sepet.Find(id);
            db.Sepet.Remove(sepet);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}