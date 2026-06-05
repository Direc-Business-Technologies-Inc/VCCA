using Integration.NS.Entities;
using Integration.NS.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text.Json;

namespace Integration.NS.Services;

public class NetSuiteConnection : INetSuiteConnection
{
    private readonly NetSuiteSession _credentials;
    private static readonly HttpClient _httpClient = new();
    private readonly SemaphoreSlim _tokenLock = new(1, 1);

    private string   _accessToken       = string.Empty;
    private DateTime _tokenExpiryTime   = DateTime.MinValue;
    private string   _jwtToken          = string.Empty;
    private DateTime _jwtTokenExpiryTime = DateTime.MinValue;

    public string AccountId        { get; }
    public string SuiteQLRoot      { get; }
    public string RecordApiRoot    { get; }
    public string TokenEndpointUrl { get; }

    public NetSuiteConnection(IConfiguration configuration)
    {
        _credentials = ResolveCredentials(configuration);
        AccountId    = _credentials.AccountID;

        string restRoot  = $"https://{AccountId}.suitetalk.api.netsuite.com/services/rest";
        string oauth2Root = $"{restRoot}/auth/oauth2/v1";

        TokenEndpointUrl = $"{oauth2Root}/token";
        RecordApiRoot    = $"{restRoot}/record/v1";
        SuiteQLRoot      = $"{restRoot}/query/v1/suiteql";
    }

    public async Task<string> GetAccessTokenAsync()
    {
        if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiryTime)
            return _accessToken;

        await _tokenLock.WaitAsync();
        try
        {
            // double-check after acquiring the lock
            if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiryTime)
                return _accessToken;

            string jwt = BuildJwtToken();

            var form = new List<KeyValuePair<string, string>>
            {
                new("grant_type",            "client_credentials"),
                new("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"),
                new("client_assertion",      jwt)
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, TokenEndpointUrl)
            {
                Content = new FormUrlEncodedContent(form)
            };

            var response = await _httpClient.SendAsync(request);
            string body  = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"NetSuite token request failed: {response.StatusCode} — {body}");

            var token = JsonSerializer.Deserialize<NetSuiteToken>(body)
                ?? throw new Exception("NetSuite token response was null.");

            _accessToken     = token.access_token;
            _tokenExpiryTime = DateTime.UtcNow.AddMinutes(28); // 2-min buffer before 30-min expiry

            return _accessToken;
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    private string BuildJwtToken()
    {
        if (!string.IsNullOrEmpty(_jwtToken) && DateTime.UtcNow < _jwtTokenExpiryTime)
            return _jwtToken;

        string pemText = File.ReadAllText(_credentials.PrivateKeyPath);
        string pemBase64 = pemText
            .Replace("-----BEGIN PRIVATE KEY-----", "")
            .Replace("-----END PRIVATE KEY-----", "")
            .Replace("\n", "")
            .Replace("\r", "");

        byte[] keyBytes = Convert.FromBase64String(pemBase64);
        using var rsa   = RSA.Create();
        rsa.ImportPkcs8PrivateKey(keyBytes, out _);

        if (rsa.KeySize < 3072)
            throw new InvalidOperationException("RSA key must be at least 3072 bits for NetSuite OAuth.");

        var securityKey  = new RsaSecurityKey(rsa) { KeyId = _credentials.CertificateId };
        var signingCreds = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSsaPssSha256);
        var now          = DateTime.UtcNow;
        var handler      = new JwtSecurityTokenHandler { SetDefaultTimesOnTokenCreation = false };

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer             = _credentials.ConsumerKey,
            Audience           = TokenEndpointUrl,
            IssuedAt           = now,
            Expires            = now.AddMinutes(10),
            Claims             = new Dictionary<string, object> { { "scope", "rest_webservices" } },
            SigningCredentials = signingCreds
        };

        _jwtToken           = handler.WriteToken(handler.CreateToken(descriptor));
        _jwtTokenExpiryTime = now.AddMinutes(8); // 2-min buffer before 10-min expiry

        return _jwtToken;
    }

    private static NetSuiteSession ResolveCredentials(IConfiguration configuration)
    {
        NetSuiteSession? session = configuration.GetSection("NetSuite").Get<NetSuiteSession>();

        if (session is not null && !string.IsNullOrEmpty(session.AccountID))
            return session;

        string accountId      = Environment.GetEnvironmentVariable("ACCOUNT_ID")              ?? string.Empty;
        string certId         = Environment.GetEnvironmentVariable("NETSUITE_CERTIFICATE_ID") ?? string.Empty;
        string consumerKey    = Environment.GetEnvironmentVariable("NETSUITE_CONSUMER_KEY")   ?? string.Empty;
        string privateKeyPath = Environment.GetEnvironmentVariable("PRIVATEKEY_PATH")         ?? string.Empty;

        if (!string.IsNullOrEmpty(accountId)      &&
            !string.IsNullOrEmpty(certId)         &&
            !string.IsNullOrEmpty(consumerKey)    &&
            !string.IsNullOrEmpty(privateKeyPath))
        {
            return new NetSuiteSession
            {
                AccountID      = accountId,
                CertificateId  = certId,
                ConsumerKey    = consumerKey,
                PrivateKeyPath = privateKeyPath
            };
        }

        throw new ArgumentNullException("NetSuite credentials are not configured. " +
            "Provide a 'NetSuite' section in appsettings or set ACCOUNT_ID, " +
            "NETSUITE_CERTIFICATE_ID, NETSUITE_CONSUMER_KEY, PRIVATEKEY_PATH env vars.");
    }
}
