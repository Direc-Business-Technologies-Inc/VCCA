using Application.DataTransferObjects.Commons;
using Application.DataTransferObjects.System.Commons;

namespace Application.DataTransferObjects.System;

public class DocumentNumberDTO : EntityDTO
{
    public DocumentTypeDTO DocumentType { get; set; }
    public string Code { get; set; }
    public string Prefix { get; set; }
}
