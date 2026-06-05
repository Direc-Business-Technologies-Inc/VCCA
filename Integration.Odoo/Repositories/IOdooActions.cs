namespace Integration.Odoo.Repositories;

public interface IOdooActions
{
    // Fetch
    Task<List<T>> SearchReadAsync<T>(string model, object[][] domain, string[] fields, int? limit = null, int? offset = null);
    Task<T?>      SearchReadOneAsync<T>(string model, object[][] domain, string[] fields);

    // Write
    Task<int>  CreateAsync(string model, object payload);
    Task<bool> WriteAsync(string model, int[] ids, object payload);
    Task<bool> UnlinkAsync(string model, int[] ids);
}
