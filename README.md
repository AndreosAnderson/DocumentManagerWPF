DocumentManagerWPF

**Description**

DocumentManager to prosta aplikacja desktopowa oparta na WPF (.NET 8). Umożliwia import dokumentów z plików CSV, ich edycję i walidację przed zapisaniem do bazy danych. Dodatkowo pozwala filtrować, wyszukiwać i eksportować dane w intuicyjnym interfejsie użytkownika.

**Technologies**

- .NET 8
- WPF (Windows Presentation Foundation)
- Entity Framework Core
- SQL Server
- CSV (import/eksport danych)
- .NET 8 SDK
- NuGet packages:
  - CsvHelper
  - Microsoft.EntityFrameworkCore
  - Microsoft.EntityFrameworkCore.SqlServer
  - MaterialDesignThemes.Wpf

**Installation**

- Sklonuj repozytorium:
```bash
git clone https://github.com/AndreosAnderson/DocumentManagerWPF.git
```
- Otwórz projekt w Visual Studio 2022 lub nowszym.
- Upewnij się, że masz zainstalowany .NET 8 SDK.
- Skonfiguruj połączenie do swojej bazy danych SQL Server w pliku appsettings.json lub w konfiguracji kontekstu EF.
- Wykonaj migracje bazy danych:
```bash
dotnet ef database update
```
**How to Run**

Uruchom projekt z poziomu Visual Studio (F5) lub poleceniem:
```bash
dotnet run
```
W aplikacji możesz:
-Zaimportować dane z pliku CSV
-Edytować dokumenty i ich pozycje
-Zatwierdzić zmiany do bazy danych
-Filtrować, wyszukiwać i eksportować dane
