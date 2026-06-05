using Integration.Odoo.Entities;
using Integration.Odoo.Repositories;
using System.Text;
using System.Text.Json;

namespace Integration.Odoo.Services;

public class OdooActions(IOdooConnection connection) : IOdooActions
{
    private static readonly HttpClient _httpClient = new();

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // ----------------------------------------------------------------
    // Fetch
    // ----------------------------------------------------------------

    public async Task<List<T>> SearchReadAsync<T>(string model, object[][] domain, string[] fields, int? limit = null, int? offset = null)
    {
        var options = BuildSearchOptions(fields, limit, offset);
        var body    = BuildRequest(model, "search_read", new object[] { domain }, options);
        var result  = await SendAsync<List<T>>(body);
        return result ?? [];
    }

    public async Task<T?> SearchReadOneAsync<T>(string model, object[][] domain, string[] fields)
    {
        var results = await SearchReadAsync<T>(model, domain, fields, limit: 1);
        return results.FirstOrDefault();
    }

    // ----------------------------------------------------------------
    // Write
    // ----------------------------------------------------------------

    public async Task<int> CreateAsync(string model, object payload)
    {
        var body = BuildRequest(model, "create", new object[] { payload }, new { });
        return await SendAsync<int>(body);
    }

    public async Task<bool> WriteAsync(string model, int[] ids, object payload)
    {
        var body = BuildRequest(model, "write", new object[] { ids, payload }, new { });
        return await SendAsync<bool>(body);
    }

    public async Task<bool> UnlinkAsync(string model, int[] ids)
    {
        var body = BuildRequest(model, "unlink", new object[] { ids }, new { });
        return await SendAsync<bool>(body);
    }

    // ----------------------------------------------------------------
    // Internals
    // ----------------------------------------------------------------

    private object BuildRequest(string model, string odooMethod, object positionalArgs, object options)
        => new
        {
            jsonrpc = "2.0",
            method  = "call",
            id      = 1,
            @params = new
            {
                service = "object",
                method  = "execute_kw",
                args    = new object[]
                {
                    connection.Database,
                    connection.Uid,
                    connection.Password,
                    model,
                    odooMethod,
                    positionalArgs,
                    options
                }
            }
        };

    private static Dictionary<string, object> BuildSearchOptions(string[] fields, int? limit, int? offset)
    {
        var options = new Dictionary<string, object> { ["fields"] = fields };
        if (limit.HasValue)  options["limit"]  = limit.Value;
        if (offset.HasValue) options["offset"] = offset.Value;
        return options;
    }

    private async Task<T> SendAsync<T>(object requestBody)
    {
        string json = JsonSerializer.Serialize(requestBody);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync(connection.JsonRpcUrl, content);
        string responseJson = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception($"Odoo HTTP error [{httpResponse.StatusCode}]: {responseJson}");

        var wrapper = JsonSerializer.Deserialize<OdooResponse<T>>(responseJson, _jsonOptions)
            ?? throw new Exception("Odoo response deserialized to null.");

        if (!wrapper.IsSuccess)
            throw new Exception($"Odoo error [{wrapper.Error?.Code}]: {wrapper.Error?.Data?.Message ?? wrapper.Error?.Message}");

        return wrapper.Result!;
    }
}
