using E_Ticaret_Uygulamasi.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Ticaret_Uygulamasi.Controllers
{
    [Authorize]
    public class SiparisController : Controller
        
    {
        ETicaretVTEntities1 db = new ETicaretVTEntities1();
        // GET: Siparis
        public ActionResult Index()
        {
            string userID = User.Identity.GetUserId();
            return View(db.Siparisler.Where(x=>x.UserID==userID).ToList());
        }
        
        public ActionResult SiparisDetay(int id)
        {
            var siparisDetay = db.SiparisDetay.Where(x => x.SiparisID == id).ToList();
            return View(siparisDetay);
        }
        public ActionResult SiparisTamamla()
        {
            //ClientID: Bankadan alınan magaza kodu
            //Amount: sepetteki ürünlerin toplam tutuarı
            //Oid: siparisid 
            //OnayUrl: odeme basarılı oldugunda gelen verılen gosterilecegı url
            //HataUrl:ödeme sırasında hata olduysa gelen hatanın gösterileceği url
            //RDN:hash karsılastırılması ıcın kullanılan bilgi
            //StoreKey:güvenlik anahtarı,bankanın sanal pos sayfasından alınır
            //TransactionType:"Auth"
            //Instalment:""
            //HashStr:HashSet olusturulurken bankanın ıstedıgı bılgıler birleştirilir
            //Hash:Farklı degerler olusturulup bırlestırılır.

            string userID = User.Identity.GetUserId();

            List<Sepet> sepetUrunleri = db.Sepet.Where(x => x.UserID == userID).ToList();

            string ClientId = "1003001";//Bankanın verdiği magaza kodu
            string ToplamTutar = sepetUrunleri.Sum(x => x.ToplamTutar).ToString();

            string sipId = string.Format("{0:yyyyMMddHHmmss}", DateTime.Now);

            string onayURL = "https://localhost:44320/Siparis/Tamamlandi";

            string hataURL = "https://localhost:44320/Siparis/Hatali";

            string RDN = "asdf";
            string StoreKey = "123456";

            string TransActionType = "Auth";
            string Instalment = "";

            string HashStr = ClientId + sipId + ToplamTutar + onayURL + hataURL + TransActionType + Instalment + RDN + StoreKey;//Bankanın istediği bilgiler

            System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();

            byte[] HashBytes = System.Text.Encoding.GetEncoding("ISO-8859-9").GetBytes(HashStr);
            byte[] InputBytes = sha.ComputeHash(HashBytes);
            string Hash = Convert.ToBase64String(InputBytes);

            ViewBag.ClientId = ClientId;
            ViewBag.Oid = sipId;
            ViewBag.okUrl = onayURL;
            ViewBag.failUrl = hataURL;
            ViewBag.TransActionType = TransActionType;
            ViewBag.RDN = RDN;
            ViewBag.Hash = Hash;
            ViewBag.Amount = ToplamTutar;
            ViewBag.StoreType = "3d_pay_hosting"; // Ödeme modelimiz
            ViewBag.Description = "";
            ViewBag.XID = "";
            ViewBag.Lang = "tr";
            ViewBag.EMail = "semihmertbel@gmail.com";
            ViewBag.UserID = "SemihBel"; // bu id yi bankanın sanala pos ekranında biz oluşturuyoruz.
            ViewBag.PostURL = "https://entegrasyon.asseco-see.com.tr/fim/est3Dgate";



            return View();
        }
        public ActionResult Tamamlandi()
        {
            string userID = User.Identity.GetUserId();

            Siparisler siparis = new Siparisler()
            {
                Ad = Request.Form.Get("Ad"),
                Soyad = Request.Form.Get("Soyad"),
                Adres = Request.Form.Get("Adres"),
                Tarih = DateTime.Now,
                TCKimlikNo= Request.Form.Get("TCKimlikNo"),
                Telefon= Request.Form.Get("Telefon"),
                UserID=userID
            };

            List<Sepet> sepetUrun = db.Sepet.Where(x => x.UserID == userID).ToList();

            foreach (var item in sepetUrun)
            {
                SiparisDetay siparisDetay = new SiparisDetay()
                {
                    Adet=item.Adet,
                    ToplamTutar=item.ToplamTutar,
                    UrunID=item.UrunID
                };
                siparis.SiparisDetay.Add(siparisDetay);
                db.Sepet.Remove(item);
            }
            db.Siparisler.Add(siparis);
            db.SaveChanges();

            return View();
        }
        public ActionResult Hatali()
        {
            ViewBag.hata = Request.Form;
            return View();
        }
    }
}