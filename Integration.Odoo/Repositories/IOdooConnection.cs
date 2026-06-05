namespace Integration.Odoo.Repositories;

public interface IOdooConnection
{
    string BaseUrl    { get; }
    string Database   { get; }
    int    Uid        { get; }
    string Password   { get; }
    string JsonRpcUrl { get; }
}
