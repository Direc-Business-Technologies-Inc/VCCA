using Application.DataTransferObjects.Commons;
using Application.DataTransferObjects.System.Commons;
using Domain.Enums.Transaction.Commons;

namespace Application.DataTransferObjects.Transactions.Commons;

public class TransactionalDocumentDTO : AuditableDTO
{
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.None;

    // Per-table running number (DB IDENTITY). AppDocNum is the display value,
    // derived by formatting DocNumber through the doc type's series config.
    public int DocNumber { get; set; }
    public string AppDocNum { get; set; } = string.Empty;
    public SapDocumentReferenceDTO SapReference { get; set; } = new();
    public DocumentTypeDTO DocumentType { get; set; } = new();
}
