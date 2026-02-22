# KonsolcumApi 🎮

KonsolcumApi, modern e-ticaret ihtiyaçlarını karşılamak üzere tasarlanmış, ölçeklenebilir ve sürdürülebilir bir backend çözümüdür. Bu proje, staj dönemim boyunca kurumsal yazılım geliştirme standartları (Clean Architecture, CQRS, vb.) uygulanarak geliştirilmiştir.

## 🏗️ Mimari Yapı (Clean Architecture)

Proje, bağımlılıkları minimize eden ve test edilebilirliği artıran **Onion Architecture** prensiplerine göre dört ana katmandan oluşmaktadır:

* **Core (Application & Domain):** İş mantığı, varlıklar (Entities), DTO'lar ve CQRS desenine dayalı komut/sorgu yapılarını içerir.
* **Infrastructure:** E-posta gönderimi (IMailService) ve JWT token yönetimi gibi dış servis entegrasyonlarını sağlar.
* **Persistence:** Veritabanı işlemleri, repository desenleri ve Entity Framework Core konfigürasyonlarını yönetir.
* **API (Presentation):** Dış dünyaya açılan endpoint'ler ve yetkilendirme katmanıdır.

## ✨ Öne Çıkan Özellikler ve Modüller

### 🔐 Kimlik Doğrulama ve Güvenlik
* **JWT & Refresh Token:** Güvenli oturum yönetimi için JWT tabanlı kimlik doğrulama ve kesintisiz erişim için Refresh Token mekanizması.
* **Şifre Yönetimi:** Güvenli şifre sıfırlama, reset token doğrulama ve şifre güncelleme süreçleri.
* **Dinamik Yetkilendirme (RBAC):** Rol tabanlı erişim kontrolü. Kullanıcılara rol atama ve endpoint'lere dinamik yetki tanımlama özellikleri.

### 🛒 Sepet ve Sipariş Yönetimi
* **Sepet İşlemleri:** Ürün ekleme, miktar güncelleme ve sepetten ürün kaldırma fonksiyonları.
* **Sipariş Süreçleri:** Sipariş oluşturma, detaylı listeleme ve sipariş tamamlandığında mail ile bilgilendirme.

### 📡 Real-Time (Anlık) Veri Güncelleme
* **SignalR Hubs:** Kategori, Ürün ve Sipariş verilerinde yapılan değişikliklerin bağlı istemcilere anlık olarak yansıtılması.

### 📁 Kategori ve Dosya Yönetimi
* **Dinamik Kategoriler:** CRUD operasyonları ve anlık hub bildirimleri ile kategori yönetimi.
* **Görsel Yönetimi:** Ürünlere ait dosya ve görsellerin yüklenmesi, listelenmesi.

## 🛠️ Teknik Altyapı

* **.NET Core 8**
* **Entity Framework Core**
* **MediatR (CQRS Deseni)**
* **SignalR (Gerçek Zamanlı İletişim)**
* **FluentValidation (Veri Doğrulama)**
* **AutoMapper (Nesne Eşleme)**
