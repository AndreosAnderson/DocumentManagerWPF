using DocumentManager.Domain;
using DocumentManager.Services.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DocumentManagerWPF
{
    public partial class MainWindow : Window
    {
        private readonly ICsvImportService _csvImportService;
        private List<Document> _documents;
        private List<DocumentItems> _documentItems;
        private Document? _selectedDocument;

        public MainWindow(ICsvImportService csvImportService)
        {
            InitializeComponent();
            _csvImportService = csvImportService;
            _documents = new();
            _documentItems = new();
            _ = LoadDocumentsAsync();
        }

        private async Task LoadDocumentsAsync()
        {
            try
            {
                _documents = await _csvImportService.GetDocumentsAsync();
                DocumentsDataGrid.ItemsSource = _documents;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania dokumentów: {ex.Message}", "Błąd",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DocumentsDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (DocumentsDataGrid.SelectedItem is not Document selectedDoc)
                return;

            _selectedDocument = selectedDoc;
            try
            {
                _documentItems = await _csvImportService.GetDocumentItemsAsync(selectedDoc.Id);
                ItemsDataGrid.ItemsSource = _documentItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania pozycji dokumentu: {ex.Message}", "Błąd",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await _csvImportService.SaveChangesAsync(_documents, _documentItems);
                MessageBox.Show("Zmiany zostały zapisane w bazie danych.", "Sukces",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Błąd walidacji danych: {ex.Message}", "Błąd danych",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd podczas zapisu: {ex.Message}", "Błąd",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadDocumentsAsync();
            ItemsDataGrid.ItemsSource = null;
        }

        private async void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Wybierz dwa pliki CSV:\n\n1️. Dokumenty\n2️. Pozycje dokumentów",
                "Import CSV", MessageBoxButton.OK, MessageBoxImage.Information);

            var documentsDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Wybierz plik dokumentów"
            };
            if (documentsDialog.ShowDialog() != true)
                return;

            var itemsDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Wybierz plik pozycji dokumentów"
            };
            if (itemsDialog.ShowDialog() != true)
                return;

            try
            {
                var (documents, items) = await _csvImportService.ReadCsvFilesAsync(documentsDialog.FileName, itemsDialog.FileName);
                var previewWindow = new ImportPreviewWindow(_csvImportService, documents, items, documentsDialog.FileName, itemsDialog.FileName)
                {
                    Owner = this
                };

                if (previewWindow.ShowDialog() == true)
                {
                    await LoadDocumentsAsync();
                    ItemsDataGrid.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd importu: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var query = SearchBox.Text?.Trim().ToLower();
            if (string.IsNullOrEmpty(query))
            {
                DocumentsDataGrid.ItemsSource = _documents;
                return;
            }

            var filtered = _documents.Where(d =>
                (d.Id.ToString().Contains(query)) ||
                (d.Type?.ToLower().Contains(query) ?? false) ||
                (d.Date.ToString("yyyy-MM-dd").Contains(query)) ||
                (d.FirstName?.ToLower().Contains(query) ?? false) ||
                (d.LastName?.ToLower().Contains(query) ?? false) ||
                (d.City?.ToLower().Contains(query) ?? false)
            ).ToList();

            DocumentsDataGrid.ItemsSource = filtered;
        }

        private async void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialogDocuments = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv",
                    Title = "Zapisz plik z dokumentami",
                    FileName = "documents_export.csv"
                };
                if (dialogDocuments.ShowDialog() != true)
                    return;

                var dialogItems = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv",
                    Title = "Zapisz plik z pozycjami dokumentów",
                    FileName = "document_items_export.csv"
                };
                if (dialogItems.ShowDialog() != true)
                    return;

                await _csvImportService.ExportDataToCsvAsync(dialogDocuments.FileName, dialogItems.FileName);
                MessageBox.Show("Eksport zakończony sukcesem.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas eksportu danych: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
