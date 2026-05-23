# EduDesk - Mikroservis Tabanlı Eğitim ve Destek Yönetim Sistemi

Bu proje, staj bitirme ödevi kriterlerine uygun olarak tamamen **Mikroservis Mimarisi (Microservices)** prensipleriyle ve **Event-Driven (Olay Güdümlü)** iletişim altyapısıyla geliştirilmiştir.

##  Mimari Yapı ve Bileşenler
Sistem, birbirleriyle gevşek bağlı (loosely coupled) çalışan mikroservisler ve merkezi bir kapıdan (API Gateway) oluşmaktadır:

1. **ApiGateway (Port: 7200):** Tüm dış istekleri tek merkezden karşılayan ve yönlendirmeleri yöneten **YARP (Yet Another Reverse Proxy)** tabanlı API Gateway servisidir.
2. **IdentityService (Port: 5001):** Çoklu kiracılı (Multi-tenant) yapıya uygun kullanıcı kayıt (`Register`) ve giriş (`Login`) işlemlerini yöneten servisidir. Şifreler **BCrypt** ile hash'lenerek `identity.db` veritabanında saklanır ve başarılı girişlerde **JWT (JSON Web Token)** üretir.
3. **SupportService (Port: 5003):** Kullanıcıların destek taleplerini (Ticket) yöneten servistir. Gelen talepler dökümandaki kriterlere uygun olarak gerçek zamanlı sıralanır.
4. **LearningService & WorkerService:** Eğitim süreçlerini, arka plan işlerini ve asenkron event akışlarını yöneten yardımcı servislerdir.

##  Geliştirilen Özel Algoritmalar ve Mekanizmalar

### 1. Bracket Öncelik Algoritması (SupportService)
Gelen destek taleplerinin kuyruktaki öncelik sırası, dökümanda belirtilen parametreler doğrultusunda şu matematiksel formül ile gerçek zamanlı hesaplanmaktadır:

$$\text{Öncelik Skoru} = (\text{Aciliyet Seviyesi} \times W_1) + (\text{Kalan SLA Süresi} \times W_2) + (\text{Tenant Plan Katsayısı} \times W_3)$$

* Kuyruk, bu skora göre **en yüksek öncelikten en düşüğe doğru** dinamik olarak sıralanmaktadır.

### 2. Güvenli Sınav Giriş Mekanizması (Zaman Sınırlı URL)
Öğrencilerin sınav güvenliğini sağlamak amacıyla dökümandaki senaryoya uygun **Zaman Sınırlı ve Tek Kullanımlık (Presigned URL)** erişim token yapısı kurgulanmıştır. 
* Üretilen URL, içerdiği zaman damgası (Timestamp) ve kriptografik imza sayesinde sadece **belirlenen süre (örn. 5 dakika) boyunca** geçerlidir. Süre dolduğunda veya imza uyuşmadığında sistem erişimi doğrudan engeller.

### 3. Çift İşlem Engelleme (Idempotency Key)
Ağ gecikmeleri veya kullanıcının "Sınavı Başlat" ya da "Bilet Oluştur" butonlarına çift tıklaması gibi durumlarda veritabanında mükerrer kayıt oluşmasını engellemek adına **Idempotency Key** mekanizması entegre edilmiştir. Benzersiz bir istek anahtarı ile gelen mükerrer çağrılar sistem tarafından yakalanarak elenir.

##  Hazır Test Hesapları (Seed Data)
Sistemin ilk kurulumunda jüri testlerinin kolaylıkla yapılabilmesi amacıyla aşağıdaki test hesapları otomatik olarak veritabanına beslenmektedir (Seed Data):

* **Sistem Yöneticisi (Admin):** `admin@edudesk.com` / `Admin123!`
* **Eğitmen (Instructor):** `instructor@edudesk.com` / `Instructor123!`
* **Öğrenci (Student):** `student@edudesk.com` / `Student123!`

##  Projeyi Çalıştırma Adımları
1. Çözüm (`EduDesk.sln`) Visual Studio ile açılır.
2. Sağ taraftaki Çözüm Gezgini (Solution Explorer) üzerinden sırasıyla **ApiGateway** ve **IdentityService** projelerine sağ tıklanarak `Hata Ayıklama (Debug) > Yeni Örnek Başlat (Start New Instance)` denilir.
3. Servislerin başarıyla ayağa kalktığı konsol ekranlarından teyit edilir.

##  API Testleri ve Dokümantasyon
* **Swagger UI:** `IdentityService` bağımsız test arayüzüne tarayıcıdan şu adresten erişilebilir:
    `http://localhost:5001/swagger/index.html`
* **HTTP Client Dosyaları:** İsteklerin hızlıca test edilebilmesi için proje dizini içerisinde hazır `IdentityService.http` dosyası yer almaktadır. Bu dosya üzerinden `POST /register` ve `POST /login` entegrasyon testleri doğrudan tetiklenebilir.