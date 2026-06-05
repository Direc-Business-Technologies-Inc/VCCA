using Integration.Odoo.Entities;
using Integration.Odoo.Repositories;
using Microsoft.Extensions.Configuration;

namespace Integration.Odoo.Services;

public class OdooConnection : IOdooConnection
{
    private readonly OdooSession _credentials;

    public string BaseUrl    { get; }
    public string Database   { get; }
    public int    Uid        { get; }
    public string Password   { get; }
    public string JsonRpcUrl { get; }

    public OdooConnection(IConfiguration configuration)
    {
        _credentials = ResolveCredentials(configuration);
        BaseUrl      = _credentials.BaseUrl;
        Database     = _credentials.Database;
        Uid          = _credentials.Uid;
        Password     = _credentials.Password;
        JsonRpcUrl   = $"{BaseUrl.TrimEnd('/')}/jsonrpc";
    }

    private static OdooSession ResolveCredentials(IConfiguration configuration)
    {
        OdooSession? session = configuration.GetSection("Odoo").Get<OdooSession>();

        if (session is not null && !string.IsNullOrEmpty(session.BaseUrl))
            return session;

        string baseUrl  = Environment.GetEnvironmentVariable("ODOO_BASE_URL")  ?? string.Empty;
        string database = Environment.GetEnvironmentVariable("ODOO_DATABASE")   ?? string.Empty;
        string uidStr   = Environment.GetEnvironmentVariable("ODOO_UID")        ?? string.Empty;
        string password = Environment.GetEnvironmentVariable("ODOO_PASSWORD")   ?? string.Empty;

        if (!string.IsNullOrEmpty(baseUrl)         &&
            !string.IsNullOrEmpty(database)        &&
            int.TryParse(uidStr, out int uid)       &&
            !string.IsNullOrEmpty(password))
        {
            return new OdooSession
            {
                BaseUrl  = baseUrl,
                Database = database,
                Uid      = uid,
                Password = password
            };
        }

        throw new ArgumentNullException(nameof(configuration),
            "Odoo credentials are not configured. Provide an 'Odoo' section in appsettings " +
            "or set ODOO_BASE_URL, ODOO_DATABASE, ODOO_UID, ODOO_PASSWORD env vars.");
    }
}
