# 📬 NotikaIdentityEmail - Tek Katmanlı Gelişmiş Mesajlaşma Platformu

NotikaIdentityEmail; **.NET 8** altyapısı üzerinde, **Tek Katmanlı (Monolithic Architecture)** MVC mimarisiyle inşa edilmiş gelişmiş bir iç mesajlaşma ve e-posta platformudur. 

Proje; veri tabanı, iş mantığı ve arayüz katmanlarını tek bir esnek yapıda birleştirirken, **ASP.NET Core Identity** altyapısını **Google OAuth 2.0** ve **JWT (JSON Web Token)** doğrulama şemalarıyla harmanlayarak hibrit bir kimlik yönetim ekosistemi sunar.

---

## 🚀 Öne Çıkan Teknik Özellikler

* **🏗️ Tek Katmanlı (Monolithic) Güçlü Mimari:** Projenin tüm veri tabanı (Context/Entities), iş mantığı (Controllers) ve arayüz (Views/Components) süreçleri tek bir çatı altında, bakımı kolay ve performanslı bir yapıda modüler olarak kurgulanmıştır.
* **🔐 Hibrit Kimlik Doğrulama:** Web arayüzü oturum yönetimi için geleneksel `Cookie Authentication` tercih edilirken, proje içerisindeki token bazlı güvenlik ve doğrulama süreçleri için `JWT Bearer` şeması aynı anda konfigüre edilmiştir.
* **🌐 Google OAuth 2.0 Entegrasyonu (Sosyal Giriş):** Kullanıcıların harici bir şifre oluşturmaya gerek duymadan, güvenli bir şekilde Google hesaplarıyla sisteme tek tıkla kayıt ve giriş yapabilmesi sağlanmıştır.
* **🛡️ Rol Tabanlı Yetkilendirme (RBAC):** Dinamik rol yönetimi mekanizması ile kullanıcıların erişim yetkileri controller ve view düzeyinde tam denetim altına alınmıştır.
* **⚡ Performans Odaklı LINQ Joins:** Gelen ve giden kutularında `DefaultIfEmpty()` (Left Join) kullanılarak; ilişkili kategorisi silinmiş olsa dahi sistemin kesintiye uğramaması (`NullReferenceException` oluşmaması) garanti altına alınmıştır.
* **📊 Sunucu Tarafı Sayfalama (Server-Side Pagination):** Yoğun mesaj trafiklerinde belleği yormamak adına `X.PagedList` kütüphanesi ile sayfalama optimizasyonu yapılmıştır.
* **🧩 Modüler ViewComponent Yapısı:** Sol menü, gelen kutusu özetleri ve kullanıcı panelleri gibi sık tekrarlanan arayüz blokları `ViewComponent` mimarisiyle geliştirilmiştir.
* **⚠️ Merkezi Hata Yönetimi:** `401 (Yetkisiz)`, `403 (Yasaklı)` ve `404 (Bulunamadı)` hataları yakalanarak projeye özel tasarlanmış hata sayfalarına dinamik olarak yönlendirilir.

---

## 🛠️ Kullanılan Teknolojiler & Kütüphaneler

### Arka Plan (Backend)
* **Framework:** .NET 8.0+ / ASP.NET Core MVC (Single Project Layout)
* **Veri Tabanı & ORM:** Entity Framework Core (Code-First) & MS SQL Server
* **Kimlik & Güvenlik:** * `Microsoft.AspNetCore.Identity` (Kullanıcı ve Rol Yönetimi)
  * `Microsoft.AspNetCore.Authentication.JwtBearer` (Token Tabanlı Kimlik Doğrulama)
  * `Microsoft.AspNetCore.Authentication.Google` (Harici Sağlayıcı Girişi)
* **Doğrulama:** `CustomIdentityValidator` (Özelleştirilmiş Türkçe kimlik doğrulama hata mesajları)

### Ön Yüz (Frontend)
* **Tema/Şablon:** Notika Admin Premium Dashboard & Colorlib Login V4 Layout
* **Arayüz Teknolojileri:** Bootstrap, FontAwesome 4.7, Material Design Iconic Font, jQuery, Animsition

---

## 📂 Çekirdek Veri Modelleri (Entities)

