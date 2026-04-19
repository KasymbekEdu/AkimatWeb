# AkimatWeb — Мемлекеттік Әкімдік Сайты

ASP.NET Core MVC (.NET 10) негізіндегі толық функционалды мемлекеттік (Акимат) веб-сайты.

## Технология стэкі
- **ASP.NET Core MVC** (.NET 10)
- **Entity Framework Core 10** (Code First + Migrations)
- **Microsoft SQL Server 2019**
- **ASP.NET Core Identity** (аутентификация + рөлдер)
- **Serilog** (логтау)
- **Repository Pattern + DataManager**

## Модульдер
- 📰 **Жаңалықтар** — CRUD, суреттер, жариялау
- 📢 **Хабарландырулар** — тендер, жол жабылуы, шұғыл хабарлар
- 📝 **Өтініштер** — беру, трекинг нөмірі, мәртебе қадағалау
- ⚙ **Мемлекеттік қызметтер** — санаттар + қызметтер тізімі
- 🔒 **Әкімшілік панель** — толық CRUD барлық мазмұн үшін

## Жылдам іске қосу

### 1. Талаптар
- .NET 10 SDK
- SQL Server 2019 (localhost\MSSQLSERVER)
- Visual Studio 2022

### 2. appsettings.json
```json
"Project": {
  "Database": {
    "ConnectionString": "Server=localhost;Database=AkimatDb;Trusted_Connection=True;..."
  },
  "Company": {
    "CompanyName": "Сіздің қаланың әкімдігі",
    "CompanyPhone": "+7 (XXX) XXX-XX-XX",
    "CompanyEmail": "info@akimat.kz"
  }
}
```

### 3. Миграция
```powershell
# Package Manager Console (Visual Studio)
Add-Migration InitAkimat
Update-Database
```

### 4. Іске қосу
```
F5 (Visual Studio) немесе: dotnet run
```

### 5. Кіру
- URL: `https://localhost:PORT/account/login`
- Email: `admin@akimat.kz`
- Құпия сөз: `Admin123!`

## Маршруттар
| Бет | URL |
|-----|-----|
| Басты бет | `/` |
| Жаңалықтар | `/news` |
| Хабарландырулар | `/announcements` |
| Қызметтер | `/services` |
| Өтініш беру | `/applications/submit` |
| Тексеру | `/applications/track` |
| Кіру | `/account/login` |
| Админ панель | `/admin` |

## Админ панель бөлімдері
| Бөлім | URL |
|-------|-----|
| Басқару тақтасы | `/admin` |
| Өтініштер | `/admin/applications` |
| Жаңалықтар | `/admin/news` |
| Хабарландырулар | `/admin/announcements` |
| Қызметтер | `/admin/services` |
