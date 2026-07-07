
namespace Web.BlazorServer.Handlers.Repositories.System;

public interface IDocumentNumberHandler
{
    Task<DocumentNumberVM> GetDocumentNumberAsync(Guid documentTypeId);
    Task<DocumentNumberVM> GetDocumentNumberAsync(string documentTypeName);
}