Code-First yaklaşımıyla hazırlanan veri tabanı mimarisi, ilişkisel veri bütünlüğünü koruyacak şekilde 4 temel entity ve Identity sınıfları üzerine kurulmuştur:
* **`AppUser` (IdentityUser):** Kullanıcı profili, ad, soyad ve sistemşel eşleşmeler.
* **`Message`:** Gelen ve giden e-postaların içerik, gönderici, alıcı ve tarih bilgileri.
* **`Category`:** Mesajların (İş, Kişisel, Finans vb.) gruplandırılmasını sağlayan yapı.
* **`Notification`:** Kullanıcılara gönderilen anlık sistem bildirimleri.
* **`Comment`:** Mesajlar veya sistem içerikleri altındaki etkileşim satırları.

---

## 🏗️ Proje Model Mimarisi ve ViewModel Analizi

Temiz kod (Clean Code) prensiplerine uygun olarak veri taşıma ve form yönetim modelleri (`ViewModels`) sorumluluklarına göre katmanlandırılmış ve tek tek şu görevleri üstlenmiştir:

### 📁 ForgetPasswordModels (Şifre Yönetim Süreçleri)
* **`ForgetPasswordViewModel.cs`:** Kullanıcının şifresini unuttuğu durumlarda, sistemde kayıtlı e-posta adresini güvenli bir şekilde sunucuya taşımak ve şifre sıfırlama linki talebi oluşturmak için kullanılır.
* **`ResetPasswordViewModel.cs`:** E-posta adresine gelen doğrulama token'ı ile birlikte kullanıcının yeni şifresini ve yeni şifre tekrarını alarak veritabanında şifre güncelleme işlemini gerçekleştiren modeldir.

### 📁 IdentityModels (Kullanıcı ve Yetkilendirme Süreçleri)
* **`CreateRoleViewModel.cs`:** Sistem yöneticisinin (Admin) yeni kullanıcı rolleri (örn: Yönetici, Personel, Standart Kullanıcı) tanımlayabilmesi için gerekli olan rol adı verisini taşır.
* **`CustomIdentityValidator.cs`:** ASP.NET Core Identity kütüphanesinin varsayılan İngilizce hata mesajlarını (şifre uzunluğu, büyük harf zorunluluğu vb.) Türkçe diline çeviren ve kuralları özelleştiren doğrulama sınıfıdır.
* **`RegisterUserViewModel.cs`:** Yeni kullanıcı kayıt formundaki Ad, Soyad, E-posta, Kullanıcı Adı, Şifre ve Şifre Tekrar verilerini eşleştirerek kullanıcı oluşturma sürecini yönetir.
* **`RoleAssignViewModel.cs`:** Mevcut kullanıcıların hangi rollere sahip olduğunu listelemek ve kullanıcılara yeni roller atayıp kaldırmak (Check/Uncheck durumları) için tasarlanmıştır.
* **`UpdateRoleViewModel.cs`:** Sistemde daha önceden tanımlanmış olan rollerin isimlerini güncellemek veya düzenlemek amacıyla kullanılır.
* **`UserEditViewModel.cs`:** Giriş yapmış olan kullanıcının kendi profil bilgilerini (isim, soyisim, e-posta, mevcut şifre güncelleme) değiştirebilmesi için form verilerini bağlar.
* **`UserLoginViewModel.cs`:** Kullanıcının sisteme güvenli giriş yapabilmesi için kullanıcı adı ve şifre bilgilerini alıp `SignInManager` ile kimlik kontrolü sağlayan modeldir.

### 📁 JwtModels (Token Tabanlı Kimlik Doğrulama)
* **`JwtSettingsModel.cs`:** `appsettings.json` dosyasında yer alan JWT parametrelerini (Issuer, Audience, Secret Key) kod tarafında nesnel olarak yönetebilmek için kullanılan ayar modelidir.
* **`SimpleUserViewModel.cs`:** JWT token üretimi esnasında veya token doğrulanırken kullanıcıya ait temel bilgileri (Id, Kullanıcı Adı, Rol) minimum kaynak tüketimiyle taşımak için kullanılan hafif (lightweight) bir modeldir.

