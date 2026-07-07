namespace Shared.Utilities;

public static class ExceptionExtensions
{
    public static string GetMeaningfulMessage(this Exception ex)
    {
        var deepest = ex;
        while (deepest.InnerException is not null)
            deepest = deepest.InnerException;

        return deepest.Message;
    }
}
