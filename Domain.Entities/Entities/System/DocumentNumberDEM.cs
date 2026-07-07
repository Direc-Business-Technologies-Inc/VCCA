
using Ardalis.GuardClauses;
using Domain.Commons;

namespace Domain.Entities.System;

public class DocumentNumberDEM : EntityDEM
{
    public Guid DocumentTypeId {  get; private set; }
    public string Code { get; private set; }
    public string Prefix { get; private set; }

    protected DocumentNumberDEM() { }

    protected DocumentNumberDEM(Guid documentTypeId, string code, string prefix)
    {
        DocumentTypeId = Guard.Against.NullOrEmpty(documentTypeId, nameof(DocumentTypeId), "Document Type Id cannot be null or empty");
        Code = Guard.Against.NullOrEmpty(code, nameof(Code), "Code cannot be null or empty");
        Prefix = Guard.Against.NullOrEmpty(prefix, nameof(Prefix), "Prefix cannot be null or empty");
    }

    public static DocumentNumberDEM Create(Guid documentTypeId, string code, string prefix)
    {
        return new DocumentNumberDEM(documentTypeId, code, prefix);
    }

    public DocumentNumberDEM Update(string code, string prefix)
    {
        Code = code;
        Prefix = prefix;

        return this;
    }

    /**
     * Formats a document table's running number (its per-table IDENTITY column)
     * into the display document number, e.g. "WMS-0001".
     */
    public string Format(int docNumber)
    {
        return $"{Code}{Prefix}-{docNumber:D4}";
    }
}
