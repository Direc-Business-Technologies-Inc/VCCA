using Application.DataTransferObjects.Transactions.Commons;

namespace Web.BlazorServer.ViewModels.System;

public class DocumentNumberVM : EntityVM
{
    public DocumentTypeVM DocumentType { get; set; }
    public string Code { get; set; }
    public string Prefix { get; set; }
}
