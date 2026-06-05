namespace Integration.Odoo.Entities;

public class OdooError
{
    public int          Code    { get; set; }
    public string       Message { get; set; } = string.Empty;
    public OdooErrorData? Data  { get; set; }
}

public class OdooErrorData
{
    public string   Name      { get; set; } = string.Empty;
    public string   Debug     { get; set; } = string.Empty;
    public string   Message   { get; set; } = string.Empty;
    public string[] Arguments { get; set; } = [];
}
