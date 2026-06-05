namespace Integration.NS.Repositories;

public interface INetSuiteActions
{
    // REST Record API
    Task<T> GetAsync<T>(string resource, object id);
    Task<T> PostAsync<T, U>(string resource, U payload);
    Task<U> PatchAsync<U>(string resource, object id, U payload);

    // SuiteQL — script file
    Task<List<T>> QueryAsync<T>(string scriptName);
    Task<List<T>> QueryAsync<T, U>(string scriptName, U parameters);
    Task<T?>      SingleAsync<T>(string scriptName);
    Task<T?>      SingleAsync<T, U>(string scriptName, U parameters);

    // SuiteQL — raw string
    Task<List<T>> RawQueryAsync<T>(string suiteql);
    Task<T?>      RawQueryOneAsync<T>(string suiteql);
}
