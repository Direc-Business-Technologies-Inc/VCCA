namespace Integration.Odoo.Entities;

public class OdooSession
{
    public string BaseUrl  { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public int    Uid      { get; set; }
    public string Password { get; set; } = string.Empty;
}
