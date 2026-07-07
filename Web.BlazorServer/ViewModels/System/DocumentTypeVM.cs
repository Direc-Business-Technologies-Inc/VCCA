using Application.DataTransferObjects.Commons;

namespace Web.BlazorServer.ViewModels.System;

public class DocumentTypeVM : EntityDTO
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
}
