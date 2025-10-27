using DocumentManager.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManager.Services.Interfaces
{
    public interface ICsvImportService
    {
        Task ImportDocumentsAsync(string documentsCsvPath, string documentItemsCsvPath);
        Task<(List<DocumentCsvModel> documents, List<DocumentItemCsvModel> items)> ReadCsvFilesAsync(string documentsCsvPath, string documentItemsCsvPath);
        Task<List<Document>> GetDocumentsAsync();
        Task<List<DocumentItems>> GetDocumentItemsAsync(int documentId);
        Task SaveChangesAsync(List<Document> documents, List<DocumentItems> items);
        Task ExportDataToCsvAsync(string documentsPath, string itemsPath);
    }
}
