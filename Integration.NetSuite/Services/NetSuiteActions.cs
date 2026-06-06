using Database.Libraries.Repositories;
using Integration.NS.Entities;
using Integration.NS.Repositories;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Integration.NS.Services;

public class NetSuiteActions(INetSuiteConnection connection, ISqlQueryManager queryManager)
    : INetSuiteActions
{
    private static readonly HttpClient _httpClient = new();

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
    };

    // ----------------------------------------------------------------
    // REST Record API
    // ----------------------------------------------------------------

    public async Task<T> GetAsync<T>(string resource, object id)
    {
        string url = $"{connection.RecordApiRoot}/{resource}/{id}";
        return await SendAsync<T>(url, null, HttpMethod.Get);
    }

    public async Task<T> PostAsync<T, U>(string resource, U payload)
    {
        string url  = $"{connection.RecordApiRoot}/{resource}";
        string body = JsonSerializer.Serialize(payload);
        return await SendAsync<T>(url, body, HttpMethod.Post);
    }

    public async Task<U> PatchAsync<U>(string resource, object id, U payload)
    {
        string url  = $"{connection.RecordApiRoot}/{resource}/{id}";
        string body = JsonSerializer.Serialize(payload);
        await SendAsync<object>(url, body, HttpMethod.Patch);
        return payload;
    }

    // ----------------------------------------------------------------
    // SuiteQL — script file
    // ----------------------------------------------------------------

    public async Task<List<T>> QueryAsync<T>(string scriptName)
    {
        string suiteql = LoadScript(scriptName);
        return await ExecuteSuiteQLAsync<T>(suiteql);
    }

    public async Task<List<T>> QueryAsync<T, U>(string scriptName, U parameters)
    {
        string suiteql = LoadScript(scriptName);
        return await ExecuteSuiteQLAsync<T>(suiteql, parameters);
    }

    public async Task<T?> SingleAsync<T>(string scriptName)
    {
        var results = await QueryAsync<T>(scriptName);
        return results.FirstOrDefault();
    }

    public async Task<T?> SingleAsync<T, U>(string scriptName, U parameters)
    {
        var results = await QueryAsync<T>(scriptName);
        return results.FirstOrDefault();
    }

    // ----------------------------------------------------------------
    // SuiteQL — raw string
    // ----------------------------------------------------------------

    public Task<List<T>> RawQueryAsync<T>(string suiteql)
        => ExecuteSuiteQLAsync<T>(suiteql);

    public async Task<T?> RawQueryOneAsync<T>(string suiteql)
    {
        var results = await ExecuteSuiteQLAsync<T>(suiteql);
        return results.FirstOrDefault();
    }

    // ----------------------------------------------------------------
    // Internals
    // ----------------------------------------------------------------

    private async Task<List<T>> ExecuteSuiteQLAsync<T>(string suiteql)
    {
        string formatted = ApplyParameters(suiteql, new { });
        string body      = JsonSerializer.Serialize(new { q = formatted });

        var response = await SendAsync<NetSuiteResponse<T>>(connection.SuiteQLRoot, body, HttpMethod.Post);
        return response?.items ?? [];
    }

    private async Task<List<T>> ExecuteSuiteQLAsync<T>(string suiteql, object parameters)
    {
        string formatted = ApplyParameters(suiteql, parameters);
        string body = JsonSerializer.Serialize(new { q = formatted });

        var response = await SendAsync<NetSuiteResponse<T>>(connection.SuiteQLRoot, body, HttpMethod.Post);
        return response?.items ?? [];
    }

    private async Task<T> SendAsync<T>(string url, string? body, HttpMethod method)
    {
        string token = await connection.GetAccessTokenAsync();

        using var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("Prefer", "transient");

        if (!string.IsNullOrEmpty(body))
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();
            throw new Exception($"NetSuite request failed [{response.StatusCode}]: {error}");
        }

        string responseJson = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(responseJson))
            return default!;

        return JsonSerializer.Deserialize<T>(responseJson, _jsonOptions)!;
    }

    private string LoadScript(string scriptName)
    {
        queryManager.GetSqlScriptWithMetadata(scriptName, out string content, out bool found);

        if (!found)
            throw new FileNotFoundException($"NetSuite SuiteQL script '{scriptName}' was not found in the Scripts directory.");

        return content;
    }

    private static string ApplyParameters(string sql, object parameters)
    {
        foreach (var prop in parameters.GetType().GetProperties())
        {
            var value = prop.GetValue(parameters);

            string replacement = value switch
            {
                null => "NULL",
                string s => $"'{s.Replace("'", "''")}'",
                char c => $"'{c.ToString().Replace("'", "''")}'",
                bool b => b ? "TRUE" : "FALSE",
                _ when value is IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
                _ => $"'{value.ToString()?.Replace("'", "''")}'"
            };

            sql = sql.Replace($"@{prop.Name}", replacement);
        }

        return sql;
    }
}
