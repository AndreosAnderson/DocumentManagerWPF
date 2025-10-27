**# DocumentManager**

**## Opis projektu**

DocumentManager to aplikacja desktopowa oparta na WPF (.NET 8), umożliwiająca:

- Import dokumentów i ich pozycji z plików CSV.
- Walidację danych przed zapisaniem.
- Edycję dokumentów i pozycji w interfejsie użytkownika.
- Zapis zmian do bazy danych (Entity Framework Core, SQL Server).
- Filtrowanie i wyszukiwanie dokumentów.
- Eksport danych do plików CSV.

**## Funkcjonalności**

- Import dokumentów i pozycji dokumentów z CSV.
- Walidacja danych CSV przed zapisaniem do bazy.
- Wyświetlanie dokumentów i ich pozycji w dwóch `DataGrid` (górny – dokumenty, dolny – pozycje).
- Edycja dokumentów i pozycji bezpośrednio w interfejsie użytkownika.
- Zapis zmian do bazy danych poprzez przycisk **Save Changes**.
- Filtrowanie dokumentów według typu, imienia, nazwiska lub miasta.
- Odświeżanie widoku danych przyciskiem **Refresh**.
- Obsługa błędów i wyjątków podczas importu i zapisu danych.
- Możliwość eksportu danych do plików CSV (opcjonalnie).

**## Wymagania**

- .NET 8 SDK
- SQL Server (lokalny lub zdalny)  
- Visual Studio 2022 lub nowsze z obsługą WPF i .NET 8
- NuGet packages:
  - CsvHelper
  - Microsoft.EntityFrameworkCore
  - Microsoft.EntityFrameworkCore.SqlServer
  - MaterialDesignThemes.Wpf
  - CommunityToolkit.Mvvm
 
**## Uruchomienie projektu**

1. Sklonuj repozytorium:
   ```bash
   git clone https://github.com/AndreosAnderson/DocumentManager.git
2. Otwórz projekt w Visual Studio:

Plik: DocumentManagerWPF.sln

3. Skonfiguruj połączenie do bazy danych w appsettings.json:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=DocumentManagerDb;Trusted_Connection=True;"
  }
}
```
4. Wykonaj migracje EF Core

dotnet ef database update

**## Strutura projektu**

- DocumentManagerWPF/
  - DocumentManagerWPF.sln       # Główne rozwiązanie
  - DocumentManagerWPF/           # Projekt WPF
    - MainWindow.xaml
    - ImportPreviewWindow.xaml
    - App.xaml
    - ...
  - DocumentManager.Services/     # Logika biznesowa, import CSV, walidacja, zapis zmian
  - DocumentManager.Data/         # Kontekst bazy danych i modele EF Core
  - DocumentManager.Domain/       # Modele domenowe (Document, DocumentItems)
  - README.md                     # Dokumentacja projektu


**## Uwagi dodatkowe**

- Pozycje dokumentów są edytowalne w osobnym DataGrid.

- Możliwe jest eksportowanie danych do CSV (metody w CsvExportService).

- Plik appsettings.json służy do konfiguracji połączenia z bazą.

- Nowo dodane dokumenty po imporcie można wyróżnić lub odświeżyć widok za pomocą przycisku Refresh.

- Błędy walidacji CSV są zgłaszane użytkownikowi w oknie komunikatów.

- Wszystkie wyjątki podczas importu, zapisu czy odczytu są obsłużone i wyświetlane w MessageBox.
