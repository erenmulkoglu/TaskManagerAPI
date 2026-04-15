# TaskManager - Görev Yönetimi Uygulaması


Bu proje, kullanıcıların görev (todo) oluşturup yönetebileceği görev yönetim sistemidir.  
Backend tarafı ASP.NET Core Web API, frontend tarafı React ve veritabanı olarak SQL Server kullanılmıştır.  
Uygulama Docker ile container ortamında çalışacak şekilde yapılandırılmıştır.


<img width="1912" height="972" alt="image" src="https://github.com/user-attachments/assets/9b0d2746-f778-4b0b-a76e-70e095e7686e" />

<img width="1917" height="960" alt="image" src="https://github.com/user-attachments/assets/b36fa39e-cd77-4198-9587-91a322c3309f" />

<img width="1587" height="900" alt="image" src="https://github.com/user-attachments/assets/f959bdbe-6ad6-4816-a3f5-bde83a50bbe2" />

<img width="1586" height="900" alt="image" src="https://github.com/user-attachments/assets/b4c2fd45-1dc9-4374-82f8-19c8add3515d" />


## Teknoloji Tercihleri

| Katman | Teknoloji | Gerekçe |
|--------|-----------|---------|
| Backend | ASP.NET Core 8 Web API | Mevcut stack (.NET Core) |
| ORM | Entity Framework Core 8 | Code-first migration, LINQ desteği |
| Veritabanı | MSSQL (Docker) |  SQL Server deneyimiyle uyumlu |
| Frontend | React | Hızlı geliştirme, component bazlı yapı |
| Test | xUnit + EF InMemory | Gerçek DB gerektirmeden servis testi |
| DevOps | Docker & Docker Compose | Birçok uygulama tek çatı altından ayağa kaldırıldı |

---

## Çalıştırma

Proje iki farklı şekilde çalıştırılabilir:

### 1. Local Development (Geliştirme Ortamı)

 A. Backend'i çalıştır

- SQL Server (LocalDB veya MSSQL)

**1. Migration (ilk kurulumda)**
```bash
cd TaskManagerAPI
dotnet ef database update
```

**2. Uygulamayı başlat**
```bash
f5 or dotnet run
```

API: https://localhost:44317

Swagger: https://localhost:44317/swagger

B. Frontend'i çalıştır

```
cd TaskManager.React
npm install
npm run dev
```

Frontend: http://localhost:5173


### 2. Docker ile Çalıştırma (Production-like)

Bu yöntem tüm sistemi container içinde ayağa kaldırır.

Gereksinimler
- Docker Desktop

docker-compose.yml 'in olduğu kök (root) dizinde locate ol.

1. Projeyi başlat
```
docker-compose up --build
```

### Gereksinimler
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 22.12+](https://nodejs.org)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

##  Kurulum (Docker ile)

### Projeyi klonla
```bash
git clone https://github.com/erenmulkoglu/TaskManagerAPI.git
cd TaskManagerAPI
docker-compose up --build
```

### 1. Veritabanını başlat
```bash
docker-compose up -d
```


## API Endpoint'leri

| Method | URL | Açıklama |
|--------|-----|---------|
| GET | /api/tasks | Tüm görevleri listele |
| GET | /api/tasks/{id} | Tek görev getir |
| POST | /api/tasks | Yeni görev oluştur |
| PUT | /api/tasks/{id} | Görevi güncelle |
| DELETE | /api/tasks/{id} | Görevi sil |

Konfigürasyon
Connection String (Docker)

## Testler

```bash
cd TaskManager.Tests
dotnet test
```

7 unit test — `TaskService` tüm CRUD operasyonları için EF InMemory ile test edildi.

## Tercihler

- Backend: ASP.NET Core Web API (.NET 8). Güçlü tip güvenliği, yüksek performans sunması ve kurumsal projelerdeki yaygın kullanımı sebebiyle tercih ettim.
- Veritabanı: MS SQL Server ve Entity Framework Core. Code-First yaklaşımı ile veritabanı şemasını kod üzerinden yönetmek ve migration'ları otomatize etmek için kullandım.
- Frontend: React ve Vite. Sürdürülebilir, bileşen tabanlı bir kullanıcı arayüzü inşa etmek için React'i; hızlı derleme süreleri ve modern geliştirici deneyimi sunması sebebiyle de Vite'i seçtim.
- DevOps: Docker ve Docker Compose. Projeyi inceleyecek kişilerin "benim bilgisayarımda çalışmıyor" gibi sorunlar yaşamadan, tek tuşla tüm sistemi bağımlılıklarıyla beraber ayağa kaldırabilmesi için konteyner mimarisi kurguladım.

## Öne Çıkan Geliştirme Kararları

Projede istenen temel CRUD işlemlerinin yanı sıra, kodun kalitesini ve sürdürülebilirliğini artırmak için aşağıdaki yapıları uyguladım:

- Katmanlı Mimari (Separation of Concerns): API controller'ları olabildiğince sade bırakılmış, tüm iş mantığı (business logic) `TaskService` katmanına soyutlanmış ve Dependency Injection ile sisteme entegre edilmiştir.    
- Asenkron Programlama: Veritabanı işlemlerinde thread bloklanmalarını (bottleneck) önlemek için uçtan uca `async/await` mimarisi kullanılmıştır.    
- Loglama Middleware: Her gelen HTTP isteğinin metodunu, yolunu ve işlem süresini milisaniye cinsinden hesaplayıp konsola yazdıran özel bir loglama aracı (middleware) yazılmıştır.    
- Global Exception Handler: Uygulama genelinde oluşabilecek beklenmedik hataları (500 Internal Server Error) merkezi bir noktada yakalayıp istemciye standart bir JSON formatında dönen hata yakalayıcı eklenmiştir.    
- Birim Testleri (Unit Tests): İş mantığının kalbi olan servis katmanı için xUnit ve InMemory Database kullanılarak uçtan uca testler yazılmıştır.

## Mimari Kararlar

- **Servis katmanı**: Controller'lar sadece HTTP'yi yönetir; iş mantığı `TaskService`'te.
- **Dependency Injection**: `ITaskService` interface'i üzerinden inject edilir — test edilebilirlik ve loose coupling için.
- **DbContext lifetime**: `Scoped` — her HTTP isteğinde yeni instance, captive dependency sorununu önler.
- **Middleware sırası**: GlobalExceptionHandler → RequestLogging → Controller. Exception handler en dışta olmalı ki tüm hataları yakalasın.
- **DTO kullanımı**: Domain modeli API'ye doğrudan açılmadı; model değişiklikleri API kontratını etkilemez.

---


## Proje Yapısı

TaskManagerAPI/

TaskManagerAPI/ # ASP.NET Core Web API

Controllers/ # HTTP endpoint'leri

Services/ # İş mantığı (ITaskService + TaskService)

Data/ # EF Core DbContext

Models/ # Entity ve DTO'lar

Middleware/ # RequestLogging + GlobalExceptionHandler

TaskManager.Tests/ # xUnit testleri (7 test)

TaskManager.React/ # React + Vite frontend

docker-compose.yml # MSSQL + API + Frontend container yapılandırması.
