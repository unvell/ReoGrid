/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net/
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * Author: Jingwood <jingwood at unvell.com>
 *
 * Copyright (c) 2012-2025 Jingwood <jingwood at unvell.com>
 * Copyright (c) 2012-2025 UNVELL Inc. All rights reserved.
 * 
 ****************************************************************************/

#if WPF

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using unvell.ReoGrid.Interaction;
using RGPoint = unvell.ReoGrid.Graphics.Point;
using RGRect = unvell.ReoGrid.Graphics.Rectangle;
using RGRectF = System.Drawing.RectangleF;

namespace unvell.ReoGrid.WPF
{
  internal class ColumnFilterContextMenu : ContextMenu
  {
    internal MenuItem SortAZItem { get; set; }
    internal MenuItem SortZAItem { get; set; }
    internal ListBox CheckedListBox { get; set; }
    internal Button OkButton { get; set; }
    internal Button CancelButton { get; set; }

    public ColumnFilterContextMenu()
    {
      InitializeComponents();
    }

    private void InitializeComponents()
    {
      // Set basic properties of ContextMenu
      this.Width = 240;
      this.Height = 340;

      // 排序菜单项
      SortAZItem = new MenuItem() { Header = LanguageResource.Filter_SortAtoZ };
      SortZAItem = new MenuItem() { Header = LanguageResource.Filter_SortZtoA };

      this.Items.Add(SortAZItem);
      this.Items.Add(SortZAItem);
      this.Items.Add(new Separator());

      // Create custom control to contain list box and buttons
      var customControl = new ContentControl() { Width = 200 };
      var mainGrid = new Grid();

      // Define rows
      mainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(240) }); // List box
      mainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });      // Buttons

      // Checked list box
      CheckedListBox = new ListBox()
      {
        SelectionMode = SelectionMode.Multiple,
        ItemContainerStyle = CreateNoHighlightItemStyle(),
        // Disable selection highlight of ListBox itself
        FocusVisualStyle = null,
        // Disable background drawing of ListBox
        Background = Brushes.Transparent
      };

      // Set data template to display checkboxes
      var dataTemplate = new DataTemplate();
      var stackPanelFactory = new FrameworkElementFactory(typeof(StackPanel));
      stackPanelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
      stackPanelFactory.SetValue(StackPanel.MarginProperty, new Thickness(2));

      var checkBoxFactory = new FrameworkElementFactory(typeof(CheckBox));
      // Key: disable validation error template
      checkBoxFactory.SetValue(Validation.ErrorTemplateProperty, null);
      checkBoxFactory.SetBinding(CheckBox.ContentProperty, new Binding("."));
      // Use converter to handle null values
      checkBoxFactory.SetBinding(CheckBox.IsCheckedProperty, new Binding("IsSelected")
      {
        RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(ListBoxItem), 1),
        Mode = BindingMode.TwoWay,
        //ValidatesOnDataErrors = false, // Disable data error validation
        ValidatesOnExceptions = false, // Disable exception validation
      });

      // Make CheckBox fill the entire row, click anywhere to select
      checkBoxFactory.SetValue(HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch);
      checkBoxFactory.SetValue(VerticalContentAlignmentProperty, VerticalAlignment.Center);

      // Add click event handler
      stackPanelFactory.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler((s, e) =>
      {
        var listBoxItem = FindVisualParent<ListBoxItem>((DependencyObject)s);
        if (listBoxItem != null)
        {
          // Toggle selection state without triggering ListBoxItem selection highlight
          listBoxItem.IsSelected = !listBoxItem.IsSelected;
          e.Handled = true; // Prevent event from propagating further
        }
      }));

      checkBoxFactory.AddHandler(CheckBox.UncheckedEvent, new RoutedEventHandler((s, e) =>
      {
        var checkBox = s as CheckBox;
        var item = checkBox?.DataContext;
        if (item?.ToString() == LanguageResource.Filter_SelectAll)
          CheckedListBox.SelectedItems.Clear();
      }));

      stackPanelFactory.AppendChild(checkBoxFactory);
      dataTemplate.VisualTree = stackPanelFactory;
      CheckedListBox.ItemTemplate = dataTemplate;

      CheckedListBox.SelectionChanged += CheckedListBox_SelectionChanged;

      Grid.SetRow(CheckedListBox, 0);
      mainGrid.Children.Add(CheckedListBox);

      // Button panel
      var buttonPanel = new StackPanel()
      {
        Orientation = Orientation.Horizontal,
        HorizontalAlignment = HorizontalAlignment.Right,
        Margin = new Thickness(0, 4, 0, 0)
      };

      OkButton = new Button()
      {
        Content = LanguageResource.Button_OK,
        Width = 70,
        Margin = new Thickness(0, 0, 8, 0)
      };

      CancelButton = new Button()
      {
        Content = LanguageResource.Button_Cancel,
        Width = 70,
      };
      CancelButton.Click += (s, e) => this.IsOpen = false;

      buttonPanel.Children.Add(OkButton);
      buttonPanel.Children.Add(CancelButton);

      Grid.SetRow(buttonPanel, 1);
      mainGrid.Children.Add(buttonPanel);
      mainGrid.Margin = new Thickness(20, 0, 0, 0);

      customControl.Content = mainGrid;

      // Create MenuItem to contain custom control
      var containerMenuItem = new MenuItem()
      {
        StaysOpenOnClick = true,
        Header = customControl
      };

      // Create style to disable highlight effect
      var noHighlightStyle = new Style(typeof(MenuItem));
      noHighlightStyle.Setters.Add(new Setter(MenuItem.BackgroundProperty, Brushes.Transparent));
      noHighlightStyle.Setters.Add(new Setter(MenuItem.BorderBrushProperty, Brushes.Transparent));

      // Create simple control template, remove all visual state transformations
      var template = new ControlTemplate(typeof(MenuItem));
      var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
      contentPresenter.SetValue(ContentPresenter.ContentProperty, new TemplateBindingExtension(MenuItem.HeaderProperty));
      template.VisualTree = contentPresenter;

      noHighlightStyle.Setters.Add(new Setter(MenuItem.TemplateProperty, template));
      containerMenuItem.Style = noHighlightStyle;

      this.Items.Add(containerMenuItem);
    }

    // Optimized no-highlight style creation method
    private Style CreateNoHighlightItemStyle()
    {
      var style = new Style(typeof(ListBoxItem));

      // Basic style settings
      style.Setters.Add(new Setter(ListBoxItem.BackgroundProperty, Brushes.Transparent));
      style.Setters.Add(new Setter(ListBoxItem.BorderBrushProperty, Brushes.Transparent));
      style.Setters.Add(new Setter(ListBoxItem.BorderThicknessProperty, new Thickness(0)));
      style.Setters.Add(new Setter(ListBoxItem.PaddingProperty, new Thickness(0)));
      style.Setters.Add(new Setter(ListBoxItem.MarginProperty, new Thickness(1)));
      style.Setters.Add(new Setter(ListBoxItem.HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch));
      style.Setters.Add(new Setter(ListBoxItem.VerticalContentAlignmentProperty, VerticalAlignment.Center));
      style.Setters.Add(new Setter(ListBoxItem.FocusVisualStyleProperty, null));
      style.Setters.Add(new Setter(ListBoxItem.IsTabStopProperty, false)); // Disable Tab focus

      // Completely remove highlighting for all states
      var multiTrigger = new MultiTrigger();
      multiTrigger.Conditions.Add(new Condition(ListBoxItem.IsSelectedProperty, true));
      multiTrigger.Conditions.Add(new Condition(ListBoxItem.IsFocusedProperty, false));
      multiTrigger.Setters.Add(new Setter(ListBoxItem.BackgroundProperty, Brushes.Transparent));
      multiTrigger.Setters.Add(new Setter(ListBoxItem.ForegroundProperty, SystemColors.ControlTextBrush));

      var focusedTrigger = new MultiTrigger();
      focusedTrigger.Conditions.Add(new Condition(ListBoxItem.IsSelectedProperty, true));
      focusedTrigger.Conditions.Add(new Condition(ListBoxItem.IsFocusedProperty, true));
      focusedTrigger.Setters.Add(new Setter(ListBoxItem.BackgroundProperty, Brushes.Transparent));
      focusedTrigger.Setters.Add(new Setter(ListBoxItem.BorderBrushProperty, Brushes.Transparent));

      // Disable mouse hover effect
      var hoverTrigger = new Trigger()
      {
        Property = UIElement.IsMouseOverProperty,
        Value = true
      };
      hoverTrigger.Setters.Add(new Setter(ListBoxItem.BackgroundProperty, Brushes.Transparent));
      hoverTrigger.Setters.Add(new Setter(ListBoxItem.BorderBrushProperty, Brushes.Transparent));

      // Add all triggers
      style.Triggers.Add(multiTrigger);
      style.Triggers.Add(focusedTrigger);
      style.Triggers.Add(hoverTrigger);

      // Completely rewrite the ListBoxItem template using ControlTemplate
      var controlTemplate = new ControlTemplate(typeof(ListBoxItem));
      var border = new FrameworkElementFactory(typeof(Border));
      border.SetValue(Border.BackgroundProperty, Brushes.Transparent);
      border.SetValue(Border.BorderThicknessProperty, new Thickness(0));

      var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
      border.AppendChild(contentPresenter);
      controlTemplate.VisualTree = border;

      // Add template trigger
      var templateTrigger = new Trigger();
      templateTrigger.Property = ListBoxItem.IsSelectedProperty;
      templateTrigger.Value = true;
      templateTrigger.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.Transparent));
      controlTemplate.Triggers.Add(templateTrigger);

      style.Setters.Add(new Setter(ListBoxItem.TemplateProperty, controlTemplate));

      return style;
    }

    // Helper method: find visual parent element
    private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
    {
      DependencyObject parentObject = VisualTreeHelper.GetParent(child);
      if (parentObject == null) return null;

      if (parentObject is T parent)
      {
        return parent;
      }
      return FindVisualParent<T>(parentObject);
    }

    internal int SelectedCount { get; set; }

    private bool inEventProcess = false;
    private void CheckedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (inEventProcess) return;

      inEventProcess = true;

      try
      {
        // If there are no changes, return directly
        if ((e.AddedItems.Count == 0 && e.RemovedItems.Count == 0)
            || (CheckedListBox.Items.Count == 0))
          return;

        // Get the select all item (first item)
        var selectAllItem = CheckedListBox.Items[0];
        bool isSelectAllChecked = CheckedListBox.SelectedItems.Contains(selectAllItem);
        bool wasSelectAllAdded = e.AddedItems.Contains(selectAllItem);
        bool wasSelectAllRemoved = e.RemovedItems.Contains(selectAllItem);

        // Get the select all checkbox control
        var listBoxItem = CheckedListBox.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
        CheckBox checkBox = null;
        if (listBoxItem != null)
        {
          // Find all other items except the first select all item in the same level (i.e., find the select all checkbox itself)
          checkBox = FindVisualChild<CheckBox>(listBoxItem);
        }

        // Handle changes to the select all item
        if (wasSelectAllAdded)
        {
          // Select all is checked, select all other items
          for (int i = 1; i < CheckedListBox.Items.Count; i++)
          {
            if (!CheckedListBox.SelectedItems.Contains(CheckedListBox.Items[i]))
            {
              CheckedListBox.SelectedItems.Add(CheckedListBox.Items[i]);
            }
          }

          // Set the select all checkbox to fully checked state
          if (checkBox != null)
          {
            checkBox.IsChecked = true;
          }
        }
        else if (wasSelectAllRemoved && !wasSelectAllAdded)
        {
          // Select all is unchecked and not because select all was added, clear all selections
          CheckedListBox.SelectedItems.Clear();

          // Set the select all checkbox to unchecked state
          if (checkBox != null)
          {
            checkBox.IsChecked = false;
          }
        }
        else
        {
          // Handle selection changes for regular items
          if (CheckedListBox.Items.Count > 1)
          {
            // Calculate how many regular items (excluding select all) are selected
            int selectedCount = 0;
            for (int i = 1; i < CheckedListBox.Items.Count; i++)
            {
              if (CheckedListBox.SelectedItems.Contains(CheckedListBox.Items[i]))
              {
                selectedCount++;
              }
            }

            // Update the select all checkbox state based on selection status
            if (selectedCount == CheckedListBox.Items.Count - 1 && selectedCount > 0)
            {
              // All regular items are selected, select the select all item
              if (!isSelectAllChecked)
              {
                CheckedListBox.SelectedItems.Add(selectAllItem);
              }
              // Set to fully checked state
              if (checkBox != null)
              {
                checkBox.IsChecked = true;
              }
            }
            else if (selectedCount > 0)
            {
              // Ensure the select all item is not in the selected list (indicating partial selection state)
              if (isSelectAllChecked)
              {
                CheckedListBox.SelectedItems.Remove(selectAllItem);
              }
              // Partially selected, set the select all checkbox to indeterminate state
              if (checkBox != null)
              {
                checkBox.IsChecked = null;
              }
            }
            else if (selectedCount == 0)
            {
              // No items selected, deselect the select all item
              if (isSelectAllChecked)
              {
                CheckedListBox.SelectedItems.Remove(selectAllItem);
              }
              // Set to unchecked state
              if (checkBox != null)
              {
                checkBox.IsChecked = false;
              }
            }
          }
        }
      }
      finally
      {
        inEventProcess = false;
      }
    }

    // Helper method: find child elements in the visual tree
    private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
    {
      // Check if the current node is the target type
      if (parent is T typedParent)
        return typedParent;

      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
      {
        var child = VisualTreeHelper.GetChild(parent, i);
        var result = FindVisualChild<T>(child);
        if (result != null)
          return result;
      }
      return null;
    }

    internal static void ShowFilterPanel(Data.AutoColumnFilter.AutoColumnFilterBody headerBody, RGPoint point)
    {
      if (headerBody.ColumnHeader == null || headerBody.ColumnHeader.Worksheet == null) return;

      var worksheet = headerBody.ColumnHeader.Worksheet;
      if (worksheet == null) return;

      RGRect headerRect = Views.ColumnHeaderView.GetColHeaderBounds(worksheet, headerBody.ColumnHeader.Index, point);
      if (headerRect.Width == 0 || headerRect.Height == 0) return;

      RGRect buttonRect = headerBody.GetColumnFilterButtonRect(headerRect.Size);

      if (headerBody.ContextMenuStrip == null)
      {
        var filterPanel = new ColumnFilterContextMenu();

        filterPanel.SortAZItem.Click += (s, e) =>
        {
          try
          {
            worksheet.SortColumn(headerBody.ColumnHeader.Index, headerBody.autoFilter.ApplyRange, SortOrder.Ascending);
          }
          catch (Exception ex)
          {
            worksheet.NotifyExceptionHappen(ex);
          }
        };

        filterPanel.SortZAItem.Click += (s, e) =>
        {
          try
          {
            worksheet.SortColumn(headerBody.ColumnHeader.Index, headerBody.autoFilter.ApplyRange, SortOrder.Descending);
          }
          catch (Exception ex)
          {
            worksheet.NotifyExceptionHappen(ex);
          }
        };

        filterPanel.OkButton.Click += (s, e) =>
        {
          if (filterPanel is ColumnFilterContextMenu wpfFilterPanel)
          {
            // Implement the same filtering logic as the original version
            var listBox = wpfFilterPanel.CheckedListBox;

            // Check if the "Select All" item is selected
            bool isSelectAll = listBox.SelectedItems.Contains(listBox.Items[0]);

            if (isSelectAll)
            {
              headerBody.IsSelectAll = true;
              headerBody.selectedTextItems.Clear();
              for (int i = 2; i < listBox.Items.Count; i++)
              {
                headerBody.selectedTextItems.Add(Convert.ToString(listBox.Items[i]));
              }
            }
            else if (listBox.SelectedItems.Count == 0)
            {
              // No items selected
              headerBody.IsSelectAll = false;
              headerBody.selectedTextItems.Clear();
            }
            else
            {
              // Partially selected
              headerBody.IsSelectAll = null;

              // check Blanks selected
              if (listBox.SelectedItems.Contains(listBox.Items[1]))
              {
                headerBody.ContainsBlank = true;
              }

              headerBody.selectedTextItems.Clear();

              // Add all selected items (except the "Select All" and "Blanks" item)
              for (int i = 2; i < listBox.Items.Count; i++)
              {
                if (listBox.SelectedItems.Contains(listBox.Items[i]))
                {
                  headerBody.selectedTextItems.Add(Convert.ToString(listBox.Items[i]));
                }
              }
            }

            headerBody.autoFilter.Apply();
            wpfFilterPanel.IsOpen = false;
          }
        };

        filterPanel.CancelButton.Click += (s, e) => filterPanel.IsOpen = false;

        headerBody.ContextMenuStrip = filterPanel;
      }

      // update checkitems of filter panel
      if (headerBody.ContextMenuStrip != null)
      {
        if (headerBody.ContextMenuStrip is ColumnFilterContextMenu filterPanel)
        {
          if (headerBody.DataDirty)
          {
            if (headerBody.IsSelectAll == null)
            {
              IList<string> oldItems = new List<string>();
              // Partially selected
              if (filterPanel.CheckedListBox.Items.Count > 1)
              {
                while (filterPanel.CheckedListBox.Items.Count > 1)
                {
                  oldItems.Add(Convert.ToString(filterPanel.CheckedListBox.Items[1]));
                  filterPanel.CheckedListBox.Items.RemoveAt(1);
                }
              }
              filterPanel.CheckedListBox.SelectedItems.Clear();

              try
              {
                headerBody.ColumnHeader.Worksheet.ControlAdapter.ChangeCursor(CursorStyle.Busy);

                var items = headerBody.GetDistinctItems();
                foreach (string item in items)
                {
                  filterPanel.CheckedListBox.Items.Add(item);

                  // Decide whether to select the item based on the previous selection status
                  if (headerBody.selectedTextItems.Contains(item) || !oldItems.Contains(item))
                  {
                    filterPanel.CheckedListBox.SelectedItems.Add(filterPanel.CheckedListBox.Items[filterPanel.CheckedListBox.Items.Count - 1]);
                  }
                }
              }
              finally
              {
                headerBody.ColumnHeader.Worksheet.ControlAdapter.ChangeCursor(CursorStyle.PlatformDefault);
              }

              filterPanel.SelectedCount = filterPanel.CheckedListBox.SelectedItems.Count;
              headerBody.DataDirty = false;
            }
            else if (headerBody.IsSelectAll == false)
            {
              // todo: keep select status for every items before clear
              filterPanel.CheckedListBox.Items.Clear();
              filterPanel.CheckedListBox.Items.Add(LanguageResource.Filter_SelectAll);
              filterPanel.CheckedListBox.Items.Add(LanguageResource.Filter_Blanks);
              filterPanel.CheckedListBox.SelectedItems.Clear();

              try
              {
                headerBody.ColumnHeader.Worksheet.ControlAdapter.ChangeCursor(CursorStyle.Busy);

                var items = headerBody.GetDistinctItems();
                foreach (string item in items)
                {
                  filterPanel.CheckedListBox.Items.Add(item);
                }
              }
              finally
              {
                headerBody.ColumnHeader.Worksheet.ControlAdapter.ChangeCursor(CursorStyle.PlatformDefault);
              }
              filterPanel.SelectedCount = 0;
              headerBody.DataDirty = false;
            }
            else
            {
              // Select All
              // todo: keep selected status for every items
              filterPanel.CheckedListBox.Items.Clear();
              filterPanel.CheckedListBox.Items.Add(LanguageResource.Filter_SelectAll);
              filterPanel.CheckedListBox.Items.Add(LanguageResource.Filter_Blanks);
              // Clear all selections, then select the "Select All" item
              filterPanel.CheckedListBox.SelectedItems.Clear();
              filterPanel.CheckedListBox.SelectedItems.Add(filterPanel.CheckedListBox.Items[0]);

              try
              {
                headerBody.ColumnHeader.Worksheet.ControlAdapter.ChangeCursor(CursorStyle.Busy);

                var items = headerBody.GetDistinctItems();
                foreach (string item in items)
                {
                  filterPanel.CheckedListBox.Items.Add(item);
                  filterPanel.CheckedListBox.SelectedItems.Add(filterPanel.CheckedListBox.Items[filterPanel.CheckedListBox.Items.Count - 1]);
                }
              }
              finally
              {
                headerBody.ColumnHeader.Worksheet.ControlAdapter.ChangeCursor(CursorStyle.PlatformDefault);
              }

              filterPanel.SelectedCount = filterPanel.CheckedListBox.Items.Count - 1;
              headerBody.DataDirty = false;
            }
          }

          // Get screen coordinates
          var pp = new RGPoint(headerRect.Left, buttonRect.Bottom + 1);
          var sp = worksheet.ControlAdapter.PointToScreen(pp);

          // Use ContextMenu with PlacementTarget as null, and set absolute position
          filterPanel.PlacementTarget = worksheet.ControlAdapter as UIElement;
          filterPanel.Placement = PlacementMode.AbsolutePoint;
          filterPanel.HorizontalOffset = sp.X;
          filterPanel.VerticalOffset = sp.Y;

          //4. Ensure display and boundary handling on UI thread
          Application.Current.Dispatcher.BeginInvoke(new Action(() =>
          {
            // Show menu
            filterPanel.IsOpen = true;
          }));
        }
      }
    }
  }
}


#endif // WPF
