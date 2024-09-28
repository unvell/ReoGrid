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
static class SystemColors
{
    private static RGColor ToAvaloniaColor(this System.Drawing.Color color)
    {
        return RGColor.FromArgb(color.A, color.R, color.G, color.B);
    }

    public static RGColor ControlDarkDark { get; } = System.Drawing.SystemColors.ControlDarkDark.ToAvaloniaColor();
    public static RGColor Highlight { get; } = System.Drawing.SystemColors.Highlight.ToAvaloniaColor();
    public static RGColor Window { get; } = System.Drawing.SystemColors.Window.ToAvaloniaColor();
    public static RGColor WindowText { get; } = System.Drawing.SystemColors.WindowText.ToAvaloniaColor();
    public static RGColor Control { get; } = System.Drawing.SystemColors.Control.ToAvaloniaColor();
    public static RGColor ControlLight { get; } = System.Drawing.SystemColors.ControlLight.ToAvaloniaColor();
    public static RGColor ControlDark { get; } = System.Drawing.SystemColors.ControlDark.ToAvaloniaColor();
}
#endif
