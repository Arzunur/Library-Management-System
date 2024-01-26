using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;


class Kitap
{
    public string KitabinAdi { get; set; }
    public string Yazar { get; set; }
    public string ISBN { get; set; }
    public int KopyaSayisi { get; set; }
    public int OduncAlinanKopyaSayisi { get; set; }
    public DateTime OduncAlmaTarihi { get; set; }
    public DateTime IadeTarihi { get; set; }
}


class Kutuphane // Kütüphane Sınıfı
{
    private List<Kitap> kitaplar;
    private List<Kitap> oduncAlinanKitaplar;

    public Kutuphane()
    {
        kitaplar = new List<Kitap>();
    }
    public List<Kitap> OduncAlinanKitaplar()
    {
        return oduncAlinanKitaplar;
    }

    public void VeriyiDosyayaKaydet(string dosyaYolu) //Verinin dosyaya(.txt) kaydetme işlemi 
    {
        using (StreamWriter yazici = new StreamWriter(dosyaYolu))
        {
            foreach (var kitap in kitaplar)
            {
                yazici.WriteLine($"{kitap.KitabinAdi},{kitap.Yazar},{kitap.ISBN},{kitap.KopyaSayisi},{kitap.OduncAlinanKopyaSayisi}"); //kaydetme formatı 
            }
        }

        Console.WriteLine("Kütüphane verileri dosyaya kaydedildi."); ;
    }

