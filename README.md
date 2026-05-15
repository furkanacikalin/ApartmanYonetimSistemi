🏢 Akıllı Apartman Yönetim Sistemi (ERP)

Bu proje, apartman ve site yönetim süreçlerini dijitalleştiren, yöneticilerin iş yükünü azaltırken sakinlerin taleplerine hızlı ve akıllı çözümler sunan bir Kurumsal Kaynak Planlama (ERP) uygulamasıdır.

🎯 Projenin Amacı

    Geleneksel yönetim sistemlerinde sakinden gelen talepler, önem sırasına bakılmaksızın sadece bir liste olarak yöneticinin önüne düşer. Bu projenin temel amacı:
    Yönetim süreçlerini tek bir merkezden yürütmek (Aidat, Borç, Talep).
    Yapay Zeka kullanarak, gelen onlarca talep arasından "hayati ve acil" olanları (gaz kaçağı, su baskını vb.) otomatik olarak tespit edip en üste taşımak.
    Yöneticinin zamanını daha verimli kullanmasını sağlayan bir Karar Destek Mekanizması oluşturmaktır.

🛠️ Kullanılan Teknolojiler

Backend & Frontend

    C# & .NET 8: Modern, hızlı ve güvenli uygulama altyapısı.
    Blazor Server: Sayfa yenilenmeden interaktif ve akıcı bir kullanıcı deneyimi sağlayan frontend mimarisi.
    Entity Framework Core: Veritabanı işlemlerini nesne yönelimli (ORM) olarak yöneten yapı.
Veri Yönetimi & Güvenlik

    SQLite: Kurulum gerektirmeyen, taşınabilir ve hızlı dosya tabanlı veritabanı.
    PBKDF2 Şifreleme: Kullanıcı şifrelerini en üst düzey güvenlik standartlarında saklayan hashing algoritması.
    RBAC (Yetkilendirme): Yönetici ve Sakin rollerine göre ayrılmış panel erişimleri.
Yapay Zeka & Ödeme

    Groq AI / Gemini API: Gelen metinleri analiz eden yapay zeka motorları.
    Iyzico API: Kredi kartı ile aidat ödemelerini sağlayan ödeme geçidi.

⚙️ Proje Tekniği ve İşleyiş

    Proje, Modüler ve Servis Odaklı bir mimariyle geliştirilmiştir:
    Talep İşleme: Kullanıcı bir şikayet yazdığında, bu metin arka planda yapay zeka servisine gönderilir.
    Anlamsal Analiz: Yapay zeka, metnin bağlamını analiz ederek 1 (Düşük) ile 5 (Kritik) arasında bir puan üretir.
    Dinamik Sıralama: Veritabanına bu puanla kaydedilen talepler, yönetici ekranında akıllı bir algoritmaya göre (en acilden en normale) sıralanır.
    Finansal Akış: Sakin tarafında oluşan borçlar, Iyzico üzerinden ödenir ve sistemde anlık olarak "Ödendi" durumuna geçer.

📋 Kurulum Adımları

    Projeyi kendi bilgisayarınızda çalıştırmak için şu adımları izleyin:
    Dosyaları İndirin: Projeyi bilgisayarınıza klonlayın veya indirin.
    Konfigürasyon: appsettings.example.json dosyasının adını appsettings.json olarak değiştirin ve gerekli API Key alanlarını doldurun.
    Veritabanı Kurulumu: Visual Studio içindeki Package Manager Console üzerinden şu komutu çalıştırarak tabloları oluşturun: Update-Database
    Çalıştır: Visual Studio üzerinden F5 tuşuna basarak uygulamayı başlatın.
