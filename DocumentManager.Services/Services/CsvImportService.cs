using CsvHelper;
using CsvHelper.Configuration;
using DocumentManager.Data;
using DocumentManager.Domain;
using DocumentManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManager.Services
{
    public class CsvImportService : ICsvImportService
    {
        private readonly DocumentDbContext _context;

        public CsvImportService(DocumentDbContext context)
        {
            _context = context;
        }

        public async Task ImportDocumentsAsync(string documentsCsvPath, string documentItemsCsvPath)
        {
            var documentsCsv = ReadCsv<DocumentCsvModel>(documentsCsvPath);
            var itemsCsv = ReadCsv<DocumentItemCsvModel>(documentItemsCsvPath);

            ValidateCsvData(documentsCsv, itemsCsv);

            var existingDocs = await _context.Documents.ToListAsync();
            var existingItems = await _context.DocumentItems.ToListAsync();

            int docsAdded = 0, docsUpdated = 0;
            int itemsAdded = 0, itemsUpdated = 0;


            foreach (var d in documentsCsv)
            {
                var existing = existingDocs.FirstOrDefault(x => x.Id == d.Id);
                if (existing == null)
                {
                    var newDoc = new Document
                    {
                        Id = d.Id,
                        Type = d.Type,
                        Date = d.Date,
                        FirstName = d.FirstName,
                        LastName = d.LastName,
                        City = d.City
                    };
                    _context.Documents.Add(newDoc);
                    docsAdded++;
                }
                else
                {
                    existing.Type = d.Type;
                    existing.Date = d.Date;
                    existing.FirstName = d.FirstName;
                    existing.LastName = d.LastName;
                    existing.City = d.City;
                    _context.Documents.Update(existing);
                    docsUpdated++;
                }
            }

            await _context.SaveChangesAsync();

            foreach (var i in itemsCsv)
            {
                var existingItem = existingItems.FirstOrDefault(x =>
                    x.DocumentId == i.DocumentId && x.Ordinal == i.Ordinal);

                if (existingItem == null)
                {
                    var newItem = new DocumentItems
                    {
                        DocumentId = i.DocumentId,
                        Ordinal = i.Ordinal,
                        Product = i.Product,
                        Quantity = i.Quantity,
                        Price = i.Price,
                        TaxRate = i.TaxRate
                    };
                    _context.DocumentItems.Add(newItem);
                    itemsAdded++;
                }
                else
                {
                    existingItem.Product = i.Product;
                    existingItem.Quantity = i.Quantity;
                    existingItem.Price = i.Price;
                    existingItem.TaxRate = i.TaxRate;
                    _context.DocumentItems.Update(existingItem);
                    itemsUpdated++;
                }
            }

            await _context.SaveChangesAsync();

            Console.WriteLine($"[IMPORT CSV] Dodano: {docsAdded} dokumentów, zaktualizowano: {docsUpdated}");
            Console.WriteLine($"[IMPORT CSV] Dodano: {itemsAdded} pozycji, zaktualizowano: {itemsUpdated}");
        }

        private static List<T> ReadCsv<T>(string path)
        {
            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, new CsvConfiguration(new CultureInfo("pl-PL"))
            {
                Delimiter = ";",
                HasHeaderRecord = true,
                PrepareHeaderForMatch = args => args.Header.Trim()
            });
            return csv.GetRecords<T>().ToList();
        }

        private void ValidateCsvData(List<DocumentCsvModel> documents, List<DocumentItemCsvModel> documentItems)
        {
            foreach (var d in documents)
            {
                if (d.Date == default)
                    throw new InvalidDataException($"Dokument Id={d.Id} ma nieprawidłową datę.");
            }

            foreach (var i in documentItems)
            {
                if (i.DocumentId <= 0)
                    throw new InvalidDataException($"Pozycja {i.Product} ma nieprawidłowy DocumentId.");
            }
        }

        public async Task<(List<DocumentCsvModel> documents, List<DocumentItemCsvModel> items)> ReadCsvFilesAsync(
            string documentsCsvPath, string documentItemsCsvPath)
        {
            if (string.IsNullOrWhiteSpace(documentsCsvPath) || string.IsNullOrWhiteSpace(documentItemsCsvPath))
                throw new ArgumentException("Ścieżki do plików nie mogą być puste.");

            if (!File.Exists(documentsCsvPath) || !File.Exists(documentItemsCsvPath))
                throw new FileNotFoundException("Nie znaleziono jednego lub obu plików CSV.");

            var documents = await Task.Run(() => ReadCsv<DocumentCsvModel>(documentsCsvPath));
            var items = await Task.Run(() => ReadCsv<DocumentItemCsvModel>(documentItemsCsvPath));

            if (!documents.Any() || !items.Any())
                throw new InvalidDataException("Plik CSV jest pusty lub niepoprawny.");

            return (documents, items);
        }

        public async Task<List<Document>> GetDocumentsAsync()
        {
            return await _context.Documents.ToListAsync();
        }

        public async Task<List<DocumentItems>> GetDocumentItemsAsync(int documentId)
        {
            return await _context.DocumentItems
                .Where(i => i.DocumentId == documentId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync(List<Document> documents, List<DocumentItems> documentItems)
        {
            _context.UpdateRange(documents);
            _context.UpdateRange(documentItems);
            await _context.SaveChangesAsync();
        }

        public async Task ExportDataToCsvAsync(string documentsPath, string itemsPath)
        {
            var documents = await _context.Documents.ToListAsync();
            var items = await _context.DocumentItems.ToListAsync();

            using (var writer = new StreamWriter(documentsPath))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(new CultureInfo("pl-PL"))
            {
                Delimiter = ";"
            }))
            {
                await csv.WriteRecordsAsync(documents);
            }

            using (var writer = new StreamWriter(itemsPath))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(new CultureInfo("pl-PL"))
            {
                Delimiter = ";"
            }))
            {
                await csv.WriteRecordsAsync(items);
            }
        }
    }

    public class DocumentCsvModel
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    public class DocumentItemCsvModel
    {
        public int DocumentId { get; set; }
        public int Ordinal { get; set; }
        public string Product { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TaxRate { get; set; }
    }
}
