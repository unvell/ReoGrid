#if AVALONIA
using Avalonia.Media;
namespace unvell.ReoGrid.AvaloniaPlatform;
class DashStyles
{
    public static RGDashStyle Solid => null;
    public static RGDashStyle Dash => DashStyle.Dash;
    public static RGDashStyle Dot => DashStyle.DashDot;
    public static RGDashStyle DashDotDot => DashStyle.DashDotDot;
    public static RGDashStyle DashDot => DashStyle.DashDot;
}

/// <summary>
/// System.Drawing.SystemColors
/// </summary>
class SystemColors
{
    public static RGColor ControlDarkDark { get; } = Color.FromArgb(a: 255, r: 105, g: 105, b: 105);
    public static RGColor Highlight { get; } = Color.FromArgb(a: 255, r: 0, g: 120, b: 215);
    public static RGColor Window { get; } = Color.FromArgb(a: 255, r: 255, g: 255, b: 255);
    public static RGColor WindowText { get; } = Color.FromArgb(a: 255, r: 0, g: 0, b: 0);
    public static RGColor Control { get; } = Color.FromArgb(a: 255, r: 240, g: 240, b: 240);
    public static RGColor ControlLight { get; } = Color.FromArgb(a: 255, r: 227, g: 227, b: 227);
    public static RGColor ControlDark { get; } = Color.FromArgb(a: 255, r: 160, g: 160, b: 160);
}
#endif
