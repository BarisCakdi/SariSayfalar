using Microsoft.AspNetCore.Mvc;
using SarıSayfalar.Models;
using System;
using System.Diagnostics;
using System.Globalization;

namespace SarıSayfalar.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Title"] = "Ana Sayfa";
            return View(UrunleriGetir());
        }
        
        public IActionResult Editor()
        {
            ViewData["Title"] = "Editör";
            return View(UrunleriGetir());
        }
        [HttpPost]
        public IActionResult UrunEkle(Urun model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Hata"] = "Ürün bilgileri eksik veya hatalı!";
                return View();
            }
            //ürün varlıığını kontrol et
            var urunler = UrunleriGetir();
            if (urunler.Any(u => u.Ad == model.Ad))
            {
                ViewData["Hata"] = "Bu ürün zaten mevcut";
                return View();
            }

            //Yeni ürün oluştur ve listeye ekle
            var urun = new Urun
            {
                Ad = model.Ad,
                Fiyat = model.Fiyat,
                Img = model.Img,
                Slug = model.Slug,
                Tarih = DateTime.Now
            };
            urunler.Add(urun);
            //Değişiklikleri kaydet
            DegisiklikleriKaydet(urunler);

            ViewData["Basari"] = "Ürün başarıyla eklendi.";
            return View("Editor", urunler);
        }

        [HttpPost]
        public IActionResult UrunGuncelle(Urun model)
        {
            ViewData["Title"] = "Editör";
            if (ModelState.IsValid)
            {
                ViewData["Hata"] = "Ürün bilgileri eksik veya hatalı";
                return View();
            }
            //Güncellencek ürün bul
            var urunler = UrunleriGetir();
            var guncellenecekUrun = urunler.FirstOrDefault(u => u.Ad == model.Ad);
            if (guncellenecekUrun == null)
            {
                ViewData["Hata"] = "Güncellenecek ürün bulunamadı.";
                return View();
            }
            //ürün bilgilerini güncelle
            guncellenecekUrun.Fiyat = model.Fiyat;
            guncellenecekUrun.Img = model.Img;
            guncellenecekUrun.Tarih = model.Tarih;
            guncellenecekUrun.Slug = model.Slug;

            //değişiklikleri kaydet
            DegisiklikleriKaydet(urunler);

            ViewData["Basarili"] = "Ürün başarıyla güncellendi.";
            return View("Editor", urunler);

        }
        [HttpPost]
        public IActionResult UrunSil(Urun model)
        {
            
            //ürün varlığı kontrol
            var urunler = UrunleriGetir();
            for (int i = 0; i < urunler.Count; i++)
            {
                if (urunler[i].Ad == model.Ad)
                {
                    urunler.RemoveAt(i);
                    ViewData["Mesaj"] = "Ürünler Silindi";
                    break;
                }
            }
            DegisiklikleriKaydet(urunler);
            return View("Editor", urunler);
        }
        public void SatisEkle(Urun urun)
        {
            using StreamReader reader = new("App_Data/satislar.txt");
            var satislarTxt = reader.ReadToEnd();
            reader.Close();

            if (!string.IsNullOrEmpty(satislarTxt))
            {
                satislarTxt += "\n";
            }
            using StreamWriter writer = new("App_Data/satislar.txt");
            writer.Write($"{satislarTxt}{urun.Ad}|{urun.Fiyat}|{urun.Img}|{urun.Slug}|{urun.Tarih}");
        }
        public List<Urun> UrunleriGetir()
        {
            var urunler = new List<Urun>();
            using StreamReader reader = new("App_Data/urunler.txt");
            var urunlerTxt = reader.ReadToEnd();
            var urunlerSatirlar = urunlerTxt.Split('\n');
            foreach(var satir in urunlerSatirlar)
            {
                var urunSatir = satir.Split('|');
                urunler.Add(new Urun
                {
                    Ad = urunSatir[0],
                    Fiyat = int.Parse(urunSatir[1]),
                    Img = urunSatir[2],
                    Slug = urunSatir[3],
                    Tarih = DateTime.Parse(urunSatir[4])

                });
            }
            return urunler;
        }

        public void DegisiklikleriKaydet(List<Urun> urunler)
        {
            var satirlarTxt = "";
            foreach (var urun in urunler)
            {
                satirlarTxt += $"{urun.Ad}|{urun.Fiyat}|{urun.Img}|{urun.Slug}|{urun.Tarih:yyyy.MM.dd}{(urun != urunler.Last() ? "\n" : "")}";

            }
            using StreamWriter writer = new("App_Data/urunler.txt");
            writer.Write(satirlarTxt);
        }
    }
}
