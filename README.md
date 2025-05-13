# 📱 Sosyal Medya Uygulaması – Veri Yapıları Odaklı Web Projesi

Bu proje, veri yapılarının gerçek bir web uygulamasında nasıl kullanılabileceğini göstermek amacıyla geliştirilmiştir. Basit ama işlevsel bir **sosyal medya platformu** simüle edilmiştir. Kullanıcılar kayıt olabilir, giriş yapabilir, gönderi paylaşabilir, yorum yapabilir, arkadaş ekleyebilir ve mesajlaşabilir.

---

## 💡 Proje Amacı

Veri yapılarının gerçek dünya problemlerine nasıl entegre edildiğini göstermek. Web uygulaması içerisinde **Stack**, **Queue**, **Tree**, **Graph** ve **HashSet** gibi yapılar uygun alanlarda kullanılarak örneklendirilmiştir.

---

## 🧱 Katmanlar

### 🖼️ Frontend (Kullanıcı Arayüzü)

- `auth.html` → Giriş ve kayıt formu
- `mainpage.html` → Giriş sonrası karşılama sayfası
- `profile.html`→Kişinin profil ekranı
- `messages.html`→Mesajların gözüktüğü sayfa
- `settings`→Ayarların olduğu sayfa
- `notification.html`→Bildirim sayfası
- `friends.html`→Arkadaşların olduğu sayfa
- `404.css` → Tüm sayfaların ortak stil dosyası
- JavaScript → Sayfa yönlendirme ve form işleme mantığı

### ⚙️ Backend (Veri Yapısı Simülasyonu)

Uygulamanın arka planında (arka uç mantığında) veri yapılarıyla çalışan simülasyon yapısı kurulmuştur:

| Fonksiyon                 | Kullanılan Veri Yapısı |
|--------------------------|------------------------|
| Mesajlaşma               | `Stack`               |
| Gönderi/Bildirim Akışı   | `Queue`               |
| Yorum ve alt yorumlar    | `Tree`                |
| Arkadaşlık İlişkileri    | `Graph`               |
| Kullanıcı Benzersizliği  | `HashSet`             |

---

## 🧠 Kullanılan Veri Yapıları

| Veri Yapısı | Kullanım Yeri               | Neden Seçtik? | Diğerleri Neden Uygun Değil? |
|-------------|-----------------------------|---------------|-------------------------------|
| **Stack**   | Mesajlaşma                  | Son gelen mesajın en üstte görünmesini sağlamak için (LIFO) | Queue ters çalışır, Tree/Graph karmaşık |
| **Queue**   | Gönderi/Bildirim sırası     | Gönderiler sırayla gösterilir (FIFO) | Stack ters çalışır, Tree/Graph gereksiz karmaşık |
| **Tree**    | Yorumlar ve yanıtlar        | Cevaplar hiyerarşik bağlıdır | Stack/Queue bağlantı gösteremez, Graph fazla esnek |
| **Graph**   | Arkadaşlık ilişkileri       | Çift yönlü ilişki kurulumu | Tree tek yönlüdür, Stack/Queue ilişki gösteremez |
| **HashSet** | Kullanıcı ID kontrolü       | Aynı kişinin tekrar kayıt olmasını engeller | Stack/Queue tekrarları engellemez |

---

## ⏱️ Zaman Karmaşıklığı Karşılaştırması

| Veri Yapısı | Ulaşma | Arama | Ekleme | Silme |
|-------------|--------|--------|--------|--------|
| Stack       | O(1)   | O(n)   | O(1)   | O(1)   |
| Queue       | O(1)   | O(n)   | O(1)   | O(1)   |
| Tree        | O(n)   | O(n)   | O(n)   | O(n)   |
| Graph       | O(1)   | O(n)   | O(1)   | O(n)   |
| HashSet     | O(n)   | O(n)   | O(n)   | O(n)   |

---


```
📂 ProjeKlasörü
├── ulustackasp 
├── README.md          
```

---

## 🔄 Yapılar Arası Detaylı Karşılaştırma

### Stack vs Queue

- **Benzerlik:** Lineer yapılardır. Elemanlar sırayla eklenir/çıkarılır.
- **Fark:** Stack → LIFO, Queue → FIFO
- **Uygulama:** Stack (Mesajlaşma), Queue (Gönderi sırası)

### Tree vs Graph

- **Benzerlik:** Düğüm ve bağlantı yapısına sahiptir, DFS/BFS ile gezilebilirler.
- **Fark:** Tree → Hiyerarşik ve tek yönlü, Graph → Karmaşık, döngü içerebilir, çift yönlü
- **Uygulama:** Tree (Yorumlar), Graph (Arkadaş ilişkisi)

---

## 🧑‍💻 Katkı Sağlayanlar

Proje, bir grup çalışması olarak veri yapıları dersi kapsamında geliştirilmiştir.
Hasan Emre Kartal
Beyzanur Postlu
Ceren Karahasan
Yusuf Cihan Yılmaz 
Arda İnanç
---





---

