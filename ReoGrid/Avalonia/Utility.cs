
#if AVALONIA

using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

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

    public static T WithName<T>(this T control, string name, INameScope nameScope)
        where T : StyledElement
    {
        control.Name = name;
        nameScope.Register(name, control);
        return control; 
    }

    public static T WithContent<T>(this T control, Control content)
        where T : ContentControl
    {
        control.Content = content;
        return control;
    }
    public static T SetGridPlace<T>(this T control, int row,int column)
    where T : Control
    {
        control.SetValue(Grid.RowProperty, row);
        control.SetValue(Grid.ColumnProperty, column);
        return control;
    }

    public static ControlTheme SetChilds(this ControlTheme theme, params IStyle[] styles)
    {
        foreach (var control in styles)
        {
            theme.Children.Add(control);
        }
        return theme;
    }

    public static Style WithSetters(this Style style, params SetterBase[] setters)
    {
        foreach (var setter in setters)
        {
            style.Setters.Add(setter);
        }
        return style;
    }
    public static T Child<T>(this Decorator decorator)
        where T : Control
    {
        return (T)decorator.Child;
    }

    public static T As<T>(this object obj)
    {
        return (T)obj;
    }
}
#endif