namespace Integration.NS.Repositories;

public interface INetSuiteConnection
{
    string AccountId        { get; }
    string SuiteQLRoot      { get; }
    string RecordApiRoot    { get; }
    string TokenEndpointUrl { get; }
    Task<string> GetAccessTokenAsync();
}
