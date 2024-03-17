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
 * Copyright (c) 2012-2023 Jingwood <jingwood at unvell.com>
 * Copyright (c) 2012-2023 unvell inc. All rights reserved.
 * 
 ****************************************************************************/

#if AVALONIA


using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using System;
using System.Linq;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Interaction;

namespace unvell.ReoGrid.AvaloniaPlatform
{
    [TemplatePart("Part_SortAZItem", typeof(RadioButton))]
    [TemplatePart("Part_SortZAItem", typeof(RadioButton))]
    [TemplatePart("Part_OkButton", typeof(Button))]
    [TemplatePart("Part_CancelButton", typeof(Button))]
    [TemplatePart("Part_SelectAll", typeof(CheckBox))]
    [TemplatePart("Part_Popup", typeof(Popup))]
    public class ColumnFilterContextMenu : SelectingItemsControl
    {
        RadioButton SortAZItem;
        RadioButton SortZAItem;

        Button OkButton;

        Button CancelButton;
        CheckBox SelectAllButton;

        Popup Popup;

        public unvell.ReoGrid.Data.AutoColumnFilter.AutoColumnFilterBody HeaderBody { get; set; }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            this.SortAZItem = e.NameScope.Find<RadioButton>("Part_SortAZItem");
            this.SortAZItem = e.NameScope.Find<RadioButton>("Part_SortZAItem");
            this.OkButton = e.NameScope.Find<Button>("Part_OkButton");
            this.CancelButton = e.NameScope.Find<Button>("Part_CancelButton");
            this.SelectAllButton = e.NameScope.Find<CheckBox>("Part_SelectAll");
            this.Popup = e.NameScope.Find<Popup>("Part_Popup");

            this.SortAZItem.Click += SortAZItem_Click;
            this.SortZAItem.Click += SortZAItem_Click;
            this.OkButton.Click += OkButton_Click;
            this.CancelButton.Click += CancelButton_Click;
        }

        private void CancelButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.Popup.Close();
        }

        private void OkButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            HeaderBody.IsSelectAll = false;
            HeaderBody.selectedTextItems.Clear();
            HeaderBody.SelectedTextItems.AddRange(this.Selection.SelectedItems.OfType<string>());
            HeaderBody.autoFilter.Apply();
            this.Popup.Close();
        }

        private void SortZAItem_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var worksheet = HeaderBody.ColumnHeader.Worksheet;
            try
            {
                worksheet.SortColumn(HeaderBody.ColumnHeader.Index, HeaderBody.autoFilter.ApplyRange, SortOrder.Descending);
            }
            catch (Exception ex)
            {
                worksheet.NotifyExceptionHappen(ex);
            }
        }

        private void SortAZItem_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var headerBody = this.HeaderBody;
            var worksheet = headerBody.ColumnHeader.Worksheet;
            try
            {

                worksheet.SortColumn(headerBody.ColumnHeader.Index, headerBody.autoFilter.ApplyRange, SortOrder.Ascending);
            }
            catch (Exception ex)
            {
                worksheet.NotifyExceptionHappen(ex);
            }
        }

        internal static void ShowFilterPanel(unvell.ReoGrid.Data.AutoColumnFilter.AutoColumnFilterBody headerBody, Point point)
        {
            if (headerBody.ColumnHeader == null || headerBody.ColumnHeader.Worksheet == null) return;

            var worksheet = headerBody.ColumnHeader.Worksheet;
            if (worksheet == null) return;

            RGRect headerRect = unvell.ReoGrid.Views.ColumnHeaderView.GetColHeaderBounds(worksheet, headerBody.ColumnHeader.Index, point);
            if (headerRect.Width == 0 || headerRect.Height == 0) return;

            RGRect buttonRect = headerBody.GetColumnFilterButtonRect(headerRect.Size);

            if (headerBody.ContextMenu == null)
            {
                var filterPanel = new ColumnFilterContextMenu()
                {
                    HeaderBody = headerBody,
                };

                filterPanel.Template = new FuncControlTemplate<ColumnFilterContextMenu>((p, ns) =>
                {
                    var popup = new Popup();
                    var az = new RadioButton();
                    var za = new RadioButton();
                    var okbtn = new Button();
                    var canclbtn = new Button();
                    var selectCb = new CheckBox();

                    popup = new Popup()
                    {
                        Child =
                        new DockPanel() { }.SetChilds(
                            new DockPanel().SetChilds(
                                az = new RadioButton()
                                {
                                    Content = "A-Z"
                                },
                                za = new RadioButton()
                                {
                                    Content = "Z-A"
                                }),
                            new DockPanel() { }.SetChilds(),
                            new ListBox()
                        ),
                       
                    };

                    ns.Register("Part_Popup", popup);
                    ns.Register("Part_SortAZItem", az);
                    ns.Register("Part_SortZAItem", za);
                    ns.Register("Part_OkButton", okbtn);
                    ns.Register("Part_CancelButton", canclbtn);
                    ns.Register("Part_SelectAll", selectCb);
                    return popup;
                });

                headerBody.ContextMenu = filterPanel;
            }

            if (headerBody.ContextMenu != null)
            {
                if (headerBody.ContextMenu is ColumnFilterContextMenu filterPanel)
                {
                    if (headerBody.DataDirty)
                    {
                        // todo: keep select status for every items before clear

                        try
                        {
                            headerBody.ColumnHeader.Worksheet.ControlAdapter.ChangeCursor(CursorStyle.Busy);

                            var items = headerBody.GetDistinctItems();
                            items.Insert(0, LanguageResource.Filter_SelectAll);
                            filterPanel.ItemsSource = items;

                            if (headerBody.IsSelectAll)
                            {
                                filterPanel.Selection.SelectAll();
                            }
                            else
                            {
								foreach (var item in headerBody.selectedTextItems)
								{
                                    filterPanel.SelectedItems.Add(item);
                                }
                            }
                        }
                        finally
                        {
                            headerBody.ColumnHeader.Worksheet.ControlAdapter.ChangeCursor(CursorStyle.PlatformDefault);
                        }

                        headerBody.DataDirty = false;
                        headerBody.IsSelectAll = true;
                    }
                }

                var pp = new Graphics.Point(headerRect.Right - 240, buttonRect.Bottom + 1);

                pp = worksheet.ControlAdapter.PointToScreen(pp);

                var rect = new RGRect(pp, pp);

                headerBody.ContextMenu.Popup.Placement = PlacementMode.AnchorAndGravity;
                headerBody.ContextMenu.Popup.PlacementRect = rect;
                headerBody.ContextMenu.Popup.PlacementAnchor = Avalonia.Controls.Primitives.PopupPositioning.PopupAnchor.Left;
                headerBody.ContextMenu.Popup.Open();
            }
        }
    }
}
#endif