    public void VeriyiDosyadanYukle(string dosyaYolu)
    {
        if (File.Exists(dosyaYolu))
        {
            kitaplar.Clear(); // Önceki kitapları temizle

            using (StreamReader okuyucu = new StreamReader(dosyaYolu))
            {
                string satir;
                while ((satir = okuyucu.ReadLine()) != null)
                {
                    string[] kitapVerisi = satir.Split(',');

                    if (kitapVerisi.Length == 5)
                    {
                        Kitap kitap = new Kitap
                        {
                            KitabinAdi = kitapVerisi[0],
                            Yazar = kitapVerisi[1],
                            ISBN = kitapVerisi[2],
                            KopyaSayisi = int.Parse(kitapVerisi[3]),
                            OduncAlinanKopyaSayisi = int.Parse(kitapVerisi[4]),
                        };
                        kitaplar.Add(kitap);
                    }
                    else
                    {
                        Console.WriteLine($"Hata: Beklenen sayıda veri yok. Satır atlandı: {satir}");
                    }
                }
            }

            Console.WriteLine("Kütüphane verileri dosyadan yüklendi.");
        }
        else
        {
            Console.WriteLine("Veri dosyası bulunamadı.");
        }
    }
    public void KitapEkle(Kitap kitap)
    {
        kitaplar.Add(kitap);
    }
    public bool KitapVarMi(string kitapAdi, string isbn)
    {
        foreach (Kitap kitap in kitaplar)
        {
            if (string.Equals(kitap.KitabinAdi, kitapAdi, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(kitap.ISBN, isbn, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
    public void TumKitaplariGoruntule()
    {
        HashSet<string> goruntulenenKitaplar = new HashSet<string>();

        foreach (var kitap in kitaplar)
        {
            string kitapBilgisi = $"{kitap.KitabinAdi}, {kitap.Yazar}, {kitap.ISBN}, {kitap.KopyaSayisi}, {kitap.OduncAlinanKopyaSayisi}";

            if (!goruntulenenKitaplar.Contains(kitapBilgisi))
            {
                Console.WriteLine($"Kitabın Adı: {kitap.KitabinAdi}, Yazar: {kitap.Yazar}, ISBN: {kitap.ISBN}, Kopya Sayısı: {kitap.KopyaSayisi}, Ödünç Alınan Kopya Sayısı: {kitap.OduncAlinanKopyaSayisi}");
                goruntulenenKitaplar.Add(kitapBilgisi);
            }
        }
    }
    public Kitap KitapAra(string aramaKelimesi)
    {
        return kitaplar.Find(kitap =>
         kitap.KitabinAdi.IndexOf(aramaKelimesi, StringComparison.OrdinalIgnoreCase) >= 0 ||
         kitap.Yazar.IndexOf(aramaKelimesi, StringComparison.OrdinalIgnoreCase) >= 0);
    }
    public void KitapIadeEt(string kitapAdi)
    {
        Kitap kitap = KitapAra(kitapAdi);

        if (kitap != null && kitap.OduncAlmaTarihi != null)
        {
            kitap.OduncAlmaTarihi = DateTime.Now; // Şu anki tarih ve saat
            Console.WriteLine($"{kitapAdi} adlı kitap iade edildi. İade edilme tarihi: {kitap.OduncAlmaTarihi}");

            // Mevcut kopya sayısını arttır
            kitap.KopyaSayisi++;

            Console.WriteLine($"Mevcut kopya sayısı: {kitap.KopyaSayisi}");
        }
        else
        {
            Console.WriteLine($"{kitapAdi} adlı kitap ödünç alınmamış veya zaten iade edilmiş.");
        }
    }
    public void GecmisKitaplariGoruntule()
    {
        var gecmisKitaplar = kitaplar.FindAll(k => k.OduncAlinanKopyaSayisi > 0);

        if (gecmisKitaplar.Count > 0)
        {
            Console.WriteLine("Ödünç alınmış ve iadesi geçmiş kitaplar:");

            foreach (var kitap in gecmisKitaplar)
            {
                // Eğer iade tarihi geçtiyse, kitabı ekrana yazdır
                if (DateTime.Today > kitap.IadeTarihi)
                {
                    Console.WriteLine($"Kitabın Adı: {kitap.KitabinAdi}, Yazar: {kitap.Yazar}, İade Tarihi: {kitap.IadeTarihi}");
                }
            }
        }
        else
        {
            Console.WriteLine("İade süresi geçmiş kitap bulunmamaktadır.");
        }
    }
    public void KitapOduncAl(string kitapAdi, DateTime oduncAlmaTarihi, int iadeSuresiGun)
    {
        Kitap kitap = KitapAra(kitapAdi);

        if (kitap != null)
        {
            if (kitap.KopyaSayisi > kitap.OduncAlinanKopyaSayisi)
            {
                kitap.OduncAlinanKopyaSayisi++;
                kitap.OduncAlmaTarihi = oduncAlmaTarihi;
                kitap.IadeTarihi = oduncAlmaTarihi.AddDays(iadeSuresiGun);

                Console.WriteLine($"{kitapAdi} adlı kitap ödünç alındı.");
                Console.WriteLine($"Ödünç alma tarihi: {kitap.OduncAlmaTarihi}");
                Console.WriteLine($"İade tarihi: {kitap.IadeTarihi}");
                Console.WriteLine($"Kitap Bilgileri - Adı: {kitap.KitabinAdi}, Yazar: {kitap.Yazar}, ISBN: {kitap.ISBN}");
                Console.WriteLine($"İade edilmesi gereken gün sayısı: {iadeSuresiGun}");
                kitap.KopyaSayisi--;
                Console.WriteLine($"Mevcut kopya sayısı: {kitap.KopyaSayisi}");

            }
            else
            {
                Console.WriteLine($"{kitapAdi} adlı kitap elimizde bulunamadı. Yeterli kopya bulunmamaktadır.");
            }
        }
        else
        {
            Console.WriteLine($"{kitapAdi} adlı kitap kütüphanede bulunmamaktadır.");
        }
    }
}
class Program
{
    static void Main()
    {
        Console.Title = "Kutuphane Sistemi Yönetimi";
        Console.ForegroundColor = ConsoleColor.Yellow;

        Kutuphane kutuphane = new Kutuphane();

        string yeniDosyaYolu = "kutuphane_verisi.txt";
        kutuphane.VeriyiDosyadanYukle(yeniDosyaYolu); // Program başladığında veriler varsa dosyadan yükle

        bool devam = true;

        while (devam)
        {
            Console.WriteLine("\n ****************************** Kütüphane Yönetim Sistemi ******************************");
            Console.WriteLine("1. KİTAP EKLE");
            Console.WriteLine("2. TÜM KİTAPLARI GÖRÜNTÜLE");
            Console.WriteLine("3. KİTAP ARA");
            Console.WriteLine("4. KİTAP ÖDÜNÇ AL");
            Console.WriteLine("5. KİTAP İADE ET");
            Console.WriteLine("6. TARİHİ GEÇMİŞ KİTAPLARI GÖRÜNTÜLE");
            Console.WriteLine("0. ÇIKIŞ\n");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("-----------Lütfen yapmak istediğiniz işlemi seçiniz:");
            Console.ForegroundColor = ConsoleColor.Yellow;

            string secim = Console.ReadLine();

            Console.WriteLine("");
            switch (secim)
            {
                case "1":
                    Console.Write("Kitabın Adı Giriniz: ");
                    string KitapAdi = Console.ReadLine();
                    Console.Write("Yazarın Adını ve Soyadını Giriniz: ");
                    string YazarAdi = Console.ReadLine();
                    Console.Write("ISBN: ");
                    string ISBN = Console.ReadLine();
                    Console.Write("Kopya Sayısı: ");
                    int KopyaSayisi = Convert.ToInt32(Console.ReadLine());

                    // Aynı isimde veya ISBN numarasında kitap olup olmadığını kontrol et
                    if (kutuphane.KitapVarMi(KitapAdi, ISBN))
                    {
                        Console.WriteLine($"Bu isimde veya ISBN numarasında bir kitap zaten var. Lütfen kontrol ediniz.");
                    }
                    else
                    {
                        Kitap yeniKitap = new Kitap { KitabinAdi = KitapAdi, Yazar = YazarAdi, ISBN = ISBN, KopyaSayisi = KopyaSayisi };
                        kutuphane.KitapEkle(yeniKitap);

                        // Eklenen kitapları göster
                        Console.WriteLine($"{KitapAdi} - {YazarAdi} kitap başarıyla eklendi. ");
                        //  kutuphane.TumKitaplariGoruntule();
                    }
                    break;

                case "2":
                    // Tüm Kitapları Görüntüleme işlemi

                    kutuphane.KitapEkle(new Kitap { KitabinAdi = "Gece Yarısı Kütüphanesi", Yazar = "Matt Haig", ISBN = "35", KopyaSayisi = 10 });
                    kutuphane.KitapEkle(new Kitap { KitabinAdi = "Şeker Portakalı", Yazar = "Jose Mauro De Vasconcelos", ISBN = "123", KopyaSayisi = 8 });
                    kutuphane.KitapEkle(new Kitap { KitabinAdi = "Hayvan Çiftliği", Yazar = "George Orwell", ISBN = "87", KopyaSayisi = 5 });

                    kutuphane.TumKitaplariGoruntule();

                    break;

                case "3":
                    // Kitap Arama işlemi
                    Console.Write("Aramak istediğiniz Kitabın adını veya Yazarın adını yazınız:  ");
                    string aramaKelimesi = Console.ReadLine();
                    Kitap arananKitap = kutuphane.KitapAra(aramaKelimesi);

                    if (arananKitap != null)
                    {
                        Console.WriteLine($"Kitabın Adı: {arananKitap.KitabinAdi}, Yazarın Adı: {arananKitap.Yazar}, Kopya Sayısı: {arananKitap.KopyaSayisi}, Ödünç Alınan Kopya Sayısı: {arananKitap.OduncAlinanKopyaSayisi}");
                    }
                    else
                    {
                        Console.WriteLine("Aradığınız kitap kütüphanemizde bulunamamaktadır.");
                    }
                    break;
                case "4":
                    // Kitap Ödünç Alma İşlemi
                    Console.Write("Kitabın Adı veya Yazarın Adını Giriniz: ");
                    string aranacakKitap = Console.ReadLine();
                    DateTime oduncAlmaTarihi = DateTime.Now; // Şu anki tarih ve saat

                    Console.Write("İade Süresi (gün olarak): ");
                    int iadeSuresiGun = Convert.ToInt32(Console.ReadLine());

                    kutuphane.KitapOduncAl(aranacakKitap, oduncAlmaTarihi, iadeSuresiGun);
                    break;
                case "5":
                    // Kitap İade Etme İşlemi
                    Console.Write("Kitabın Adı: ");
                    string iadeEdilecekKitap = Console.ReadLine();
                    kutuphane.KitapIadeEt(iadeEdilecekKitap);
                    break;

                case "6":
                    // Gecmiş Kitapları Görüntüle
                    kutuphane.GecmisKitaplariGoruntule();
                    break;

                case "0":
                    // Veriyi dosyaya kaydet ve çık
                    kutuphane.VeriyiDosyayaKaydet("kutuphane_verisi.txt");
                    devam = false;
                    break;

                default:
                    Console.WriteLine("Geçersiz seçenek. Lütfen tekrar deneyin.");
                    break;
            }
        }
    }
}

