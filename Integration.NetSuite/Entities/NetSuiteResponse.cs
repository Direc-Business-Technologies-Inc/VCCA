namespace Integration.NS.Entities;

public class NetSuiteResponse<T>
{
    public int     count        { get; set; }
    public bool    hasMore      { get; set; }
    public int     offset       { get; set; }
    public int     totalResults { get; set; }
    public List<T> items        { get; set; } = [];
}
