using B1SLayer;
using Integration.SAP.Entities;
using Integration.SAP.Repositories;
using Microsoft.Extensions.Configuration;

namespace Integration.SAP.Services;

public class ServiceLayer
    : IServiceLayer
{
    IConfiguration configuration;
    ServiceLayerAccount Credentials { get; set; }
    public SLConnection Access { get; private set; }

    public ServiceLayer(IConfiguration _configuration)
    {
        configuration = _configuration;
        Credentials = GetCredentials();
        Access = new SLConnection(
            Credentials.Uri,
            Credentials.CompanyDB,
            Credentials.UserName,
            Credentials.Password);
        Access.LoginAsync().ConfigureAwait(false);
    }

    ServiceLayerAccount GetCredentials()
    {

        ServiceLayerAccount? credentials = configuration.GetSection("SAPSL").Get<ServiceLayerAccount>();

        if (credentials is null)
        {
            string uri = Environment.GetEnvironmentVariable("SAPSL__URI") ?? string.Empty;
            string companyDB = Environment.GetEnvironmentVariable("SAPSL__CompanyDB") ?? string.Empty;
            string userName = Environment.GetEnvironmentVariable("SAPSL__UserName") ?? string.Empty;
            string password = Environment.GetEnvironmentVariable("SAPSL__Password") ?? string.Empty;

            if (!string.IsNullOrEmpty(uri) &&
                !string.IsNullOrEmpty(companyDB) &&
                !string.IsNullOrEmpty(userName) &&
                !string.IsNullOrEmpty(password))
                credentials = new ServiceLayerAccount
                {
                    Uri = uri,
                    CompanyDB = companyDB,
                    UserName = userName,
                    Password = password
                };
            else
                throw new ArgumentNullException("SAP Service Layer Credentials is not configured");
        }

        return credentials;
    }

}
