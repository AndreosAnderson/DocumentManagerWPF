namespace DocumentManager.Services.Interfaces
{
    using DocumentManager.Domain;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDocumentService
    {
        Task<List<Document>> GetAllDocumentsAsync();
        Task<Document?> GetDocumentByIdAsync(int id);
        Task AddDocumentAsync(Document document);
        Task UpdateDocumentAsync(Document document);
        Task DeleteDocumentAsync(int id);
    }
}