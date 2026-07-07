namespace Web.BlazorServer.Components.Common.Abstraction;

public partial class AppWatermark
{
    #region Parameters
    [Parameter] public string Text { get; set; } = string.Empty;

    [Parameter] public RenderFragment? ChildContent { get; set; }
    #endregion Parameters

    // Builds a tiled, rotated, repeating-text SVG as a data URI so the backdrop
    // pattern word follows the Text parameter (e.g. "Cylinder" / "Bulk").
    private string BuildSvg()
    {
        var svg =
            "<svg xmlns='http://www.w3.org/2000/svg' width='100' height='64'>" +
            "<text x='50' y='38' text-anchor='middle' transform='rotate(-30 50 32)' " +
            "font-family='Arial, sans-serif' font-size='18' font-weight='700' " +
            $"fill='#000000'>{Text}</text></svg>";

        return "data:image/svg+xml," + Uri.EscapeDataString(svg);
    }
}
