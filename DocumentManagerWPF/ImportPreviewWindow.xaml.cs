using DocumentManager.Services;
using DocumentManager.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DocumentManagerWPF
{
    public partial class ImportPreviewWindow : Window
    {
        private readonly ICsvImportService _csvImportService;
        private readonly string _documentsFilePath;
        private readonly string _documentItemsFilePath;
        private List<DocumentCsvModel> _documents;
        private List<DocumentItemCsvModel> _items;

        private bool _showOnlyInvalidDocs = false;
        private bool _showOnlyInvalidItems = false;

        public ImportPreviewWindow(
            ICsvImportService csvImportService,
            List<DocumentCsvModel> documents,
            List<DocumentItemCsvModel> items,
            string documentsFilePath,
            string documentItemsFilePath)
        {
            InitializeComponent();
            _csvImportService = csvImportService;
            _documents = documents;
            _items = items;
            _documentsFilePath = documentsFilePath;
            _documentItemsFilePath = documentItemsFilePath;

            RefreshDataGrids();
        }

        private void RefreshDataGrids()
        {
            var filteredDocs = _showOnlyInvalidDocs
                ? _documents.Where(d => string.IsNullOrWhiteSpace(d.Type)
                    || string.IsNullOrWhiteSpace(d.FirstName)
                    || string.IsNullOrWhiteSpace(d.LastName)
                    || string.IsNullOrWhiteSpace(d.City)).ToList()
                : _documents;

            var filteredItems = _showOnlyInvalidItems
                ? _items.Where(i => i.DocumentId <= 0
                    || i.Ordinal <= 0
                    || string.IsNullOrWhiteSpace(i.Product)
                    || i.Quantity <= 0
                    || i.Price < 0
                    || i.TaxRate < 0).ToList()
                : _items;

            DocumentsDataGrid.ItemsSource = filteredDocs;
            ItemsDataGrid.ItemsSource = filteredItems;
        }

        private void ToggleDocsFilterButton_Click(object sender, RoutedEventArgs e)
        {
            _showOnlyInvalidDocs = !_showOnlyInvalidDocs;
            ToggleDocsFilterButton.Content = _showOnlyInvalidDocs ? "Pokaż wszystkie" : "Pokaż tylko błędne";
            RefreshDataGrids();
        }

        private void ToggleItemsFilterButton_Click(object sender, RoutedEventArgs e)
        {
            _showOnlyInvalidItems = !_showOnlyInvalidItems;
            ToggleItemsFilterButton.Content = _showOnlyInvalidItems ? "Pokaż wszystkie" : "Pokaż tylko błędne";
            RefreshDataGrids();
        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            bool hasInvalidDocs = _documents.Any(d => string.IsNullOrWhiteSpace(d.Type)
                || string.IsNullOrWhiteSpace(d.FirstName)
                || string.IsNullOrWhiteSpace(d.LastName)
                || string.IsNullOrWhiteSpace(d.City));

            bool hasInvalidItems = _items.Any(i => i.DocumentId <= 0
                || i.Ordinal <= 0
                || string.IsNullOrWhiteSpace(i.Product)
                || i.Quantity <= 0
                || i.Price < 0
                || i.TaxRate < 0);

            if (hasInvalidDocs || hasInvalidItems)
            {
                var result = MessageBox.Show(
                    "Niektóre rekordy zawierają błędy. Czy na pewno chcesz kontynuować import?",
                    "Uwaga",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                    return;
            }

            try
            {
                await _csvImportService.ImportDocumentsAsync(_documentsFilePath, _documentItemsFilePath);
                MessageBox.Show("Dane zostały zaimportowane do bazy.", "Sukces",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas importu:\n\n{ex.Message}\n\nINNER:\n{ex.InnerException?.Message}\n\nSTACK:\n{ex}",
                    "Błąd",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
