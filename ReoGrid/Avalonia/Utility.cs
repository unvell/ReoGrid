
#if AVALONIA

using Avalonia.Controls;

namespace unvell.ReoGrid.AvaloniaPlatform;

public static class AvaloinaEx
{
    public static Panel SetChilds(this Panel panel, params Control[] controls)
    {
        foreach (Control control in controls)
        {
            panel.Children.Add(control);
        }
        return panel;
    }
}
#endif