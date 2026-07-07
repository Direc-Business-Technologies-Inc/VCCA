using Domain.Entities.Transaction.Common;
using Domain.ValueObjects.Transaction;

namespace Domain.Extensions;

public static class DomainTransactionalDocsExtensions
{
    public static T UpdateSapDocNums<T>(this T transDoc, SapDocumentReferenceVO sapDocNums) where T : TransactionalDocumentDEM
    {
        transDoc
            .Update(sapDocNums);

        return transDoc;
    }
}