### 📁 MessageViewModels (E-Posta Trafiği ve İlişkisel Veriler)
* **`MessageListWithUsersInfoViewModel.cs`:** Gelen kutusu ve giden kutusu listelerinde mesajların id, konu ve tarih gibi özet verilerini, mesajı gönderen veya alan kullanıcıların detaylı profil bilgileriyle birleştirerek sunan listeleme modelidir.
* **`MessageWithReceiverInfoViewModel.cs`:** Giden kutusundaki (Sendbox) mesaj detaylarında, mesaj içeriğiyle birlikte mesajı alan (alıcı) kullanıcının bilgilerini ve mesajın kategorisini tek bir nesnede birleştiren modeldir.
* **`MessageWithSenderInfoViewModel.cs`:** Gelen kutusundaki (Inbox) mesaj detaylarında, gelen mesajın gövdesiyle birlikte mesajı gönderen kullanıcının ad, soyad bilgilerini ve mesaj kategorisini arayüze taşımak için LINQ Join sorgularıyla beslenen modeldir.

---

## 📸 Proje Ekran Görüntüleri (Arayüz Panelleri)

### 🔐 Kimlik Doğrulama & Kullanıcı Yönetimi

#### Kullanıcı Giriş Ekranı (Login V4)
<img width="979" height="867" alt="Login" src="https://github.com/user-attachments/assets/1e3386f4-f84a-4cbb-97e4-ee0754b4f56e" />

#### Yeni Kullanıcı Kayıt Ekranı (Register)
<img width="906" height="847" alt="Register" src="https://github.com/user-attachments/assets/d9cc6b9c-d2d8-4c0d-9c98-57349f8fc564" />

#### Kullanıcı Yönetim Paneli
<img width="916" height="839" alt="User" src="https://github.com/user-attachments/assets/de9dca7a-de80-43a5-a0a4-c793296c5b58" />

#### Profil Düzenleme ve Güncelleme Ekranı
<img width="1140" height="986" alt="Profile" src="https://github.com/user-attachments/assets/a12deca5-afa6-432a-815d-b77d0c8ae377" />

#### Identity Dinamik Rol Yönetimi
<img width="1135" height="1003" alt="Roller" src="https://github.com/user-attachments/assets/12ef5d89-7943-4828-a648-5eeb96d34dcd" />

---

### 📬 E-Posta & Mesajlaşma Akışı

#### Gelen Kutusu (Inbox)
<img width="1309" height="1079" alt="Inbox" src="https://github.com/user-attachments/assets/2c937b3a-bc51-4ad6-bcf6-4640f2223c43" />

#### Giden Kutusu (SendBox)
<img width="1227" height="1079" alt="SendBox" src="https://github.com/user-attachments/assets/41183a37-b761-47e6-9172-bec74a685be7" />

#### Kategori Yönetim Paneli
<img width="1071" height="788" alt="Category" src="https://github.com/user-attachments/assets/567d54b0-9077-4630-9c8b-442225a64291" />

---

### 💬 Yorum & Bildirim Etkileşimleri

#### Yorum Listesi Paneli
<img width="1272" height="760" alt="CommentList" src="https://github.com/user-attachments/assets/b58d0ced-ab07-4e81-94df-e007eaf25b08" />

#### Yeni Yorum Ekleme Ekranı
<img width="876" height="944" alt="Comment" src="https://github.com/user-attachments/assets/169ef4cb-8af8-4f53-8ebb-8e091ad7848c" />

---

### ⚠️ Merkezi Hata Sayfaları (Global Error Custom Pages)

#### 401 Yetkisiz Erişim Hatası
<img width="728" height="340" alt="401" src="https://github.com/user-attachments/assets/c375fc01-650f-4acd-8ada-2c95418c12a0" />

#### 403 Erişim Engellendi Hatası
<img width="651" height="340" alt="403" src="https://github.com/user-attachments/assets/4e070b5f-81f2-4774-a765-eacb35cf3a86" />

#### 404 Sayfa Bulunamadı Hatası
<img width="815" height="487" alt="404" src="https://github.com/user-attachments/assets/fe998360-0526-40bb-be7c-b76f66ef9dff" />

---
