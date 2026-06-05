using System.Text.Json.Serialization;

namespace Integration.Odoo.Entities;

public class OdooResponse<T>
{
    [JsonPropertyName("jsonrpc")] public string     JsonRpc { get; set; } = string.Empty;
    [JsonPropertyName("id")]      public int        Id      { get; set; }
    [JsonPropertyName("result")]  public T?         Result  { get; set; }
    [JsonPropertyName("error")]   public OdooError? Error   { get; set; }

    public bool IsSuccess => Error is null;
}
